using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    public float HitPoints = 100f;

    [RPC]
    public void TakeDamage(float amount) {
        currentHitPoints -= amount;

        if (currentHitPoints <= 0) {
            Die();
        }
    }

    private void Start() {
        currentHitPoints = HitPoints;
    }

    private void OnGUI() {
        if (GetComponent<PhotonView>().isMine && gameObject.tag == "Player") {
            if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 40), "Suicide")) {
                Die();
            }
        }
    }

    private void Die() {
        PhotonView photonView = GetComponent<PhotonView>();

        if (photonView.instantiationId == 0) {
            Destroy(gameObject);
        } else {
            if (photonView.isMine) {
                if (gameObject.tag == "Player") {
                    // NRE:
                    //GameObject.Find("StandbyCamera").SetActive(true);
                    //GameObject.FindObjectOfType<NetworkManager>().RespawnTimer = 3f;
                    
                    // Hack fix:
                    NetworkManager nm = GameObject.FindObjectOfType<NetworkManager>();
                    nm.StandbyCamera.SetActive(true);
                    nm.RespawnTimer = 3f;
                } else if (gameObject.tag == "Bot") {
                    Debug.LogError("No bot respawn code exists.");
                }

                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private float currentHitPoints;
}
