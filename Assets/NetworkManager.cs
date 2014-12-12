using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    public Camera standbyCamera;

    private void Start() {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        Connect();
    }

    private void Connect() {
        PhotonNetwork.offlineMode = true;
        PhotonNetwork.ConnectUsingSettings("MultiFPS v001");
    }

    private void OnGUI() {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
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
        SpawnMyPlayer();
    }

    private void SpawnMyPlayer() {
        SpawnSpot mySpawnSpot = spawnSpots[Random.Range(0, spawnSpots.Length)];
        GameObject myPlayer = PhotonNetwork.Instantiate("PlayerController", mySpawnSpot.transform.position, mySpawnSpot.transform.rotation, 0);
        standbyCamera.enabled = false;
        ((MonoBehaviour)myPlayer.GetComponent("FPSInputController")).enabled = true;
        ((MonoBehaviour)myPlayer.GetComponent("MouseLook")).enabled = true;
        myPlayer.transform.FindChild("Main Camera").gameObject.SetActive(true);
    }

    private SpawnSpot[] spawnSpots;
}
