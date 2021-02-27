using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetailBase : MonoBehaviour {
	//オブジェクト情報をファイル書き込み用のstringに変換
	public virtual string ToFileString() { return ""; }

	//オブジェクト情報をステージエディタ用のstringに変換
	public virtual string ToEditorString() { return ""; }
}
