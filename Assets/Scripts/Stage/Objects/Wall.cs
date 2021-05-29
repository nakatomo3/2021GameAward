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
        var rand = Random.Range(0, 4);
        if(Random.Range(0, 5) > 1) {
            rand = 0;
        }
        renderer.material.mainTextureOffset = new Vector2(rand * 0.25f, 0);
	}
}
