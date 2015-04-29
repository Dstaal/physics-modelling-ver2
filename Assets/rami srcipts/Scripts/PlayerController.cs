using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public float MovementSpeed = 2f;
	public float RotationSpeed = 5f;

	// Update is called once per frame
	void Update () {
	
		Vector3 direction = Vector3.zero;
		Vector3 speed = Vector3.zero;

		if (Input.GetKey(KeyCode.W)) {
			direction = this.transform.forward;
		}
		if  (Input.GetKey(KeyCode.S)) {
			direction = -this.transform.forward;
		}

		if (Input.GetKey(KeyCode.A)) {
			this.transform.Rotate(this.transform.up, -RotationSpeed);
		}

		if (Input.GetKey(KeyCode.D)) {
			this.transform.Rotate(this.transform.up, RotationSpeed);
		}

		if (direction.sqrMagnitude > 0f) {
			speed = direction.normalized * Time.deltaTime * MovementSpeed;
			this.transform.position += speed;
		}

	}
}
