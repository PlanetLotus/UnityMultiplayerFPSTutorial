using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update() {
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.isWriting) {
            // Our player. Need to send our actual position to network.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("Speed"));
            stream.SendNext(animator.GetBool("Jumping"));
        } else {
            // Someone else's player. Need to receive their position (as of a few milliseconds ago) and update our version of them.
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
            animator.SetBool("Jumping", (bool)stream.ReceiveNext());

            if (!gotFirstUpdate) {
                gotFirstUpdate = true;
                transform.position = realPosition;
                transform.rotation = realRotation;
            }
        }
    }

    private Vector3 realPosition = Vector3.zero;
    private Quaternion realRotation = Quaternion.identity;
    private Animator animator;
    private bool gotFirstUpdate = false;
}
