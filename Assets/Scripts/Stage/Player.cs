using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤークラス。この下にPlayerMoveやPlayerStatusなど別クラスを作る予定
/// </summary>
public class Player : MonoBehaviour {

	public static Player instance;

	private void Awake() {
		if(instance != null) {
			Destroy(instance);
		}
		instance = this;
	}

	public void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	/// <summary>
	/// 基本的にこの関数は変数を変更するのみ。一定タイミングで自動的にこの関数が呼ばれる。
	/// </summary>
	void Action() {

	}
}
