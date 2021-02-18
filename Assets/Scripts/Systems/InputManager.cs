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
	/// �������B�f�[�^�̓ǂݍ��݂Ȃ�
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
	/// �L�[��ݒ肵�܂�
	/// </summary>
	/// <param name="key">�ݒ肳���L�[</param>
	/// <param name="code">�ݒ肷��L�[�R�[�h</param>
	public static void SetKey(Keys key, KeyCode code) {
		AddKey(key, code);
	}

	/// <summary>
	/// �L�[��ǉ����܂�
	/// </summary>
	/// <param name="key">�ݒ肳���L�[</param>
	/// <param name="code">�ݒ肳���L�[�R�[�h</param>
	public static void AddKey(Keys key, KeyCode code) {
		keys[key].Add(code);
		Debug.Log("�ǉ�");
	}



	/// <summary>
	/// GetKey�Ɠ����ł�
	/// </summary>
	/// <param name="key">�Ώۂ̃L�[</param>
	/// <returns></returns>
	public static bool GetKey(Keys key) {
		//AutoDebugger�ł��Ăяo����

		if(keys.ContainsKey(key) == false) {
			Debug.LogError("�L�[���o�^����Ă��܂���");
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
	/// GetKeyDown�Ɠ����ł�
	/// </summary>
	/// <param name="key">�Ώۂ̃L�[</param>
	/// <returns></returns>
	public static bool GetKeyDown(Keys key) {
		if (keys.ContainsKey(key) == false) {
			Debug.LogError("�L�[���o�^����Ă��܂���");
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
	/// GetKeyUp�Ɠ����ł�
	/// </summary>
	/// <param name="key">�Ώۂ̃L�[</param>
	/// <returns></returns>
	public static bool GetKeyUp(Keys key) {
		if (keys.ContainsKey(key) == false) {
			Debug.LogError("�L�[���o�^����Ă��܂���");
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
