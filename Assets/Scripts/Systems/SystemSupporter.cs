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

	/// <summary>
	/// デバッグのサポート、エンターでリロード、左シフトで3倍速
	/// </summary>
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

	public static void DebugInitInput() {
		InputManager.AddKey(Keys.A, "A");
		InputManager.AddKey(Keys.B, "B");
		InputManager.AddKey(Keys.X, "X");
		InputManager.AddKey(Keys.Y, "Y");
		InputManager.AddAxis(Keys.RIGHT, "Right");
		InputManager.AddAxis(Keys.LEFT, "Left");
		InputManager.AddAxis(Keys.UP, "Up");
		InputManager.AddAxis(Keys.DOWN, "Down");
		InputManager.AddKey(Keys.START, "Start");
		InputManager.AddKey(Keys.SELECT, "Select");
		InputManager.AddKey(Keys.L, "L");
		InputManager.AddKey(Keys.R, "R");
		InputManager.AddAxis(Keys.TRIGGER, "LTrigger");
		InputManager.AddAxis(Keys.TRIGGER, "RTrigger");
		InputManager.AddAxis(Keys.R_HORIZONTAL, "RHorizontal");
		InputManager.AddAxis(Keys.R_VERTICAL, "RVertical");

		InputManager.AddKey(Keys.RIGHT, KeyCode.D);
		InputManager.AddKey(Keys.LEFT, KeyCode.A);
		InputManager.AddKey(Keys.UP, KeyCode.W);
		InputManager.AddKey(Keys.DOWN, KeyCode.S);

	}

	public static bool IsUnityEditor() {
		#if UNITY_EDITOR
			return true;
		#else
			return false;
		#endif
	}
}
