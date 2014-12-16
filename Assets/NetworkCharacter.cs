using UnityEngine;
using System.Collections;

public class NetworkCharacter : Photon.MonoBehaviour {

    // Use this for initialization
    void Start() {
        CacheComponents();
    }

    private void CacheComponents() {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update() {
        if (!photonView.isMine) {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
            animator.SetFloat("AimAngle", Mathf.Lerp(animator.GetFloat("AimAngle"), realAimAngle, 0.1f));
        }
    }

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        // OnPhotonSerializeView is called before Start sometimes, giving an NRE on animator.
        // This call prevents that. It's unfortunate though because this is called unnecessarily several times per second for the rest of the game
        // when it is not needed.
        CacheComponents();

        if (stream.isWriting) {
            // Our player. Need to send our actual position to network.
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(animator.GetFloat("Speed"));    // Getting NRE here when joining with 2nd player
            stream.SendNext(animator.GetBool("Jumping"));
            stream.SendNext(animator.GetFloat("AimAngle"));
        } else {
            // Someone else's player. Need to receive their position (as of a few milliseconds ago) and update our version of them.
            realPosition = (Vector3)stream.ReceiveNext();
            realRotation = (Quaternion)stream.ReceiveNext();
            animator.SetFloat("Speed", (float)stream.ReceiveNext());
            animator.SetBool("Jumping", (bool)stream.ReceiveNext());
            realAimAngle = (float)stream.ReceiveNext();

            if (!gotFirstUpdate) {
                gotFirstUpdate = true;
                transform.position = realPosition;
                transform.rotation = realRotation;
                animator.SetFloat("AimAngle", realAimAngle);
            }
        }
    }

    private Vector3 realPosition = Vector3.zero;
    private Quaternion realRotation = Quaternion.identity;
    private float realAimAngle = 0f;
    private Animator animator;
    private bool gotFirstUpdate = false;
}
