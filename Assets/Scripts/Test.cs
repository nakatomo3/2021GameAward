using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	private void Start() {
		InputManager.AddKey(Keys.SUBMIT, KeyCode.A);
	}

	// Update is called once per frame
	void Update() {
		if (InputManager.GetKey(Keys.SUBMIT)) {
			Debug.Log("hoge");
		}
	}
}
