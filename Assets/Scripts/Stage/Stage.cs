using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 初期化とロード、ステージ生成のクラス
/// </summary>
public class Stage : MonoBehaviour {

	public GameObject player;
	public string stagePath;

	public List<List<char>> stageData { get; private set; }

	void Start() {
		if (InputManager.isInit == false) {
			InputManager.Init();
		}

		if (player != null) {
			Instantiate(player);
		} else {
			Debug.LogError("プレイヤーが設定されていません");
			SystemSupporter.ExitGame();
		}

		if (ReadCSV(stagePath) == false) {
			Debug.LogError("ステージの読み込みで不具合が発生したため終了しました");
			SystemSupporter.ExitGame();
		}
	}

	void Update() {

	}

	/// <summary>
	/// ステージのCSVを読み込みます
	/// </summary>
	/// <param name="path">ファイルのパス</param>
	bool ReadCSV(string path) {
		if (path == "") {
			Debug.LogError("Stageのファイルパスがnullでした");
			return false;
		}

		try {

		} catch (Exception e) {
			Debug.LogError(e);
		}
		return true;
	}

	/// <summary>
	/// オブジェクトを生成します。
	/// </summary>
	/// <param name="x">x座標</param>
	/// <param name="y">y座標</param>
	/// <param name="obj">対象のオブジェクト(char)</param>
	void GenerateObjct(float x, float y, char obj) {

	}
}
