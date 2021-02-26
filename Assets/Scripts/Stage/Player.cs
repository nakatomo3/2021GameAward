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

}
