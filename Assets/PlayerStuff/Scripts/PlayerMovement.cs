using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
    public float Speed = 10f;
    public Vector3 Direction = Vector3.zero;

	// Use this for initialization
	void Start () {
        charController = GetComponent<CharacterController>();	
	}
	
	// Update is called once per frame
	void Update () {
        Direction = transform.rotation * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")).normalized;
	}

    // Called once per physics loop
    private void FixedUpdate() {
        charController.SimpleMove(Direction * Speed);
    }

    private CharacterController charController;
}
