using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private UICatalogItem _itemPrefab;
    [SerializeField] private Transform _catalogPanel;
    [SerializeField] private Transform _bundlePanel;

    private bool _waitingForData = true;
    private WaitForSeconds _span = new WaitForSeconds(0.5f);

    void Start()
    {
        //StartCoroutine(LoadingMessage());
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccount, OnError);
        //PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);
        //PlayFabServerAPI.GetRandomResultTables(new PlayFab.ServerModels.GetRandomResultTablesRequest(), OnGetRandomResultTables, OnError);
    }

    private void OnGetRandomResultTables(PlayFab.ServerModels.GetRandomResultTablesResult result)
    {
        //result.Tables
    }

    private void OnGetCatalogSuccess(GetCatalogItemsResult result)
    {
        Debug.Log("OnGetCatalogSuccess");
        ShowItems(result.Catalog);
    }

    private void ShowItems(List<CatalogItem> catalog)
    {
        var bundleList = new List<string>();
        Debug.Log("List of items:");
        foreach (CatalogItem item in catalog)
        {
            if (item.Bundle == null)
                InstantiateItem(item.ItemId, _catalogPanel);
            else
            {
                bundleList.Add(item.ItemId);
            }
        }
        Debug.Log("List of bundles:");
        for (int i = 0; i < bundleList.Count; i++)
            InstantiateItem(bundleList[i], _bundlePanel);
    }

    private void InstantiateItem(string name, Transform parent)
    {
        var item = Instantiate(_itemPrefab, parent);
        item.Name.text = name;
    }
    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError(errorMessage);
    }

    private void OnGetAccount(GetAccountInfoResult result)
    {
        GetUserData(result.AccountInfo.PlayFabId, "startHP");
        _waitingForData = false;
        _titleLabel.text = $"User Name: {result.AccountInfo.Username}\nPlayFab ID: {result.AccountInfo.PlayFabId}";
    }

    IEnumerator LoadingMessage()
    {
        int count = 0;
        while (_waitingForData)
        {
            if (count <= 3)
            {
                yield return _span;
                count++;
                _titleLabel.text += ".";
            }
            else
            {
                count = 0;
                _titleLabel.text = "Loading client data ";
            }
        }
    }

    private void GetUserData(string playFabId, string keyData)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            PlayFabId = playFabId
        },
        result =>
        {
            if (keyData == "startHP" && result.Data.ContainsKey(keyData))
            {
                float number = 0;
                if(float.TryParse(result.Data[keyData].Value, out number))
                {
                    PhotonGameManager.Instance.PlayerStartHP = number;
                    Debug.Log($"PhotonGameManager HP taken from Playfab is {PhotonGameManager.Instance.PlayerStartHP}");
                }

            }
        },
        OnError);
    }
}
