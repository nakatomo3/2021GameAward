using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �v���C���[�N���X�B���̉���PlayerMove��PlayerStatus�ȂǕʃN���X�����\��
/// </summary>
public class Player : MonoBehaviour {

	public static Player instance;

	private void Awake() {
		if(instance != null) {
			Destroy(instance);
		}
		instance = this;
	}

	public void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	/// <summary>
	/// ��{�I�ɂ��̊֐��͕ϐ���ύX����̂݁B���^�C�~���O�Ŏ����I�ɂ��̊֐����Ă΂��B
	/// </summary>
	void Action() {

	}
}
