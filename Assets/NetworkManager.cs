using UnityEngine;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
    public GameObject StandbyCamera;

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
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            foreach (string message in chatMessages) {
                GUILayout.Label(message);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
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
        SpawnMyPlayer();
    }

    private void SpawnMyPlayer() {
        AddChatMessage("Spawning player: " + PhotonNetwork.player.name);

        SpawnSpot mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
        GameObject myPlayer = PhotonNetwork.Instantiate("PlayerController", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);

        StandbyCamera.SetActive(false);
        ((MonoBehaviour)myPlayer.GetComponent("MouseLook")).enabled = true;
        ((MonoBehaviour)myPlayer.GetComponent("PlayerMovement")).enabled = true;
        ((MonoBehaviour)myPlayer.GetComponent("PlayerShooting")).enabled = true;
        myPlayer.transform.FindChild("Main Camera").gameObject.SetActive(true);
    }

    private SpawnSpot[] spawnSpots;
    private bool connecting = false;
    private List<string> chatMessages;
    private const int maxChatMessages = 5;
}
