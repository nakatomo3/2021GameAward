using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

	new private GameObject camera;

	private Vector3 deltaPosition;
	private float maxRange = 5;

	private void Start() {
		camera = Stage.instance.camera;
	}

	// Update is called once per frame
	void Update() {
		deltaPosition = Vector3.Lerp(deltaPosition, Vector3.zero, 0.02f);

		var moveVector = new Vector3(InputManager.GetAxis(Keys.R_HORIZONTAL), 0, InputManager.GetAxis(Keys.R_VERTICAL));
		deltaPosition += moveVector.normalized * 10 * Time.deltaTime;
		
		if (Mathf.Abs(deltaPosition.x) > maxRange) {
			deltaPosition = new Vector3(Mathf.Sign(deltaPosition.x) * maxRange, 0, deltaPosition.z);
		}
		if(Mathf.Abs(deltaPosition.z) > maxRange) {
			deltaPosition = new Vector3(deltaPosition.x, 0, Mathf.Sign(deltaPosition.z) * maxRange);
		}
		if(moveVector.magnitude < 0.1f) {
			deltaPosition = Vector3.Lerp(deltaPosition, Vector3.zero, 0);
		}

		camera.transform.position = Vector3.Lerp(camera.transform.position, new Vector3(gameObject.transform.position.x + 0.5f, 10, gameObject.transform.position.z + -4.5f) + deltaPosition, 0.1f);
		camera.transform.localEulerAngles = new Vector3(70, 0, 0);
	}
}
