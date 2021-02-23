using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class SystemSupporter {
	public static void ExitGame() {
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
      UnityEngine.Application.Quit();
#endif
	}

	public static void PlaySupport() {
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Return)){
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}

		if (Input.GetKeyDown(KeyCode.LeftShift)) {
			Time.timeScale = 3;
		}
		if (Input.GetKeyUp(KeyCode.LeftShift)) {
			Time.timeScale = 1;
		}
#endif
	}

}
