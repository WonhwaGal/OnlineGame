using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;

public class PhotonGameManager : MonoBehaviourPunCallbacks
{
	static public PhotonGameManager Instance;

	private GameObject instance;
	private GameObject player;
	private float playerStartHP = 0;

	[SerializeField] private GameObject playerPrefab;

	public float PlayerStartHP 
	{
		get => playerStartHP;
		set
        {
			playerStartHP = value;
			if (player != null)
            {
				player.GetComponent<PlayerManager>().Health = value;
				Debug.Log($"Player HP is set to {value}");
			}
            else
            {
				Debug.Log($"Player is null so value is set to {value}");
			}
		}
	}

private void Awake()
    {
		Instance = this;
	}
    void Start()
	{
		Instance = this;

		if (!PhotonNetwork.IsConnected)
		{
			SceneManager.LoadScene("PlayFab");
			return;
		}

		if (playerPrefab == null)
		{
			Debug.Log("No Player Prefab attached");
		}
		else
		{
			if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
			{
				Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
				player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
				if (playerStartHP != 0)
                {
					player.GetComponent<PlayerManager>().Health = PlayerStartHP;
					player.GetComponent<PlayerManager>().MaxHealth = PlayerStartHP;
					Debug.Log("player Hp changed after spawn");
				}
			}
			else
			{
				Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
			}
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			QuitApplication();
		}
	}

	public override void OnJoinedRoom()
	{
		if (PlayerManager.LocalPlayerInstance == null)
		{
			Debug.LogFormat("On Joined Room - We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
			player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
			if (playerStartHP != 0)
			{
				player.GetComponent<PlayerManager>().Health = PlayerStartHP;
				player.GetComponent<PlayerManager>().MaxHealth = PlayerStartHP;
				Debug.Log("player Hp changed after spawn");
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player other)
	{
		Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting

		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
		}
	}

	public override void OnPlayerLeftRoom(Player other)
	{
		Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

		if (PhotonNetwork.IsMasterClient)
		{
			Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
		}
	}

    public override void OnLeftRoom() => SceneManager.LoadScene("PunBasics-Launcher");

    public void LeaveRoom() => PhotonNetwork.LeaveRoom();

    public void QuitApplication() => Application.Quit();
}