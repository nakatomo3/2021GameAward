using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Keys {
	SUBMIT,
	CANCEL,
	UP,
	DOWN,
	LEFT,
	RIGHT,
	LAST
}

public class InputManager : MonoBehaviour {

	public static bool isInit { get; private set; }

	private static Dictionary<Keys, List<KeyCode>> keys;

	/// <summary>
	/// 初期化。データの読み込みなど
	/// </summary>
	public static void Init() {
		if(isInit == true) {
			return;
		}
		keys = new Dictionary<Keys, List<KeyCode>>();
		for(int i = 0; i < (int)Keys.LAST; i++) {
			keys[(Keys)i] = new List<KeyCode>();
		}
		isInit = true;
	}

	private void ReadSetting() {

	}

	/// <summary>
	/// キーを設定します
	/// </summary>
	/// <param name="key">設定されるキー</param>
	/// <param name="code">設定するキーコード</param>
	public static void SetKey(Keys key, KeyCode code) {
		AddKey(key, code);
	}

	/// <summary>
	/// キーを追加します
	/// </summary>
	/// <param name="key">設定されるキー</param>
	/// <param name="code">設定されるキーコード</param>
	public static void AddKey(Keys key, KeyCode code) {
		keys[key].Add(code);
		Debug.Log("追加");
	}



	/// <summary>
	/// GetKeyと同じです
	/// </summary>
	/// <param name="key">対象のキー</param>
	/// <returns></returns>
	public static bool GetKey(Keys key) {
		//AutoDebuggerでも呼び出せる

		if(keys.ContainsKey(key) == false) {
			Debug.LogError("キーが登録されていません");
			return false;
		}
		for(int i = 0; i < keys[key].Count; i++) {
			if (Input.GetKey(keys[key][i])) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// GetKeyDownと同じです
	/// </summary>
	/// <param name="key">対象のキー</param>
	/// <returns></returns>
	public static bool GetKeyDown(Keys key) {
		if (keys.ContainsKey(key) == false) {
			Debug.LogError("キーが登録されていません");
			return false;
		}
		for (int i = 0; i < keys[key].Count; i++) {
			if (Input.GetKeyDown(keys[key][i])) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// GetKeyUpと同じです
	/// </summary>
	/// <param name="key">対象のキー</param>
	/// <returns></returns>
	public static bool GetKeyUp(Keys key) {
		if (keys.ContainsKey(key) == false) {
			Debug.LogError("キーが登録されていません");
			return false;
		}
		for (int i = 0; i < keys[key].Count; i++) {
			if (Input.GetKeyUp(keys[key][i])) {
				return true;
			}
		}
		return false;
	}

	private static List<KeyCode> ConvertKeyCode(Keys key) {
		return keys[key];
	}
}
