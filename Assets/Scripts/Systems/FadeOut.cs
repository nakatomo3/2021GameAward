using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class FadeOut : MonoBehaviour {

	public string nextStagePath;
	public float timeScale = 1;

	private float timer = 0;
	private Image image;

	private void Awake() {
		image = gameObject.GetComponent<Image>();
        AudioManeger.instance.Play("SceneChange");
    }

	void Update() {
		timer += Time.deltaTime;

		image.color = new Color(0, 0, 0, timer * timeScale);
		if (timer >= 1 / timeScale && nextStagePath != "") {
            SceneManager.LoadScene(nextStagePath);
		}
	}
}
