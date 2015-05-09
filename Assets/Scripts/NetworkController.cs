using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour
{

	public GameObject playerPrefab;
	public int maxActivePlayers;

	private int activePlayers;
	private const string GameName = "CS 352 Tron";
	private HostData[] hostData;
	private float btnX, btnY, btnW, btnH;
	private NetworkView _networkView;

	private GameObject[] spawns;
	private List<int> unclaimedSpawns = new List<int>();
	private GameObject mySpawnPoint;
	private bool gameStarted = false;
	private System.Random rand = new System.Random();

	void Start()
	{
		activePlayers = 0;
		btnX = (float)(Screen.width * 0.05);
		btnY = (float)(Screen.width * 0.05);
		btnW = (float)(Screen.width * 0.1);
		btnH = (float)(Screen.width * 0.1);

		//Get all the spawn points and generate a 'used' list
		spawns = GameObject.FindGameObjectsWithTag("Spawn Point");
		for (var i = 0; i < spawns.Length; i++)
			unclaimedSpawns.Add(i);

		//Load the network view
		_networkView = GetComponent<NetworkView>();
	}

	public void startServer()
	{
		//Debug.Log ("Starting Server");
		Network.InitializeServer(maxActivePlayers, 25001, !Network.HavePublicAddress());
		MasterServer.RegisterHost(GameName, "Tron Game", "This is in beta");
	}

	public void RefreshHostList()
	{
		MasterServer.RequestHostList(GameName);
	}

	public void Update()
	{
		if (Network.isServer)
		{
			var start = Input.GetKey("p");

			if (!gameStarted && start)
			{
				_networkView.RPC("StartGame", RPCMode.All);
				gameStarted = true;
			}
		}
	}

	[RPC]
	void StartGame()
	{
		SpawnPlayer();
	}

	/// <summary>
	/// Spawns a player at a random unused spawn point.
	/// </summary>
	// Could add overloaded version with specific spawn
	void SpawnPlayer()
	{
		//Add player at spawn point
		Network.Instantiate(playerPrefab, mySpawnPoint.transform.position, Quaternion.identity, 0);

		//Debug.Log (activePlayers);
	}

	void GetSpawnPoint()
	{
		//Get random spawn point
		var i = unclaimedSpawns[rand.Next(unclaimedSpawns.Count)];
		mySpawnPoint = spawns[i];

		//Tell everyone the player spawned in
		_networkView.RPC("ClaimedSpawn", RPCMode.AllBuffered, i);
	}

	[RPC]
	void ClaimedSpawn(int spawnPointIndex)
	{
		unclaimedSpawns.Remove(spawnPointIndex);
	}

	void OnServerInitialized()
	{
		//Debug.Log ("Server initialized");
		activePlayers++;
		Invoke("GetSpawnPoint", 0.1f);
	}

	void OnConnectedToServer()
	{
		if (activePlayers < maxActivePlayers)
		{
			activePlayers++;
			Invoke("GetSpawnPoint", 0.1f);
		}
	}

	void OnMasterServerEvent(MasterServerEvent mse)
	{
		switch (mse)
		{
			case MasterServerEvent.RegistrationSucceeded:
				//Debug.Log ("Registered Server");
				break;
			case MasterServerEvent.HostListReceived:
				hostData = MasterServer.PollHostList();
				break;
		}
	}

	void OnGUI()
	{
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(btnX, btnY, btnW, btnH), "Start Server"))
			{
				startServer();
			}
			if (GUI.Button(new Rect(btnX, (float)(btnY * 1.2 + btnH), btnW, btnH), "Refresh"))
			{
				RefreshHostList();
			}
			if (hostData != null)
			{
				for (int i = 0; i < hostData.Length; i++)
				{
					if (GUI.Button(new Rect((float)(btnX * 1.5 + btnW), (float)(btnY * 1.2 + (btnH * i)), (float)(btnW * 3), (float)(btnH * 0.5)), hostData[i].gameName))
					{
						Debug.Log(Network.Connect(hostData[i]));
					}
				}
			}
		}
	}
}
