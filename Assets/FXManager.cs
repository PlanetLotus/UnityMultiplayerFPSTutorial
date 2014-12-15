using UnityEngine;
using System.Collections;

public class FXManager : MonoBehaviour {
    public GameObject sniperBulletFXPrefab;

    [RPC]
    private void SniperBulletFX(Vector3 start, Vector3 end) {
        GameObject sniperFX = (GameObject)Instantiate(sniperBulletFXPrefab, start, Quaternion.LookRotation(end - start));

        LineRenderer lr = sniperFX.transform.Find("LineFX").GetComponent<LineRenderer>();
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
