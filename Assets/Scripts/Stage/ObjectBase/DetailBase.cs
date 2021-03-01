using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DetailBase : MonoBehaviour {
	//オブジェクト情報をファイル書き込み用のstringに変換
	public abstract string ToFileString();

	//オブジェクト情報をステージエディタ用のstringに変換
	public abstract string ToEditorString();

	//オブジェクト情報のstringをオブジェクトの変数に反映
	//public abstract void SetData(string information);
}
