using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// �������ƃ��[�h�A�X�e�[�W�����̃N���X
/// </summary>
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
	#endregion


	#region �f�[�^��
	public GameObject stageParent { get; private set; }
	public List<List<char>> stageData { get; private set; }
	private Vector2 startPosition = new Vector2();
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
			Debug.LogError("�v���C���[���ݒ肳��Ă��܂���");
			SystemSupporter.ExitGame();
		}

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
			Debug.Log("�t�@�C����������܂���ł����B�V�K�쐬���[�h�ɂ��܂�");
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
	/// �I�u�W�F�N�g�𐶐����܂��B
	/// </summary>
	/// <param name="x">x���W</param>
	/// <param name="y">y���W</param>
	/// <param name="obj">�Ώۂ̃I�u�W�F�N�g(char)</param>
	void GenerateObjct(float x, float y, char obj) {

	}

	/// <summary>
	/// ���̍��W�̃M�~�b�N�I�u�W�F�N�g���擾���܂�
	/// </summary>
	/// <param name="pos">���W</param>
	/// <returns>���̍��W�̃X�e�[�W�I�u�W�F�N�g</returns>
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
