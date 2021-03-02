using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Text;

/// <summary>
/// �������ƃ��[�h�A�X�e�[�W�����̃N���X
/// </summary>
[DisallowMultipleComponent]
#if UNITY_EDITOR
[InitializeOnLoad]
#endif
public class Stage : MonoBehaviour {



	#region �C���X�y�N�^�ҏW��
	public static Stage instance;

	[Tooltip("�ҏW���[�h")]
	public bool isEditorMode = false;

	[Tooltip("Resources/StageDatas�ȉ��̃p�X�ł�")]
	public string stagePath;

	public List<char> objectIndex;
	public List<GameObject> objectList;
	#endregion

	[Header("-------------")]


	#region �C���X�y�N�^�Q�ƕ�(������Ȃ��悤��Disable����)
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


	#region �f�[�^��
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
			Debug.LogError("�X�e�[�W�̓ǂݍ��݂ŕs��������������ߏI�����܂���");
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
	/// �X�e�[�W��CSV��ǂݍ��݂܂�
	/// </summary>
	/// <param name="path">�t�@�C���̃p�X</param>
	bool ReadCSV(string path) {
		if (path == "") {
			Debug.LogError("Stage�̃t�@�C���p�X��null�ł���");
			return false;
		}

		try {
			TextAsset csv = Resources.Load("StageDatas/" + path) as TextAsset;
			StringReader reader = new StringReader(csv.text);

			reader.ReadLine(); // �X�e�[�W�S�̂̃w�b�_�ǂݍ���
			var line = reader.ReadLine();

			//�X�e�[�W�z���������
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

			//objectHeader�ɓ�����܂ŃR�����g�ǂݍ���
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

			//�X�e�[�W�����ǂݍ��݂Ɛ���
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
				if (reader.Peek() <= -1) { //Detail�I����
					Debug.Log("Detail�Ȃ��ŏI�����܂���");
					return true;
				}
				line = reader.ReadLine();
				lineCount++;
			}

			//�ڍדǂݍ���
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
			Debug.Log("�t�@�C����������܂���ł����B�V�K�쐬���[�h�ɂ��܂�");
			StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
			streamWriter.Flush();
			streamWriter.Close();

			//�ŏ��̃G���A
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
	/// �R���X�g���N�^(InitializeOnLoad�����ɂ��G�f�B�^�[�N�����ɌĂяo�����)
	/// </summary>
	static Stage() {
		//Playmode�̏�Ԃ��ς�������̃C�x���g��o�^
#if UNITY_EDITOR

		EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
	}

	private static void OnPlayModeStateChanged(PlayModeStateChange state) {
#if UNITY_EDITOR
		//�Đ��I���{�^����������
		if (instance != null && !EditorApplication.isPlayingOrWillChangePlaymode) {
			instance.WriteCSV();
		}
		try {
		} catch (Exception e) {
			Debug.LogError("�I�����ɃG���[���������܂���" + e);
		}
	}
#endif

	void WriteCSV() {

		float maxLeft = 0;
		float maxRight = 0;
		float maxUp = 0;
		float maxDown = 0;

		//�}�b�v�쐬�B�S�ẴI�u�W�F�N�g�𒲂ׁA�S�ẴI�u�W�F�N�g������List<List<char>>�����
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

		//stageData��������
		stageData = new List<List<char>>();
		for (int y = 0; y < sizeY; y++) {
			stageData.Add(new List<char>());
			for (int x = 0; x < sizeX; x++) {
				stageData[y].Add('0');
			}
		}

		//�t�@�C�����J���ăo�b�t�@����
		StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
		StringBuilder sb = new StringBuilder();
		StringBuilder detailSb = new StringBuilder(); //�ڍאݒ�pStringBuilder

		//�X�e�[�W�w�b�_�[�ɏ�������
		sb.AppendLine(stageHeader);
		sb.AppendLine("size:" + sizeX.ToString() + "," + sizeY.ToString());
		sb.AppendLine("design:" + 0.ToString()); //�X�e�[�W�f�U�C��
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

		//�ڍ׃w�b�_�[�ɏ�������
		detailSb.AppendLine("#Detail");

		//stageData�Ɍ��݂̏󋵂𔽉f
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

		//�o�b�t�@�ɃX�e�[�W�f�[�^����������ł���
		for (int y = 0; y < stageData.Count; y++) {
			for (int x = 0; x < stageData[y].Count; x++) {
				var code = stageData[y][x];
				sb.Append(code);
			}
			sb.Append("\n");

		}

		//��������
		streamWriter.WriteLine(sb.ToString());
		if (detailSb.Length > 10) { //�����������܂�Ă�����
			streamWriter.Write(detailSb.ToString());
		}

		//�t�@�C�������
		streamWriter.Flush();
		streamWriter.Close();
	}

	public void WriteStage(float x, float y, char obj) {

	}

	/// <summary>
	/// �I�u�W�F�N�g�𐶐����܂��B
	/// </summary>
	/// <param name="x">x���W</param>
	/// <param name="y">y���W</param>
	/// <param name="obj">�Ώۂ̃I�u�W�F�N�g(char)</param>
	void GenerateObjct(float x, float y, char obj) {

	}

	public void GenerateObject(Vector3 pos, GameObject obj) {
		if (pos == Vector3.zero) {
			//�X�^�[�g�G���A�Ȃ̂ŃX�L�b�v
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
	/// ���̍��W�̃M�~�b�N�I�u�W�F�N�g���擾���܂�
	/// </summary>
	/// <param name="pos">���W</param>
	/// <returns>���̍��W�̃X�e�[�W�I�u�W�F�N�g</returns>
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
		//�����K���́u�R�[�h_���O�v�Ȃ̂ŃR�[�h�P�Ƃ̒��o�\
		return name.Split('_')[0][0];

		//��肪����΂��̃R�[�h�ɖ߂�
		//for (int i = 0; i < objectList.Count; i++) {
		//	var objName = objectList[i].name;
		//	if (name.Contains(objName)) {
		//		return objectIndex[i];
		//	}
		//}
		//Debug.LogError("�I�u�W�F�N�g�̃R�[�h�擾���o���܂���ł����B" + name);
		//return '0';
	}
}
