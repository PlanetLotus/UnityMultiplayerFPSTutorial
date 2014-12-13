using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    public float Speed = 10f;
    public float JumpSpeed = 6f;
    public Vector3 Direction = Vector3.zero;

    // Use this for initialization
    void Start() {
        charController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update() {
        Direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (Direction.magnitude > 1f)
            Direction = Direction.normalized;

        animator.SetFloat("Speed", Direction.magnitude);

        // Handle jumping
        if (charController.isGrounded && Input.GetButton("Jump")) {
            verticalVelocity = JumpSpeed;
        }
    }

    // Called once per physics loop
    private void FixedUpdate() {
        Vector3 distance = Direction * Speed * Time.deltaTime;

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

    private CharacterController charController;
    private Animator animator;
    private float verticalVelocity = 0f;
}
