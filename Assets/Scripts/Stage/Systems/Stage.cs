using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// 初期化とロード、ステージ生成のクラス
/// </summary>
public class Stage : MonoBehaviour {



	#region インスペクタ編集部
	public static Stage instance;

	[Tooltip("編集モード")]
	public bool isEditorMode = false;

	[Tooltip("Resources/StageDatas以下のパスです")]
	public string stagePath;

	public List<char> objectIndex;
	public List<GameObject> objectList;
	#endregion

	[Header("-------------")]


	#region インスペクタ参照部(いじれないようにDisable属性)
	[Disable]
	[SerializeField]
	private GameObject player;

	[Disable]
	[SerializeField]
	private GameObject stageEditor;

	[Disable]
	[SerializeField]
	new public GameObject camera;

	[Disable]
	[SerializeField]
	private GameObject start;
	#endregion


	#region データ部
	public GameObject stageParent { get; private set; }
	public List<List<char>> stageData { get; private set; }
	public Vector2 startPosition { get; private set; }
	#endregion

	private void Awake() {
		instance = this;

#if UNITY_EDITOR
		stageEditor = Instantiate(stageEditor, transform);
#else
		isEditorMode = false;
#endif
		stageParent = new GameObject("StageParent");
		stageParent.transform.parent = transform;
	}

	void Start() {
		if (InputManager.isInit == false) {
			InputManager.Init();
#if UNITY_EDITOR
			SystemSupporter.DebugInitInput();
#endif
		}

		if (player != null) {
			player = Instantiate(player);
		} else {
			Debug.LogError("プレイヤーが設定されていません");
			SystemSupporter.ExitGame();
		}

		if (ReadCSV(stagePath) == false) {
			Debug.LogError("ステージの読み込みで不具合が発生したため終了しました");
			SystemSupporter.ExitGame();
		}

		Instantiate(objectList[1]);
		Instantiate(start);
		Instantiate(objectList[0], new Vector3(-1, 0, 1), Quaternion.identity, stageParent.transform);
		Instantiate(objectList[0], new Vector3(0, 0, 1), Quaternion.identity, stageParent.transform);
		Instantiate(objectList[0], new Vector3(1, 0, 1), Quaternion.identity, stageParent.transform);
		Instantiate(objectList[0], new Vector3(-1, 0, 0), Quaternion.identity, stageParent.transform);
		Instantiate(objectList[0], new Vector3(1, 0, 0), Quaternion.identity, stageParent.transform);
		Instantiate(objectList[0], new Vector3(-1, 0, -1), Quaternion.identity, stageParent.transform);
		Instantiate(objectList[0], new Vector3(0, 0, -1), Quaternion.identity, stageParent.transform);
		Instantiate(objectList[0], new Vector3(1, 0, -1), Quaternion.identity, stageParent.transform);

	}

	void Update() {
		SystemSupporter.PlaySupport();

		player.SetActive(!isEditorMode);
		stageEditor.SetActive(isEditorMode);

		if (InputManager.GetKeyDown(Keys.START)) {
			isEditorMode = !isEditorMode;
			if(SystemSupporter.IsUnityEditor() == true) {
				player.transform.position = stageEditor.transform.position;
			}
		}
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
			TextAsset csv = Resources.Load("StageDatas/" + path) as TextAsset;
			StringReader reader = new StringReader(csv.text);

			int lineCount = 0;
			while (reader.Peek() > -1) {

				string line = reader.ReadLine();
				string[] values = line.Split(',');
				for (int j = 0; j < values.Length; j++) {
					stageData[lineCount][j] = values[j][0];
				}
				lineCount++;
			}
		} catch (NullReferenceException) {
			Debug.Log("ファイルが見つかりませんでした。新規作成モードにします");
			StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
			streamWriter.Flush();
			streamWriter.Close();
		} catch (Exception) {

		}
		return true;
	}

	void WriteCSV() {

	}

	public void WriteStage(float x, float y, char obj) {

	}

	/// <summary>
	/// オブジェクトを生成します。
	/// </summary>
	/// <param name="x">x座標</param>
	/// <param name="y">y座標</param>
	/// <param name="obj">対象のオブジェクト(char)</param>
	void GenerateObjct(float x, float y, char obj) {

	}

	public void GenerateObject(Vector3 pos, GameObject obj) {
		bool isExistObj = false;
		GameObject existObj = GetStageObject(transform.position);
		string objName = "";
		if (existObj != null) {
			isExistObj = true;
			objName = existObj.name;
		}
		if (isExistObj == false) {
			Instantiate(obj, pos, Quaternion.identity, stageParent.transform);
		} else if (objName != obj.name + "(Clone)") {
			Destroy(obj.gameObject);
			Instantiate(obj, pos, Quaternion.identity, stageParent.transform);
		}
	}

	/// <summary>
	/// その座標のギミックオブジェクトを取得します
	/// </summary>
	/// <param name="pos">座標</param>
	/// <returns>その座標のステージオブジェクト</returns>
	public GameObject GetStageObject(Vector3 pos) {
		for (int i = 0; i < Stage.instance.stageParent.transform.childCount; i++) {
			var child = stageParent.transform.GetChild(i);
			if (child.transform.position == pos) {
				return child.gameObject;
			}
		}
		return null;
	}
}
