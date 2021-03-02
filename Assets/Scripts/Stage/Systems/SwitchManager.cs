using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchManager : MonoBehaviour {

	public static SwitchManager instance;

	public bool[] channel = new bool[16];

	private void Awake() {
		instance = this;
	}

	// Start is called before the first frame update
	void Start() {
		channel = new bool[16];
	}
}
