using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using TMPro;
using UnityEngine;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleLabel;
    private bool _waitingForData = true;
    private WaitForSeconds _span = new WaitForSeconds(0.5f);

    void Start()
    {
        StartCoroutine(LoadingMessage());
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccount, OnError);
    }

    private void OnError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError(errorMessage);
    }

    private void OnGetAccount(GetAccountInfoResult result)
    {
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
}
