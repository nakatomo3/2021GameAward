using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

	new private GameObject camera;

	private void Start() {
		camera = Stage.instance.camera;
	}

	// Update is called once per frame
	void Update() {
		camera.transform.position = gameObject.transform.position + new Vector3(0, 10, -5);
		camera.transform.localEulerAngles = new Vector3(70, 0, 0);
	}
}
