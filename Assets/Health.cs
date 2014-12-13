using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
    public float HitPoints = 10f;

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

    private void Die() {
        if (GetComponent<PhotonView>().instantiationId == 0) {
            Destroy(gameObject);
        } else {
            if (PhotonNetwork.isMasterClient) {
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private float currentHitPoints;
}
