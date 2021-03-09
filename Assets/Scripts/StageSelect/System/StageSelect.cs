using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour {

	public static StageSelect instance;

	#region �C���X�y�N�^�ҏW��
	public List<string> path;
	#endregion

	#region �C���X�y�N�^�Q�ƕ�
	[Disable]
	[SerializeField]
	private Text stageName;

	[Disable]
	[SerializeField]
	private GameObject createCaution; //�t�@�C���쐬���܂���
	#endregion

	#region �f�[�^��
	private int _stageIndex;
	private int stageIndex {
		get { return _stageIndex; }
		set { _stageIndex = (value + path.Count) % path.Count; }
	}

	public static bool isStageSelect;
	public static string stagePath;
	#endregion

	private void Awake() {
		instance = this;
	}

	// Start is called before the first frame update
	void Start() {
		gameObject.AddComponent<InputManager>();
		if (InputManager.isInit == false) {
			InputManager.Init();
#if UNITY_EDITOR
			SystemSupporter.DebugInitInput();
#endif
		}

		isStageSelect = true;
	}

	// Update is called once per frame
	void Update() {
		if (InputManager.GetKeyDown(Keys.LEFT)) {
			stageIndex--;
		}
		if (InputManager.GetKeyDown(Keys.RIGHT)) {
			stageIndex++;
		}
		if (InputManager.GetKeyDown(Keys.A)) {
			stagePath = path[stageIndex];
			SceneManager.LoadScene("Stage");
		}
		if (!Resources.Load("StageDatas/" + path[stageIndex])) {
			createCaution.SetActive(true);
		} else {
			createCaution.SetActive(false);
		}
	
		stageName.text = path[stageIndex];
	}
}
