using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �������ƃ��[�h�A�X�e�[�W�����̃N���X
/// </summary>
public class Stage : MonoBehaviour {

	public GameObject player;
	public string stagePath;

	public List<List<char>> stageData { get; private set; }

	void Start() {
		if (InputManager.isInit == false) {
			InputManager.Init();
		}

		if (player != null) {
			Instantiate(player);
		} else {
			Debug.LogError("�v���C���[���ݒ肳��Ă��܂���");
			SystemSupporter.ExitGame();
		}

		if (ReadCSV(stagePath) == false) {
			Debug.LogError("�X�e�[�W�̓ǂݍ��݂ŕs��������������ߏI�����܂���");
			SystemSupporter.ExitGame();
		}
	}

	void Update() {

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

		} catch (Exception e) {
			Debug.LogError(e);
		}
		return true;
	}

	/// <summary>
	/// �I�u�W�F�N�g�𐶐����܂��B
	/// </summary>
	/// <param name="x">x���W</param>
	/// <param name="y">y���W</param>
	/// <param name="obj">�Ώۂ̃I�u�W�F�N�g(char)</param>
	void GenerateObjct(float x, float y, char obj) {

	}
}
