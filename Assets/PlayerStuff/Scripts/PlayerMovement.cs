using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    // Use this for initialization
    void Start() {
        animator = GetComponent<Animator>();
        netChar = GetComponent<NetworkCharacter>();
    }

    // Update is called once per frame
    void Update() {
        netChar.Direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (netChar.Direction.magnitude > 1f)
            netChar.Direction = netChar.Direction.normalized;

        animator.SetFloat("Speed", netChar.Direction.magnitude);

        // Handle jumping
        if (Input.GetButton("Jump")) {
            netChar.IsJumping = true;
        } else {
            netChar.IsJumping = false;
        }

        AdjustAimAngle();
    }

    private void AdjustAimAngle() {
        Camera camera = this.GetComponentInChildren<Camera>();

        float aimAngle = 0;

        if (camera.transform.rotation.eulerAngles.x <= 90f) {
            // We're looking down
            aimAngle = -camera.transform.rotation.eulerAngles.x;
        } else {
            aimAngle = 360 - camera.transform.rotation.eulerAngles.x;
        }

        animator.SetFloat("AimAngle", aimAngle);
    }

    private NetworkCharacter netChar;
    private Animator animator;
}
