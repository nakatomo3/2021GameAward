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

		//�ŏ��̃G���A
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
		} catch (Exception) {

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

		//stageData��������
		stageData = new List<List<char>>();
		for (int y = 0; y < maxDown - maxUp + 1; y++) {
			stageData.Add(new List<char>());
			for (int x = 0; x < maxRight - maxLeft + 1; x++) {
				stageData[y].Add('0');
			}
		}

		//stageData�Ɍ��݂̏󋵂𔽉f
		for (int i = 0; i < stageParent.transform.childCount; i++) {
			var obj = stageParent.transform.GetChild(i);
			var posX = Mathf.CeilToInt(obj.position.x - maxLeft);
			var posZ = Mathf.CeilToInt(obj.position.z - maxUp);
			stageData[posZ][posX] = GetObjectCode(obj.name);
		}

		//�t�@�C�����J���ăo�b�t�@����
		StreamWriter streamWriter = new StreamWriter(Application.dataPath + "/Resources/StageDatas/" + stagePath + ".txt", false);
		StringBuilder sb = new StringBuilder();

		//�X�e�[�W�w�b�_�[�ɏ�������
		sb.Append("#StageData\n");
		sb.Append((maxRight - maxLeft).ToString() + "," + (maxDown - maxUp).ToString() + "\n");
		sb.Append(0.ToString() + "\n\n"); //�X�e�[�W�f�U�C��

		sb.Append("#ObjectData\n");
		//�o�b�t�@�ɃX�e�[�W�f�[�^����������ł���
		for (int y = 0; y < stageData.Count; y++) {
			for (int x = 0; x < stageData[y].Count; x++) {
				sb.Append(stageData[y][x]);
			}
			sb.Append("\n");

		}

		//��������
		streamWriter.WriteLine(sb.ToString());

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

	private char GetObjectCode(string name) {
		for (int i = 0; i < objectList.Count; i++) {
			var objName = objectList[i].name;
			if (name.Contains(objName)) {
				return objectIndex[i];
			}
		}
		Debug.LogError("�I�u�W�F�N�g�̃R�[�h�擾���o���܂���ł����B" + name);
		return '0';
	}
}
