using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    public GameObject StandbyCamera;
    public bool OfflineMode = false;

    private void Start() {
        spawnSpots = GameObject.FindObjectsOfType<SpawnSpot>();
        Connect();
    }

    private void Connect() {
        if (OfflineMode) {
            PhotonNetwork.offlineMode = true;
            OnJoinedLobby();
        } else {
            PhotonNetwork.ConnectUsingSettings("MultiFPS v001");
        }
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

        StandbyCamera.SetActive(false);
        //((MonoBehaviour)myPlayer.GetComponent("FPSInputController")).enabled = true;
        ((MonoBehaviour)myPlayer.GetComponent("MouseLook")).enabled = true;
        ((MonoBehaviour)myPlayer.GetComponent("PlayerMovement")).enabled = true;
        ((MonoBehaviour)myPlayer.GetComponent("PlayerShooting")).enabled = true;
        myPlayer.transform.FindChild("Main Camera").gameObject.SetActive(true);
    }

    private SpawnSpot[] spawnSpots;
}
