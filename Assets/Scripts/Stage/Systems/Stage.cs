using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;

/// <summary>
/// 初期化とロード、ステージ生成のクラス
/// </summary>
[DisallowMultipleComponent]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
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

	[Disable]
	[SerializeField]
	private GameObject ghostManager;
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
		gameObject.AddComponent<InputManager>();
		gameObject.AddComponent<SwitchManager>();

		if (InputManager.isInit == false) {
			InputManager.Init();
#if UNITY_EDITOR
			SystemSupporter.DebugInitInput();
#endif
		}

		player = Instantiate(player);
		ghostManager = Instantiate(ghostManager);

		if (ReadCSV(stagePath) == false) {
			Debug.LogError("ステージの読み込みで不具合が発生したため終了しました");
			SystemSupporter.ExitGame();
		}

		Instantiate(objectList[1]);
		Instantiate(start);

		//最初のエリア
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
			if (SystemSupporter.IsUnityEditor() == true) {
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

			//int lineCount = 0;
			//while (reader.Peek() > -1) {

			//	string line = reader.ReadLine();
			//	string[] values = line.Split(',');
			//	for (int j = 0; j < values.Length; j++) {
			//		stageData[lineCount][j] = values[j][0];
			//	}
			//	lineCount++;
			//}
		} catch (NullReferenceException) {
			Debug.Log("ファイルが見つかりませんでした。新規作成モードにします");
			StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
			streamWriter.Flush();
			streamWriter.Close();
		} catch (Exception) {

		}
		return true;
	}

	/// <summary>
	/// コンストラクタ(InitializeOnLoad属性によりエディター起動時に呼び出される)
	/// </summary>
	static Stage() {
		//Playmodeの状態が変わった時のイベントを登録
#if UNITY_EDITOR

		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
	}

	private static void OnPlayModeStateChanged(PlayModeStateChange state) {
#if UNITY_EDITOR
		//再生終了ボタンを押した
		if (instance != null && !EditorApplication.isPlayingOrWillChangePlaymode) {
			instance.WriteCSV();
		}
		try {
		} catch (Exception e) {
			Debug.LogError("終了時にエラーが発生しました" + e);
		}
	}
#endif

	void WriteCSV() {

		float maxLeft = 0;
		float maxRight = 0;
		float maxUp = 0;
		float maxDown = 0;

		//マップ作成。全てのオブジェクトを調べ、全てのオブジェクトが入るList<List<char>>を作る
		for (int i = 0; i < stageParent.transform.childCount; i++) {
			var child = stageParent.transform.GetChild(i).transform;
			if (child.position.x < maxLeft) {
				maxLeft = child.position.x;
			}
			if (child.position.x > maxRight) {
				maxRight = child.position.x;
			}
			if (child.position.z < maxUp) {
				maxUp = child.position.z;
			}
			if (child.position.z > maxDown) {
				maxDown = child.position.z;
			}
		}

		//stageDataを初期化
		stageData = new List<List<char>>();
		for (int y = 0; y < maxDown - maxUp + 1; y++) {
			stageData.Add(new List<char>());
			for (int x = 0; x < maxRight - maxLeft + 1; x++) {
				stageData[y].Add('0');
			}
		}

		//stageDataに現在の状況を反映
		for (int i = 0; i < stageParent.transform.childCount; i++) {
			var obj = stageParent.transform.GetChild(i);
			var posX = Mathf.CeilToInt(obj.position.x - maxLeft);
			var posZ = Mathf.CeilToInt(obj.position.z - maxUp);
			stageData[posZ][posX] = GetObjectCode(obj.name);
		}

		//ファイルを開いてバッファ準備
		StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
		StringBuilder sb = new StringBuilder();

		//ステージヘッダーに書き込み
		sb.Append("#StageData\n");
		sb.Append((maxRight - maxLeft).ToString() + "," + (maxDown - maxUp).ToString() + "\n");
		sb.Append(0.ToString() + "\n\n"); //ステージデザイン

		sb.Append("#ObjectData\n");
		//バッファにステージデータを書き込んでいく
		for (int y = 0; y < stageData.Count; y++) {
			for (int x = 0; x < stageData[y].Count; x++) {
				sb.Append(stageData[y][x]);
			}
			sb.Append("\n");

		}

		//書きこみ
		streamWriter.WriteLine(sb.ToString());

		//ファイルを閉じる
		streamWriter.Flush();
		streamWriter.Close();
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
		if (pos == Vector3.zero) {
			//スタートエリアなのでスキップ
			return;
		}
		bool isExistObj = false;
		GameObject existObj = GetStageObject(pos);
		string objName = "";
		if (existObj != null) {
			isExistObj = true;
			objName = existObj.name;
		}
		if (isExistObj == false) {
			Instantiate(obj, pos, Quaternion.identity, stageParent.transform);
		} else if (objName != obj.name + "(Clone)") {
			Destroy(existObj);
			Instantiate(obj, pos, Quaternion.identity, stageParent.transform);
		}
	}

	/// <summary>
	/// その座標のギミックオブジェクトを取得します
	/// </summary>
	/// <param name="pos">座標</param>
	/// <returns>その座標のステージオブジェクト</returns>
	public GameObject GetStageObject(Vector3 pos) {
		for (int i = 0; i < stageParent.transform.childCount; i++) {
			var child = stageParent.transform.GetChild(i);
			if (child.transform.position == pos) {
				return child.gameObject;
			}
		}
		return null;
	}

	private char GetObjectCode(string name) {
		for (int i = 0; i < objectList.Count; i++) {
			var objName = objectList[i].name;
			if (name.Contains(objName)) {
				return objectIndex[i];
			}
		}
		Debug.LogError("オブジェクトのコード取得が出来ませんでした。" + name);
		return '0';
	}
}
