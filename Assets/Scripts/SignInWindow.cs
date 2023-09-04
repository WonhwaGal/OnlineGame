using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.UI;

public class SignInWindow : AccountDataWindowBase
{
    [SerializeField] private Button _signInButton;

    protected override void SubscriptionsElementsUI()
    {
        base.SubscriptionsElementsUI();
        _signInButton.onClick.AddListener(SignIn);
    }

    private void SignIn()
    {
        ShowLoadingSign();
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
        {
            Username = _username,
            Password = _password,
        }, 
        result =>
        {
            Debug.Log($"Success: {_username}");
            EnterInGameScene();
        }, 
        error =>
        {
            Debug.LogError($"Fail: {error.ErrorMessage}");
        });
    }
}
