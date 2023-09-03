using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private TextMeshProUGUI _text;

    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;

    private void Start()
    {
        _connectButton.onClick.AddListener(Connect);
        _disconnectButton.onClick.AddListener(DisconnectFromPhoton);
    }

    private void Connect()
    {
        if (PhotonNetwork.IsConnected)
            return;

        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = Application.version;
    }

    private void DisconnectFromPhoton()
    {
        PhotonNetwork.Disconnect();
        _text.text = "Disconnecting...";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        _text.color = Color.red;
        _text.text = "You are disconnected";

        if (!PhotonNetwork.IsConnected)
        {
            var reason = cause.ToString();
            Debug.Log("Disconnection is caused by: " + reason);
        }
        else
        {
            Debug.Log("Photon is still connected");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        PhotonNetwork.CreateRoom("NewRoom");
        _text.color = Color.green;
        _text.text = "Successful connection";
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }
}
