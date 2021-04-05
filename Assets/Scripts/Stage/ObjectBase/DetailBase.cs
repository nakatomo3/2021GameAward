using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DetailBase : MonoBehaviour {

	/// <summary>
	/// オブジェクト情報をファイル書き込み用のstringに変換
	/// </summary>
	/// <returns>ファイル書き込み用のstring</returns>
	public abstract string ToFileString();

	/// <summary>
	/// オブジェクト情報をステージエディタ用のstringに変換
	/// </summary>
	/// <returns>ステージエディタ用のstring</returns>
	public abstract string ToEditorString();

	/// <summary>
	/// オブジェクト情報のstringをオブジェクトの変数に反映
	/// </summary>
	/// <param name="information">パラメーター。各行ごとに連結している</param>
	public abstract void SetData(string information);

	protected string ConvertPos() {
		return Mathf.CeilToInt(transform.position.x) + "," + Mathf.CeilToInt(transform.position.z);
	}
}
