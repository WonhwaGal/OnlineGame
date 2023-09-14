using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountWindow : AccountDataWindowBase
{
    [SerializeField] private InputField _mailField;
    [SerializeField] private Button _createAccountButton;
    [SerializeField] private PhotonLauncher _photonLauncher;
    //    private const int HP = 2;
    private string _mail;
    protected override void SubscriptionsElementsUI()
    {
        base.SubscriptionsElementsUI();
        _mailField.onValueChanged.AddListener(UpdateMail);
        _createAccountButton.onClick.AddListener(CreateAccount);
        _createAccountButton.onClick.AddListener(_photonLauncher.Connect);
    }

    private void CreateAccount()
    {
        ShowLoadingSign();
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest()
        {
            Username = _username,
            Email = _mail,
            Password = _password,
        }, 
        result =>
        {
            Debug.Log($"Success: {_username}");
            SetUserData(result.PlayFabId);
            //EnterInGameScene();
        }, 
        error =>
        {
            Debug.LogError($"Fail: {error.ErrorMessage}");
        });
    }

    private void UpdateMail(string mail)
    {
        _mail = mail;
    }
}
