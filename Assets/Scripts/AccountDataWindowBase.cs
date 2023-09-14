using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;


public class AccountDataWindowBase : MonoBehaviour
{
    [SerializeField] private InputField _usernameField;
    [SerializeField] private InputField _passwordField;
    [SerializeField] private LoadingSign _loadingSign;

    protected string _username;
    protected string _password;

    private void Start()
    {
        SubscriptionsElementsUI();
    }

    protected virtual void SubscriptionsElementsUI()
    {
        _usernameField.onValueChanged.AddListener(UpdateUserName);
        _passwordField.onValueChanged.AddListener(UpdatePassword);
    }

    private void UpdatePassword(string password)
    {
        _password = password;
    }

    private void UpdateUserName(string username)
    {
        _username = username;
        PhotonLauncher.SetNickName(username);
    }

    protected void EnterInGameScene()
    {
        SceneManager.LoadScene(1);
    }

    protected void ShowLoadingSign()
    {
        _loadingSign.gameObject.SetActive(true);
        _loadingSign.StartLoading();
    }

    protected void SetUserData(string playFabId)
    {
        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                {"time_receive_daily_reward", DateTime.UtcNow.ToString() },
                {"startHP",  "2" }
            }
        },
        result =>
        {
            Debug.Log("SetUserData");
        },
        OnLoginError);
    }
    private void OnLoginError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError(errorMessage);
    }
}
