using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayFabAccountManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private UICatalogItem _itemPrefab;
    [SerializeField] private Transform _catalogPanel;
    [SerializeField] private Transform _bundlePanel;
    [SerializeField] private GameObject _newCharacterCreatePanel;
    [SerializeField] private Button _createCharacterButton;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private List<SlotCharacterWidget> _slots;

    private string _characterName;
    private float _startHP;
    private bool _waitingForData = true;
    private WaitForSeconds _span = new WaitForSeconds(0.5f);

    void Start()
    {
        //StartCoroutine(LoadingMessage());
        //PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), OnGetAccount, OnError);
        //PlayFabClientAPI.GetCatalogItems(new GetCatalogItemsRequest(), OnGetCatalogSuccess, OnError);
        //PlayFabServerAPI.GetRandomResultTables(new PlayFab.ServerModels.GetRandomResultTablesRequest(), OnGetRandomResultTables, OnError);
        GetCharacters();
        foreach(var slot in _slots)
            slot.SlotButton.onClick.AddListener(OpenCreateNewCharacter);
        _inputField.onValueChanged.AddListener(OnNameChanged);
        _createCharacterButton.onClick.AddListener(CreateCharacter);
    }

    private void CreateCharacter()
    {
        PlayFabClientAPI.GrantCharacterToUser(new GrantCharacterToUserRequest
        {
            CharacterName = _characterName,
            ItemId = "Robot_token"
        },
        result =>
        {
            UpdateCharacterStatistics(result.CharacterId);
        }, OnError);
    }

    private void UpdateCharacterStatistics(string characterID)
    {
        PlayFabClientAPI.UpdateCharacterStatistics(new UpdateCharacterStatisticsRequest
        {
            CharacterId = characterID,
            CharacterStatistics = new Dictionary<string, int>
            {
                {"Level", 1 },
                {"Gold", 0 },
                {"HP", 2 },   //(int)_startHP
                {"Damage", 5},
                {"Experience", 0 }
            }
        },
        result => 
        {
            Debug.Log("Character creation completed!!!");
            CloseCreateNewCharacter();
            GetCharacters();
        }, OnError);
    }

    private void OnNameChanged(string name) => _characterName = name;

    private void OpenCreateNewCharacter() => _newCharacterCreatePanel.SetActive(true);

    private void CloseCreateNewCharacter() => _newCharacterCreatePanel.SetActive(false);

    private void GetCharacters()
    {
        PlayFabClientAPI.GetAllUsersCharacters(new ListUsersCharactersRequest(),
        result =>
        {
            Debug.Log("Character count is" + result.Characters.Count);
            ShowCharactersInSlots(result.Characters);
        }, OnError);
    }

    private void ShowCharactersInSlots(List<CharacterResult> characters)
    {
        foreach (var slot in _slots)
            slot.ShowEmptySlot();

        if (characters.Count > 0 && characters.Count <= _slots.Count)
        {
            for(int i = 0; i < characters.Count; i++)
            {
                int index = i;
                PlayFabClientAPI.GetCharacterStatistics(new GetCharacterStatisticsRequest
                {
                    CharacterId = characters[index].CharacterId
                },
                result =>
                {
                    var level = result.CharacterStatistics["Level"].ToString();
                    var gold = result.CharacterStatistics["Gold"].ToString();
                    var hp = result.CharacterStatistics["HP"].ToString();
                    var damage = result.CharacterStatistics["Damage"].ToString();
                    var experience = result.CharacterStatistics["Experience"].ToString();
                    _slots[index].ShowCharacterSlost(characters[index].CharacterName, level, gold, hp, damage, experience);
                }, OnError);
            }
        }
        else
        {
            Debug.Log("Add slots for characters");
        }
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
                    _startHP = number;
                    Debug.Log($"PhotonGameManager HP taken from Playfab is {PhotonGameManager.Instance.PlayerStartHP}");
                }

            }
        },
        OnError);
    }
}
