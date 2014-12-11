using UnityEngine;
using System.Collections;

public class NetworkManager : MonoBehaviour {
    public Camera standbyCamera;

    private void Start() {
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
        PhotonNetwork.Instantiate("PlayerController", new Vector3(50, 2, 0), Quaternion.identity, 0);
        standbyCamera.enabled = false;
    }
}
