using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Keys {
	A,
	B,
	X,
	Y,
	UP,
	DOWN,
	LEFT,
	RIGHT,
	START,
	SELECT,
	L,
	R,
	TRIGGER,
	R_HORIZONTAL,
	R_VERTICAL,
	LAST
}

public class InputManager : MonoBehaviour {

	public static bool isInit { get; private set; }

	private static Dictionary<Keys, List<KeyCode>> keys;
	private static Dictionary<Keys, List<string>> keyStrings;
	private static Dictionary<Keys, List<string>> axesString;

	private static Dictionary<string, float> axes;
	private static Dictionary<string, float> beforeAxes;

	private static float threshold = 0.5f; //臒l

	/// <summary>
	/// �������B�f�[�^�̓ǂݍ��݂Ȃ�
	/// </summary>
	public static void Init() {
		if(isInit == true) {
			return;
		}
		keys = new Dictionary<Keys, List<KeyCode>>();
		keyStrings = new Dictionary<Keys, List<string>>();
		axesString = new Dictionary<Keys, List<string>>();
		axes = new Dictionary<string, float>();
		beforeAxes = new Dictionary<string, float>();
		for(int i = 0; i < (int)Keys.LAST; i++) {
			keys[(Keys)i] = new List<KeyCode>();
			keyStrings[(Keys)i] = new List<string>();
			axesString[(Keys)i] = new List<string>();
		}
		isInit = true;
	}

	private void ReadSetting() {

	}

	private void Update() {
		foreach(var axis in axes) {
			beforeAxes[axis.Key] = axis.Value;
		}
		foreach (var axis in beforeAxes) {
			axes[axis.Key] = Input.GetAxis(axis.Key);
		}
	}

	/// <summary>
	/// �L�[��ݒ肵�܂�
	/// </summary>
	/// <param name="key">�ݒ肳���L�[</param>
	/// <param name="code">�ݒ肷��L�[�R�[�h</param>
	public static void SetKey(Keys key, KeyCode code) {
		AddKey(key, code);
	}

	public static void SetKey(Keys key, string code) {
		AddKey(key, code);
	}


	/// <summary>
	/// �L�[��ǉ����܂�
	/// </summary>
	/// <param name="key">�ݒ肳���L�[</param>
	/// <param name="code">�ݒ肳���L�[�R�[�h</param>
	public static void AddKey(Keys key, KeyCode code) {
		Debug.Log("�L�[�ǉ�:(" + key + ":" + code + ")");
		keys[key].Add(code);
	}

	public static void AddKey(Keys key, string code) {
		Debug.Log("�L�[�ǉ�:(" + key + ":" + code + ")");
		keyStrings[key].Add(code);
	}

	public static void AddAxis(Keys key, string axis) {
		Debug.Log("���ǉ�:(" + key + ":" + axis + ")");
		axes.Add(axis, 0);
		beforeAxes.Add(axis, 0);
		axesString[key].Add(axis);
	}

	/// <summary>
	/// GetKey�Ɠ����ł�
	/// </summary>
	/// <param name="key">�Ώۂ̃L�[</param>
	/// <returns></returns>
	public static bool GetKey(Keys key) {
		//AutoDebugger�ł��Ăяo����

		if(keys.ContainsKey(key) == false && keyStrings.ContainsKey(key)) {
			Debug.LogError("�L�[���o�^����Ă��܂���:" + key);
			return false;
		}
		for(int i = 0; i < keys[key].Count; i++) {
			if (Input.GetKey(keys[key][i])) {
				return true;
			}
		}

		for (int i = 0; i < keyStrings[key].Count; i++) {
			if (Input.GetButton(keyStrings[key][i])) {
				return true;
			}
		}

		foreach (var axis in axesString[key]) {
			if(axes[axis] > threshold) {
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
		if (keys.ContainsKey(key) == false && keyStrings.ContainsKey(key)) {
			Debug.LogError("�L�[���o�^����Ă��܂���:" + key);
			return false;
		}
		for (int i = 0; i < keys[key].Count; i++) {
			if (Input.GetKeyDown(keys[key][i])) {
				return true;
			}
		}
		for (int i = 0; i < keyStrings[key].Count; i++) {
			if (Input.GetButtonDown(keyStrings[key][i])) {
				return true;
			}
		}
		foreach (var axis in axesString[key]) {
			if (axes[axis] > threshold && beforeAxes[axis] < threshold) {
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
		if (keys.ContainsKey(key) == false && keyStrings.ContainsKey(key)) {
			Debug.LogError("�L�[���o�^����Ă��܂���:" + key);
			return false;
		}
		for (int i = 0; i < keys[key].Count; i++) {
			if (Input.GetKeyUp(keys[key][i])) {
				return true;
			}
		}
		for (int i = 0; i < keyStrings[key].Count; i++) {
			if (Input.GetButtonUp(keyStrings[key][i])) {
				return true;
			}
		}
		foreach (var axis in axesString[key]) {
			if (axes[axis] < threshold && beforeAxes[axis] > threshold) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// key�̒������Βl����ԑ傫�����
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public static float GetAxis(Keys key) {
		float value = 0;
		foreach(var axis in axesString[key]) {
			if(Mathf.Abs(axes[axis]) > threshold && Mathf.Abs(axes[axis]) > Mathf.Abs(value)) {
				value = axes[axis];
			}
		}
		return value;
	}

	private static List<KeyCode> ConvertKeyCode(Keys key) {
		return keys[key];
	}
}
