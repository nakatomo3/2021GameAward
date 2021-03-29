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
		deltaPosition = Vector3.Lerp(deltaPosition, Vector3.zero, 0.03f);

		deltaPosition += new Vector3(InputManager.GetAxis(Keys.R_HORIZONTAL), 0, InputManager.GetAxis(Keys.R_VERTICAL)).normalized * 10 * Time.deltaTime;
		
		if (Mathf.Abs(deltaPosition.x) > maxRange) {
			deltaPosition = new Vector3(Mathf.Sign(deltaPosition.x) * maxRange, 0, deltaPosition.z);
		}
		if(Mathf.Abs(deltaPosition.z) > maxRange) {
			deltaPosition = new Vector3(deltaPosition.x, 0, Mathf.Sign(deltaPosition.z) * maxRange);
		}

		camera.transform.position = gameObject.transform.position + new Vector3(0, 10, -5) + deltaPosition;
		camera.transform.localEulerAngles = new Vector3(70, 0, 0);
	}
}
