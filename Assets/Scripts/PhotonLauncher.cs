using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PhotonLauncher : MonoBehaviourPunCallbacks
{
	[SerializeField] private byte _maxPlayers = 4;

	private bool _isConnecting;
	private string _version = "1";

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

	public void Connect()
	{
		_isConnecting = true;

		if (PhotonNetwork.IsConnected)
		{
			PhotonNetwork.JoinRandomRoom();
		}
		else
		{
			PhotonNetwork.ConnectUsingSettings();
			PhotonNetwork.GameVersion = this._version;
		}
	}

	public override void OnConnectedToMaster()
	{
		if (_isConnecting)
		{
			Debug.Log("PUN OnConnectedToMaster()");
			PhotonNetwork.JoinRandomRoom();
		}
	}

	public override void OnJoinRandomFailed(short returnCode, string message)
	{
		PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = this._maxPlayers });
	}

    public static void SetNickName(string username) => PhotonNetwork.NickName = username;

    public override void OnDisconnected(DisconnectCause cause)
	{
		Debug.LogError("PUN : Disconnected");
		_isConnecting = false;
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("PUN : OnJoinedRoom()");

		if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
		{
			Debug.Log("First Player");
			PhotonNetwork.LoadLevel("GameScene");
		}
	}
}
