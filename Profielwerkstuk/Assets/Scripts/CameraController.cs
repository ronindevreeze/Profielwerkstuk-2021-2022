using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float speed;
    public float sensitivity;

    private Vector2 orbit;
    private Vector3 movement;
    
    void Update() {
        // Input
        Vector2 rotationInput = new Vector2(
			-Input.GetAxis("Mouse Y") * sensitivity,
			Input.GetAxis("Mouse X") * sensitivity
		);
        Vector2 movementInput = new Vector2(
			Input.GetAxis("Vertical") / 4f,
			Input.GetAxis("Horizontal") / 4f
		);

        // Orbit
		orbit += rotationInput;
        orbit.x = Mathf.Clamp(orbit.x, 0f, 90f);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(orbit), Time.deltaTime * speed);

        // Movement
        movement += transform.forward * movementInput.x;
        movement += transform.right * movementInput.y;
        movement.y = 0;
        transform.position = Vector3.Lerp(transform.position, movement, Time.deltaTime * speed);
    }
}
