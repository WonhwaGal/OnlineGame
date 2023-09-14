using UnityEngine;
using UnityEngine.UI;

public class EnterInGameWindow : MonoBehaviour
{
    [SerializeField] private Button _signInButton;
    [SerializeField] private Button _createAccountButton;
    [SerializeField] private Canvas _enterInGameCanvas;
    [SerializeField] private Canvas _createAccountCanvas;
    [SerializeField] private Canvas _signInCanvas;

    void Start()
    {
        _signInButton.onClick.AddListener(OpenSignInWndow);
        _createAccountButton.onClick.AddListener(CreateAccountWndow);
    }

    private void CreateAccountWndow()
    {
        _createAccountCanvas.enabled = true;
        _enterInGameCanvas.enabled = false;
    }

    private void OpenSignInWndow()
    {
        _signInCanvas.enabled = true;
        _enterInGameCanvas.enabled = false;
    }


}
