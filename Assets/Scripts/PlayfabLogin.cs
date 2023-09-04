using UnityEngine;
using System;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;

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
