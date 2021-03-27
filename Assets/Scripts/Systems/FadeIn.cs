using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FadeIn : MonoBehaviour {

	public float timeScale = 1;

	private float timer = 0;
	private Image image;

	private void Awake() {
		image = gameObject.GetComponent<Image>();
		image.color = new Color(0, 0, 0, 1);
	}

	void Update() {
		timer += Time.deltaTime;

		image.color = new Color(0, 0, 0, 1 - timeScale * timer);
		if (1 - timeScale * timer < 0) {
			Destroy(this);
		}
	}
}
