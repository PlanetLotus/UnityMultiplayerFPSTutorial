using UnityEngine;
using System.Collections;

/// <summary>
/// Responsible for actually moving a character.
/// For local characters, we read things like "direction" and "isJumping" and then affect the character controller.
/// For remote characters, we skip that and simply update the raw transform position based on info we received over the network.
/// </summary>
public class NetworkCharacter : Photon.MonoBehaviour {
    // Only local character will use these
    public float Speed = 10f;
    public float JumpSpeed = 6f;

    [System.NonSerialized]
    public Vector3 Direction = Vector3.zero;
    [System.NonSerialized]
    public bool IsJumping = false;
    private float verticalVelocity = 0f;

    // Use this for initialization
    void Start() {
        CacheComponents();
    }

    private void CacheComponents() {
        if (animator == null)
            animator = GetComponent<Animator>();

        if (charController == null)
            charController = GetComponent<CharacterController>();
    }

    private void FixedUpdate() {
        if (photonView.isMine) {
            DoLocalMovement();
        } else {
            transform.position = Vector3.Lerp(transform.position, realPosition, 0.1f);
            transform.rotation = Quaternion.Lerp(transform.rotation, realRotation, 0.1f);
            animator.SetFloat("AimAngle", Mathf.Lerp(animator.GetFloat("AimAngle"), realAimAngle, 0.1f));
        }
    }

    private void DoLocalMovement() {
        Vector3 distance = Direction * Speed * Time.deltaTime;

        if (IsJumping) {
            IsJumping = false;
            if (charController.isGrounded) {
                verticalVelocity = JumpSpeed;
            }
        }

        if (charController.isGrounded && verticalVelocity < 0) {
            animator.SetBool("Jumping", false);
            verticalVelocity = Physics.gravity.y * Time.deltaTime;
        } else {
            if (Mathf.Abs(verticalVelocity) > JumpSpeed * 0.75f) {
                animator.SetBool("Jumping", true);
            }

            verticalVelocity += Physics.gravity.y * Time.deltaTime;
        }

        distance.y = verticalVelocity * Time.deltaTime;

        charController.Move(distance);
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
    private CharacterController charController;
}
