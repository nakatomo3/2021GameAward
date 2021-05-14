using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0108

public class DeathReason : MonoBehaviour {

	public Renderer renderer;

	private float timer;

	private void Start() {
		Destroy(gameObject, 1.5f);
		transform.Rotate(70, 0, 0);
		Invoke("Rewind", 1);
	}

    // Update is called once per frame
    void Update() {
		timer += Time.deltaTime;

		transform.localScale = Vector3.one * (timer * 0.5f + 0.5f);
		renderer.material.color = new Color(1, 1, 1, Mathf.Max(0, 1 - timer));
	}

	void Rewind() {
		Stage.instance.crt.enabled = true;
	}
}
