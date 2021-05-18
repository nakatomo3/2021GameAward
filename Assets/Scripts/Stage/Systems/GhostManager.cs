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
    private List<Vector3> goalPositions;

	public List<Vector3> newPos;
	public List<Vector3> oldPos;

	private float moveStepRate = 0;//�ړ��̐��`�⊮�Ŏg��

	public GameObject ghost;
	public List<GameObject> ghosts;

	void Start() {
		instance = this;
		moveRecords = new List<List<ActionRecord>>();
		nowSteps = new List<int>();
		startPositions = new List<Vector3>();

		newPos = new List<Vector3>();
		oldPos = new List<Vector3>();

		goalPositions = new List<Vector3>();
	}


	void Update() {
		if (Player.instance.isAlive == true) {
			moveStepRate += Time.deltaTime;
			if (moveStepRate >= Player.instance.moveIntervalMax) {
				moveStepRate = Player.instance.moveIntervalMax;

				for (int i = 0; i < ghosts.Count; ++i) {
					oldPos[i] = newPos[i];
				}
			}
			for (int i = 0; i < ghosts.Count; ++i) {
				ghosts[i].transform.position = Vector3.Lerp(oldPos[i], newPos[i], moveStepRate / Player.instance.moveIntervalMax);
			}
		}
	}

	public void Action() {
		for (int i = 0; i < ghosts.Count; i++) {
			//�ő�X�e�b�v�ɂȂ�����ړ����Ȃ�
			if (nowSteps[i] < moveRecords[i].Count) {
				MoveNextStep(i);
				nowSteps[i]++;
			}

			var nowObj = Stage.instance.GetStageObject(ghosts[i].transform.position);
			if (nowObj != null && nowObj.name[0] == 'J' && ((int)Mathf.Pow(2, i) & nowObj.GetComponent<Goal>().phaseCount) > 0) {
				ResetGhost(i);
			}
		}
	}

	//�S�ẴS�[�X�g�����Z�b�g
	public void ResetStage() {
		for (int i = 0; i < ghosts.Count; i++) {
			ghosts[i].transform.position = startPositions[i];
			ghosts[i].transform.localEulerAngles = new Vector3();
			newPos[i] = startPositions[i];
			oldPos[i] = startPositions[i];
			nowSteps[i] = 0;
		}
	}

	//�S�[�X�g�̈ʒu�����Z�b�g
	public void ResetGhost(int i) {
		ghosts[i].transform.position = startPositions[i];
		ghosts[i].transform.rotation = Quaternion.identity;
		nowSteps[i] = 0;

		newPos[i] = startPositions[i];
		oldPos[i] = startPositions[i];
	}

	public void DeleteGhost() {
		for (int i = 0; i < ghosts.Count; ++i) {
			Destroy(ghosts[i].gameObject);
		}

		moveRecords.Clear();
		startPositions.Clear();
		nowSteps.Clear();
		ghosts.Clear();
	}


	//moveRecords�����ƂɎ��̏ꏊ�Ɉړ�����
	public void MoveNextStep(int i) {
		moveStepRate = 0;
		switch (moveRecords[i][nowSteps[i]]) {
			case ActionRecord.UP:
				newPos[i] = ghosts[i].transform.position + Vector3.forward;
				ghosts[i].transform.localEulerAngles = Vector3.up * 0;
				break;

			case ActionRecord.DOWN:
				newPos[i] = ghosts[i].transform.position + Vector3.back;
				ghosts[i].transform.localEulerAngles = Vector3.up * 180;
				break;

			case ActionRecord.LEFT:
				newPos[i] = ghosts[i].transform.position + Vector3.left;
				ghosts[i].transform.localEulerAngles = Vector3.up * -90;
				break;

			case ActionRecord.RIGHT:
				newPos[i] = ghosts[i].transform.position + Vector3.right;
				ghosts[i].transform.localEulerAngles = Vector3.up * 90;
				break;

			case ActionRecord.ATTACK:
				//�U�����[�V����
				break;

			case ActionRecord.DAMAGE:
				//�_���[�W���[�V����
				break;
		}
	}


	public void AddGhost(Vector3 startPos, Vector3 goalPos) {
		ghosts.Add(Instantiate(ghost, startPos, Quaternion.identity));
		nowSteps.Add(0);
		startPositions.Add(startPos);
		oldPos.Add(startPos);
		newPos.Add(startPos);

		oldPos[startPositions.Count - 1] = startPos;
		newPos[startPositions.Count - 1] = startPos;
		ghosts[ghosts.Count - 1].GetComponent<GhostNormal>().id = ghosts.Count - 1;

		goalPositions.Add(goalPos);
	}

	public void Rewind() {
		for (int i = 0; i < ghosts.Count; i++) {
			switch (moveRecords[i][nowSteps[i]]) {
				case ActionRecord.UP:
					ghosts[i].transform.position -= Vector3.forward;
					ghosts[i].transform.localEulerAngles = Vector3.up * 0;
					break;
				case ActionRecord.DOWN:
					ghosts[i].transform.position -= Vector3.down;
					ghosts[i].transform.localEulerAngles = Vector3.up * 180;
					break;
				case ActionRecord.LEFT:
					ghosts[i].transform.position -= Vector3.left;
					ghosts[i].transform.localEulerAngles = Vector3.up * -90;
					break;
				case ActionRecord.RIGHT:
					ghosts[i].transform.position -= Vector3.right;
					ghosts[i].transform.localEulerAngles = Vector3.up * 90;
					break;
				case ActionRecord.NONE:
				case ActionRecord.ATTACK:
				case ActionRecord.DAMAGE:
					//�_���[�W���[�V����
					break;
			}
			if (ghosts[i].transform.position == startPositions[i]) {
				ghosts[i].transform.position = goalPositions[i];
			}
			nowSteps[i]--;
			if (nowSteps[i] < 0) {
				nowSteps[i] = moveRecords[i].Count - 1;
				ghosts[i].transform.position = goalPositions[i];
			}
		}

	}

    public void PhaseBack() {
        Destroy(ghosts[ghosts.Count - 1]);

        newPos.RemoveAt(newPos.Count - 1);
        oldPos.RemoveAt(oldPos.Count - 1);
        moveRecords.RemoveAt(moveRecords.Count - 1);
        ghosts.RemoveAt(ghosts.Count - 1);
        startPositions.RemoveAt(startPositions.Count - 1);
        goalPositions.RemoveAt(goalPositions.Count - 1);

        
    }
}
