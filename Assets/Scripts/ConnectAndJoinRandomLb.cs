using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class ConnectAndJoinRandomLb : MonoBehaviour, IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks
{
    [SerializeField] private ServerSettings _serverSettings;
    [SerializeField] private TMP_Text _stateUiText;

    [SerializeField] private Button _createRoomButton;
    [SerializeField] private Button _joinRoomListButton;
    [SerializeField] private GameObject _buttonPanel;
    [SerializeField] private Transform _roomListPanel;
    [SerializeField] private Button _closeRoomButton;
    [SerializeField] private RoomButton _roomButton;

    private List<RoomInfo> _roomList = new List<RoomInfo>();
    //PhotonNetwork это обертка над static LoadBalancingClient class
    private LoadBalancingClient _lbc;  //логика взаимодействия с сервером

    private const string GAME_MODE_KEY = "gm";
    private const string AI_MODE_KEY = "ai";

    private const string MAP_PROP_KEY = "C0";
    private const string GOLD_PROP_KEY = "C1";

    private List<string> _myExpectedFriends = new List<string>();
    private TypedLobby _sqlLobby = new TypedLobby("SQLLobby", LobbyType.SqlLobby);
    private string[] someArray = new string[] { "1", "2" };

    private void Start()
    {
        _lbc = new LoadBalancingClient();
        _lbc.AddCallbackTarget(this);

        _lbc.ConnectUsingSettings(_serverSettings.AppSettings);

        _createRoomButton.onClick.AddListener(CreateCustomRoom);
        _createRoomButton.interactable = false;
        _joinRoomListButton.onClick.AddListener(() => GetListOfRooms(_roomList));
        _joinRoomListButton.interactable = false;
        _closeRoomButton.transform.parent.gameObject.SetActive(false);
        _closeRoomButton.onClick.AddListener(CloseRoom);
    }

    private void OnDestroy()
    {
        _lbc.RemoveCallbackTarget(this);

        _createRoomButton.onClick.RemoveListener(CreateCustomRoom);
    }

    private void Update()
    {
        if (_lbc == null)
            return;

        _lbc.Service();   //контракт на получение данных

        var state = _lbc.State.ToString();
        _stateUiText.text = state;
;    }

    public void OnConnected()
    {
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        //_lbc.OpJoinRandomRoom();

        _createRoomButton.interactable = true;
        _joinRoomListButton.interactable = true;
        //PhotonNetwork.JoinLobby();
    }

    private void GetListOfRooms(List<RoomInfo> roomList)
    {
        Debug.Log($"getting room list: {roomList.Count} rooms exist");
        for(int i = 0; i < roomList.Count; i++)
        {
            RoomButton roomButton = Instantiate<RoomButton>(_roomButton, _roomListPanel);
            roomButton.RoomName.text = roomList[i].Name;
        }
    }

    private void CreateCustomRoom()
    {
        PhotonNetwork.FindFriends(someArray);

        var randomNumber = Random.Range(0.0f, 1000.0f);
        var roomOptions = new RoomOptions
        {
            MaxPlayers = 12,
            PublishUserId = true, // to reserve a spot for a friend
            CustomRoomPropertiesForLobby = new[] { MAP_PROP_KEY, GOLD_PROP_KEY },
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable { { GOLD_PROP_KEY, 400 }, { MAP_PROP_KEY, "Map3" } }
        };

        var enterRoomParams = new EnterRoomParams
        {
            RoomName = $"Room_{randomNumber}",
            RoomOptions = roomOptions,
            ExpectedUsers = _myExpectedFriends.ToArray(),
            Lobby = _sqlLobby
        };
        _lbc.OpCreateRoom(enterRoomParams);
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList)
    {
        for(int i = 0; i < friendList.Count; i++)
        {
            if (friendList[i].IsOnline && !friendList[i].IsInRoom)
            {
                _myExpectedFriends.Add(friendList[i].UserId);
            }
        }
    }

    public void OnCreatedRoom()
    {
        Debug.Log("OnCreatedRoom");
        _buttonPanel.SetActive(false);
        _roomListPanel.gameObject.SetActive(false);
        _closeRoomButton.transform.parent.gameObject.SetActive(true);
    }

    private void CloseRoom()
    {
        _lbc.CurrentRoom.IsOpen = false;
        Debug.Log($"Current room is open - {_lbc.CurrentRoom.IsOpen}");
    }

    public void OnCreateRoomFailed(short returnCode, string message)
    {
    }

    public void OnCustomAuthenticationFailed(string debugMessage)
    {
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data)
    {
    }

    public void OnDisconnected(DisconnectCause cause)
    {
    }

    public void OnJoinedLobby()
    {
        var sqlLobbyFilter = $"{MAP_PROP_KEY} = Map3 AND {GOLD_PROP_KEY} between 300 and 500";
        var opJoinRandomRoomParams = new OpJoinRandomRoomParams
        {
            SqlLobbyFilter = sqlLobbyFilter,
        };
        
        _lbc.OpJoinRandomRoom(opJoinRandomRoomParams);
    }

    public void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom");
        // receive info about a room
        //var id = _lbc.CurrentRoom.Players.Values.First().UserId;
        //_lbc.CurrentRoom.IsOpen = false;
    }

    public void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        _lbc.OpCreateRoom(new EnterRoomParams());
    }

    public void OnJoinRoomFailed(short returnCode, string message)
    {

    }

    public void OnLeftLobby()
    {
    }

    public void OnLeftRoom()
    {
    }

    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
    }

    public void OnRegionListReceived(RegionHandler regionHandler)
    {
    }

    public void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomList = roomList;
        Debug.Log("New room added");
        GetListOfRooms(roomList);
    }


}
