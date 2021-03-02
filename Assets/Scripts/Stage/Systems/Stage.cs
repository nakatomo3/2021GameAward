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

	private int visualMode = 0;

	private List<string> comments = new List<string>();

	private const string stageHeader = "#StageData";
	private const string objectHeader = "#ObjectData";
	private const string detailHeader = "#Detail";
	private const string commentHeader = "#Comment";

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

			reader.ReadLine(); // ステージ全体のヘッダ読み込み
			var line = reader.ReadLine();

			//ステージ配列を初期化
			stageData = new List<List<char>>();
			var sizeX = int.Parse(line.Split(':')[1].Split(',')[0]);
			var sizeY = int.Parse(line.Split(':')[1].Split(',')[1]);
			for (int i = 0; i < sizeY; i++) {
				stageData.Add(new List<char>());
				for (int j = 0; j < sizeX; j++) {
					stageData[i].Add('0');
				}
			}

			visualMode = int.Parse(reader.ReadLine().Split(':')[1]);
			line = reader.ReadLine();
			var posX = int.Parse(line.Split(':')[1].Split(',')[0]);
			var posY = int.Parse(line.Split(':')[1].Split(',')[1]);
			reader.ReadLine();

			//objectHeaderに当たるまでコメント読み込み
			bool isComment = false;
			while (line != objectHeader) {
				if (isComment == true) {
					comments.Add(line);
				}
				if (line == commentHeader) {
					isComment = true;
				}
				line = reader.ReadLine();
			}

			//ステージ部分読み込みと生成
			line = reader.ReadLine();
			var lineCount = 0;
			Debug.Log("read:" + line);
			while (line != detailHeader || reader.Peek() > -1) {
				if (line == "") {
					break;
				}
				var lineData = line.ToCharArray();
				Debug.Log(line);
				for (int i = 0; i < line.Length - 1; i++) {
					var code = lineData[i];
					stageData[lineCount][i] = code;
					if (code != '0') {
						Instantiate(objectList[objectIndex.FindIndex(n => code == n)], new Vector3(posX + i, 0, posY + lineCount), Quaternion.identity, stageParent.transform);
					}
				}
				if (reader.Peek() <= -1) { //Detail終了時
					Debug.Log("Detailなしで終了しました");
					return true;
				}
				line = reader.ReadLine();
				lineCount++;
			}

			//詳細読み込み
			while (reader.Peek() > -1) {
				line = reader.ReadLine();

				Debug.Log(line);
			}

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

			//最初のエリア
			Instantiate(objectList[0], new Vector3(-1, 0, 1), Quaternion.identity, stageParent.transform);
			Instantiate(objectList[0], new Vector3(0, 0, 1), Quaternion.identity, stageParent.transform);
			Instantiate(objectList[0], new Vector3(1, 0, 1), Quaternion.identity, stageParent.transform);
			Instantiate(objectList[0], new Vector3(-1, 0, 0), Quaternion.identity, stageParent.transform);
			Instantiate(objectList[0], new Vector3(1, 0, 0), Quaternion.identity, stageParent.transform);
			Instantiate(objectList[0], new Vector3(-1, 0, -1), Quaternion.identity, stageParent.transform);
			Instantiate(objectList[0], new Vector3(0, 0, -1), Quaternion.identity, stageParent.transform);
			Instantiate(objectList[0], new Vector3(1, 0, -1), Quaternion.identity, stageParent.transform);
		} catch (Exception e) {
			Debug.LogError(e);
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

		var sizeX = maxRight - maxLeft + 1;
		var sizeY = maxDown - maxUp + 1;

		//stageDataを初期化
		stageData = new List<List<char>>();
		for (int y = 0; y < sizeY; y++) {
			stageData.Add(new List<char>());
			for (int x = 0; x < sizeX; x++) {
				stageData[y].Add('0');
			}
		}

		//ファイルを開いてバッファ準備
		StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
		StringBuilder sb = new StringBuilder();
		StringBuilder detailSb = new StringBuilder(); //詳細設定用StringBuilder

		//ステージヘッダーに書き込み
		sb.AppendLine(stageHeader);
		sb.AppendLine("size:" + sizeX.ToString() + "," + sizeY.ToString());
		sb.AppendLine("design:" + 0.ToString()); //ステージデザイン
		sb.AppendLine("pos:" + maxLeft + "," + maxUp);
		sb.AppendLine();

		sb.AppendLine(commentHeader);
		for (int i = 0; i < comments.Count; i++) {
			sb.AppendLine(comments[i]);
		}
		if (comments.Count == 0) {
			sb.AppendLine();
		}

		sb.AppendLine(objectHeader);

		//詳細ヘッダーに書き込み
		detailSb.AppendLine("#Detail");

		//stageDataに現在の状況を反映
		for (int i = 0; i < stageParent.transform.childCount; i++) {
			var obj = stageParent.transform.GetChild(i);
			var posX = Mathf.CeilToInt(obj.position.x - maxLeft);
			var posZ = Mathf.CeilToInt(maxDown - obj.position.z);
			var code = GetObjectCode(obj.name);
			stageData[posZ][posX] = code;

			if (code >= 'A' && code <= 'Z') {
				detailSb.AppendLine(obj.GetComponent<DetailBase>().ToFileString());
			}
		}

		//バッファにステージデータを書き込んでいく
		for (int y = 0; y < stageData.Count; y++) {
			for (int x = 0; x < stageData[y].Count; x++) {
				var code = stageData[y][x];
				sb.Append(code);
			}
			sb.Append("\n");

		}

		//書きこみ
		streamWriter.WriteLine(sb.ToString());
		if (detailSb.Length > 10) { //何か書き込まれていたら
			streamWriter.Write(detailSb.ToString());
		}

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

	public static char GetObjectCode(string name) {
		//命名規則は「コード_名前」なのでコード単独の抽出可能
		return name.Split('_')[0][0];

		//問題があればこのコードに戻す
		//for (int i = 0; i < objectList.Count; i++) {
		//	var objName = objectList[i].name;
		//	if (name.Contains(objName)) {
		//		return objectIndex[i];
		//	}
		//}
		//Debug.LogError("オブジェクトのコード取得が出来ませんでした。" + name);
		//return '0';
	}
}
