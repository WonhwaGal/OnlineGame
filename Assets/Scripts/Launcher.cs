using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using System.Collections.Generic;

public class Launcher : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button _connectButton;
    [SerializeField] private Button _disconnectButton;
    [SerializeField] private TextMeshProUGUI _text;

    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _getRoomListButton;
    [SerializeField] private GameObject _buttonPanel;
    [SerializeField] private Transform _roomListPanel;
    [SerializeField] private Button _closeRoomButton;
    [SerializeField] private RoomButton _roomButton;

    private List<RoomInfo> _roomList = new List<RoomInfo>();
    private List<RoomButton> _roomButtons = new List<RoomButton>();
    private bool _wantToSeeRooms = false;

    private void Awake() => PhotonNetwork.AutomaticallySyncScene = true;

    private void Start()
    {
        _connectButton.onClick.AddListener(Connect);
        _disconnectButton.onClick.AddListener(DisconnectFromPhoton);
        Connect();
        _createRoomButton.onClick.AddListener(CreateMyRoom);
        _createRoomButton.interactable = false;
        _getRoomListButton.onClick.AddListener(GetListOfRooms);
        _getRoomListButton.interactable = false;
        _closeRoomButton.transform.parent.gameObject.SetActive(false);
        _closeRoomButton.onClick.AddListener(CloseRoom);
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
        //_text.text = "Disconnecting...";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        //_text.color = Color.red;
        //_text.text = "You are disconnected";

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
        _createRoomButton.interactable = true;
        _getRoomListButton.interactable = true;
        //PhotonNetwork.CreateRoom("NewRoom");
        //_text.color = Color.green;
        //_text.text = "Successful connection";

        //we have to join lobby to get roomList info
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("OnJoinLobby");
    }
    private void GetListOfRooms()
    {
        if (!_wantToSeeRooms)
        {
            _wantToSeeRooms = true;
            _getRoomListButton.interactable = false;
        }

        for (int i = 0; i < _roomList.Count; i++)
        {
            if (_roomList[i].RemovedFromList || !_roomList[i].IsOpen)
            {
                for(int r = 0; r < _roomButtons.Count; r++)
                {
                    if(_roomButtons[r].RoomName.text == _roomList[i].Name)
                    {
                        Destroy(_roomButtons[r].gameObject);
                        _roomButtons.Remove(_roomButtons[r]);
                    } 
                }
            }
            else
            {
                RoomButton roomButton = Instantiate<RoomButton>(_roomButton, _roomListPanel);
                roomButton.RoomName.text = _roomList[i].Name;
                _roomButtons.Add(roomButton);
            }
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("room list updated, number of rooms is " + roomList.Count);
        _roomList = roomList;
        if (_wantToSeeRooms)
            GetListOfRooms();
    }

    private void CreateMyRoom()
    {
        PhotonNetwork.CreateRoom($"Room_{Random.Range(0, 10000)}");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("On created room");
        _buttonPanel.SetActive(false);
        _roomListPanel.gameObject.SetActive(false);
        _closeRoomButton.transform.parent.gameObject.SetActive(true);
    }
    private void CloseRoom()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
    }
}
