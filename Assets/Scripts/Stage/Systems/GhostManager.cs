using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostManager : MonoBehaviour {

	public static GhostManager instance;

	[HideInInspector]
	public List<List<ActionRecord>> moveRecords;//[�S�[�X�g�ԍ�][�ړ�����]
	private List<int> nowSteps;               //�S�[�X�g���Ƃ̌��݂̐i�񂾉�

	private List<Vector3> startPositions;

	public List<List<bool>> isMoveSteps; //�v���C���[���ړ��������ǂ���


	private int ghostCount = 0;
	public GameObject ghost;
	public List<GameObject> ghosts;

	void Start() {
		instance = this;
		moveRecords = new List<List<ActionRecord>>();
		nowSteps = new List<int>();
		isMoveSteps = new List<List<bool>>();
		startPositions = new List<Vector3>();
	}


	void Update() {
		//Move();
	}

	public void Action() {
		for (int i = 0; i < ghostCount; i++) {
			MoveNextStep(i);
			nowSteps[i]++;

			//�ړ����ő吔�𒴂��Ă�����ŏ��̈ʒu�ɖ߂�
			if (moveRecords[i].Count > nowSteps[i]) {
				ResetGhost(i);
			}
		}
	}

	//�S�ẴS�[�X�g�����Z�b�g
	public void ResetStage() {
		for (int i = 0; i < ghostCount; i++) {
			ghosts[i].transform.position = Player.instance.startPosition;
			nowSteps[i] = 0;
		}

	}

	//�S�[�X�g�̈ʒu�����Z�b�g
	private void ResetGhost(int i) {
		ghosts[i].transform.position = Player.instance.startPosition;
		ghosts[i].transform.rotation = Quaternion.identity;
		nowSteps[i] = 0;
	}

	public void DeleteGhost() {
		for (int i = 0; i < ghosts.Count; ++i) {
			Destroy(ghosts[i].gameObject);
		}

		moveRecords.Clear();
		startPositions.Clear();
		nowSteps.Clear();
		isMoveSteps.Clear();
		ghosts.Clear();
		ghostCount = 0;
	}


	//moveRecords�����ƂɎ��̏ꏊ�Ɉړ�����
	public void MoveNextStep(int i) {
		switch (moveRecords[i][nowSteps[i]]) {
			case ActionRecord.UP:
				if (isMoveSteps[i][nowSteps[i]] == true) {
					ghosts[i].transform.position += Vector3.forward;
				}
				ghosts[i].transform.localEulerAngles = Vector3.up * 0;
				break;
			case ActionRecord.DOWN:
				if (isMoveSteps[i][nowSteps[i]] == true) {
					ghosts[i].transform.position += Vector3.back;
				}
				ghosts[i].transform.localEulerAngles = Vector3.up * 180;
				break;
			case ActionRecord.LEFT:
				if (isMoveSteps[i][nowSteps[i]] == true) {
					ghosts[i].transform.position += Vector3.left;
				}
				ghosts[i].transform.localEulerAngles = Vector3.up * -90;
				break;
			case ActionRecord.RIGHT:
				if (isMoveSteps[i][nowSteps[i]] == true) {
					ghosts[i].transform.position += Vector3.right;
				}
				ghosts[i].transform.localEulerAngles = Vector3.up * 90;
				break;
		}
	}


	public void AddGhost(Vector3 startPos) {
		ghosts.Add(Instantiate(ghost, new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y), Quaternion.identity));
		ghostCount++;
		nowSteps.Add(0);
		startPositions.Add(startPos);
	}


}
