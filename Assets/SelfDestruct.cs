using UnityEngine;
using System.Collections;

public class SelfDestruct : MonoBehaviour {
    public float SelfDestructTime = 1f;

    private void Update() {
        SelfDestructTime -= Time.deltaTime;

        if (SelfDestructTime <= 0) {
            PhotonView photonView = GetComponent<PhotonView>();
            if (photonView != null && photonView.instantiationId != 0) {
                PhotonNetwork.Destroy(gameObject);
            } else {
                Destroy(gameObject);
            }
        }
    }
}
