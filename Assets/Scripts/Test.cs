using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour {

	private void Start() {
		InputManager.AddKey(Keys.A, "A");
		InputManager.AddKey(Keys.B, "B");
		InputManager.AddKey(Keys.X, "X");
		InputManager.AddKey(Keys.Y, "Y");
		InputManager.AddAxis(Keys.RIGHT, "Right");
		InputManager.AddAxis(Keys.LEFT, "Left");
		InputManager.AddAxis(Keys.UP, "Up");
		InputManager.AddAxis(Keys.DOWN, "Down");

	}

	// Update is called once per frame
	void Update() {
		//if (InputManager.GetKeyDown(Keys.A)) {
		//	Debug.Log("A");
		//}
		//if (InputManager.GetKeyDown(Keys.B)) {
		//	Debug.Log("B");
		//}
		//if (InputManager.GetKeyDown(Keys.X)) {
		//	Debug.Log("X");
		//}
		//if (InputManager.GetKeyDown(Keys.Y)) {
		//	Debug.Log("Y");
		//}

		//if (InputManager.GetKeyDown(Keys.RIGHT)) {
		//	Debug.Log("Right");
		//}
		//if (InputManager.GetKey(Keys.LEFT)) {
		//	Debug.Log("Left");
		//}
	}
}
