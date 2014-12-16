using UnityEngine;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
    public GameObject StandbyCamera;
    public float RespawnTimer = 0f;

    public void AddChatMessage(string message) {
        GetComponent<PhotonView>().RPC("AddChatMessageRPC", PhotonTargets.All, message);
    }

    [RPC]
    private void AddChatMessageRPC(string message) {
        while (chatMessages.Count >= maxChatMessages) {
            chatMessages.RemoveAt(0);
        }
        chatMessages.Add(message);
    }

    private void Start() {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        PhotonNetwork.player.name = PlayerPrefs.GetString("Username", "TestPlayer");
        chatMessages = new List<string>();
    }

    private void Update() {
        if (RespawnTimer > 0) {
            RespawnTimer -= Time.deltaTime;

            if (RespawnTimer <= 0) {
                SpawnMyPlayer(teamId);
            }
        }
    }

    private void OnDestroy() {
        PlayerPrefs.SetString("Username", PhotonNetwork.player.name);
    }

    private void Connect() {
        PhotonNetwork.ConnectUsingSettings("MultiFPS v001");
    }

    private void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());

        if (!PhotonNetwork.connected && !connecting) {
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Username: ");
            PhotonNetwork.player.name = GUILayout.TextField(PhotonNetwork.player.name);
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Single Player")) {
                connecting = true;
                PhotonNetwork.offlineMode = true;
                OnJoinedLobby();
            }

            if (GUILayout.Button("Multi Player")) {
                connecting = true;
                Connect();
            }

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        if (PhotonNetwork.connected && !connecting) {
            if (hasPickedTeam) {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                foreach (string message in chatMessages) {
                    GUILayout.Label(message);
                }

                GUILayout.EndVertical();
                GUILayout.EndArea();
            } else {
                GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Red Team")) {
                    SpawnMyPlayer(1);
                }

                if (GUILayout.Button("Green Team")) {
                    SpawnMyPlayer(2);
                }

                if (GUILayout.Button("Random")) {
                    SpawnMyPlayer(Random.Range(1, 3));
                }

                if (GUILayout.Button("Renegade")) {
                    SpawnMyPlayer(0);
                }

                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }
    }

    private void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
        PhotonNetwork.JoinRandomRoom();
    }

    private void OnPhotonRandomJoinFailed() {
        Debug.Log("OnPhotonRandomJoinFailed");
        PhotonNetwork.CreateRoom(null);
    }

    private void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom");
        connecting = false;
        //SpawnMyPlayer();
    }

    private void SpawnMyPlayer(int teamId) {
        this.teamId = teamId;
        hasPickedTeam = true;
        AddChatMessage("Spawning player: " + PhotonNetwork.player.name);

        SpawnSpot mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
        GameObject myPlayer = PhotonNetwork.Instantiate("PlayerController", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);

        StandbyCamera.SetActive(false);
        ((MonoBehaviour)myPlayer.GetComponent("MouseLook")).enabled = true;
        ((MonoBehaviour)myPlayer.GetComponent("PlayerMovement")).enabled = true;
        ((MonoBehaviour)myPlayer.GetComponent("PlayerShooting")).enabled = true;
        myPlayer.transform.FindChild("Main Camera").gameObject.SetActive(true);
        myPlayer.GetComponent<PhotonView>().RPC("SetTeamId", PhotonTargets.AllBuffered, teamId);
    }

    private SpawnSpot[] spawnSpots;
    private bool connecting = false;
    private List<string> chatMessages;
    private const int maxChatMessages = 5;
    private bool hasPickedTeam = false;
    private int teamId = 0;
}
