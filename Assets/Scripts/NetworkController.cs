using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NetworkController : MonoBehaviour {

	public GameObject playerPrefab;
	public Transform spawnPoint1;
	public Transform spawnPoint2;
	public int maxNumberOfPlayers;

	private int currentNumberofPlayers;
	private string _gameName = "CS 352 Tron";
	private bool refreshing = false;
	private HostData[] hostData;
	private float btnX, btnY, btnW, btnH;

	void Start() {
		currentNumberofPlayers = 0;
		btnX = (float)(Screen.width * 0.05);
		btnY = (float)(Screen.width * 0.05);
		btnW = (float)(Screen.width * 0.1);
		btnH = (float)(Screen.width * 0.1);
	}

	public void startServer() {
		//Debug.Log ("Starting Server");
		Network.InitializeServer (maxNumberOfPlayers, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(_gameName, "Tron Game", "This is in beta");
	}

	public void refreshHostList() {
		MasterServer.RequestHostList (_gameName);
		refreshing = true;
	}

	public void Update() {
		if (refreshing) {
			if(MasterServer.PollHostList ().Length > 0) {
				refreshing = false;
				hostData = MasterServer.PollHostList();
			}
		}
	}

	void spawnPlayer(int spawnPoint) {
		if (spawnPoint == 1) {
			Network.Instantiate (playerPrefab, spawnPoint1.position, Quaternion.identity, 0);
		} else if (spawnPoint == 2) {
			Network.Instantiate (playerPrefab, spawnPoint2.position, Quaternion.identity, 0);
		}
		//Debug.Log (currentNumberofPlayers);
	}

	void OnServerInitialized() {
		//Debug.Log ("Server initialized");
		currentNumberofPlayers++;
		spawnPlayer(1);

	}

	void OnConnectedToServer() {
		if (currentNumberofPlayers < maxNumberOfPlayers) {
			currentNumberofPlayers++;
			spawnPlayer (2);

		}
	}

	void OnMasterServerEvent(MasterServerEvent mse) {
		if (mse == MasterServerEvent.RegistrationSucceeded) {
			//Debug.Log ("Registered Server");
		}
	}

	void OnGUI() {
		if (!Network.isClient && !Network.isServer) {
			if (GUI.Button (new Rect (btnX, btnY, btnW, btnH), "Start Server")) {
				startServer ();
			}
			if (GUI.Button (new Rect (btnX, (float)(btnY * 1.2 + btnH), btnW, btnH), "Refresh")) {
				refreshHostList ();
			}
			if (hostData != null) {
				for (int i = 0; i < hostData.Length; i++) {
					if(GUI.Button (new Rect ((float)(btnX * 1.5 + btnW), (float)(btnY * 1.2 + (btnH * i)), (float)(btnW * 3), (float)(btnH * 0.5)), hostData [i].gameName)) {
						Network.Connect(hostData[i]);
					}
				}
			}
		}
	}
}
