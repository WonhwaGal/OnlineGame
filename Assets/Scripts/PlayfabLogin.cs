using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PlayfabLogin : MonoBehaviour
{
    [SerializeField] private Button _loginButton;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Toggle _errorToggle;

    private const string AuthGuidKey = "auth_guid_key";

    void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId))
            PlayFabSettings.staticSettings.TitleId = "F079C";
        _loginButton.onClick.AddListener(LogIn);
        _errorToggle.isOn = false;
        //LogIn();   // to log in automatically
    }

    private void LogIn()
    {
        if (_errorToggle.isOn)
            PlayFabSettings.staticSettings.TitleId = "F111C";
        else
            PlayFabSettings.staticSettings.TitleId = "F079C";

        var needCreation = PlayerPrefs.HasKey(AuthGuidKey);
        var id = PlayerPrefs.GetString(AuthGuidKey, Guid.NewGuid().ToString());

        var request = new LoginWithCustomIDRequest
        {
            CustomId = id,
            CreateAccount = !needCreation
        };
        PlayFabClientAPI.LoginWithCustomID(request, 
            result =>
            {
                PlayerPrefs.SetString(AuthGuidKey, id);
                OnLoginSuccess(result);
            },
            OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        _text.color = Color.green;
        _text.text = "Successful login";
        Debug.Log("Successful login!!!");

        //SetUserData(result.PlayFabId);
        //MakePurchase();
        //GetInventory();
    }

    private void GetInventory()
    {
        PlayFabClientAPI.GetUserInventory(
            new GetUserInventoryRequest(),
            result => ShowInventory(result.Inventory),
            OnLoginError);
    }

    private void ShowInventory(List<ItemInstance> inventory)
    {
        var firstItem = inventory.First();
        Debug.Log($"item id : {firstItem.ItemId}");
        ConsumePotion(firstItem.ItemInstanceId);
    }

    private void ConsumePotion(string itemInstanceId)
    {
        PlayFabClientAPI.ConsumeItem(new ConsumeItemRequest
        {
            ConsumeCount = 1,
            ItemInstanceId = itemInstanceId,
        },
        result =>
        {
            Debug.Log("Complete consume item");
        },
        OnLoginError);
    }

    private void MakePurchase()
    {
        PlayFabClientAPI.PurchaseItem(new PurchaseItemRequest
        {
            CatalogVersion = "homework",
            ItemId = "Crafts_P1",
            Price = 10,
            VirtualCurrency = "PC"
        },
        result =>
        {
            Debug.Log("Complete Purchase Item");
        },
        OnLoginError);
    }

    private void SetUserData(string playFabId)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"time_receive_daily_reward", DateTime.UtcNow.ToString() },
            }
        }, 
        result =>
        {
            Debug.Log("SetUserData");
            GetUserData(playFabId, "time_receive_daily_reward");
        }, 
        OnLoginError);
    }

    private void GetUserData(string playFabId, string keyData)
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest
        {
            PlayFabId = playFabId
        },
        result =>
        {
            if (result.Data.ContainsKey(keyData))
                Debug.Log($"{keyData} : {result.Data[keyData].Value}");  
        }, 
        OnLoginError);
    }

    private void OnLoginError(PlayFabError error)
    {
        _text.color = Color.red;
        _text.text = "Login Failed";
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError(errorMessage);
    }

    private void OnDestroy()
    {
        _loginButton.onClick.RemoveAllListeners();
    }
}
