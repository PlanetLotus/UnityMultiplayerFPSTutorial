using UnityEngine;
using System.Collections;

public class FXManager : MonoBehaviour {
    public AudioClip sniperBulletFXAudio;

    [RPC]
    private void SniperBulletFX(Vector3 start, Vector3 end) {
        AudioSource.PlayClipAtPoint(sniperBulletFXAudio, start);
    }
}
