using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0108

public class Wall : MonoBehaviour {

	public Renderer renderer;
	public Material[] materials;

	// Start is called before the first frame update
	void Start() {
		renderer.material = materials[Stage.instance.visualMode];
	}
}
