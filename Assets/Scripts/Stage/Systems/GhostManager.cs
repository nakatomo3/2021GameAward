using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostManager : MonoBehaviour {

    public static GhostManager instance;

    [HideInInspector]
    public List<List<MoveVector>> moveRecords;//[�S�[�X�g�ԍ�][�ړ�����]

    public List<List<float>> stepIntervals;   //[�S�[�X�g�ԍ�][���͑ҋ@����]
    private List<float> stepTimers;           //�S�[�X�g���Ƃ̓��͑ҋ@���Ԃ̃J�E���g
    private List<int> nowSteps;               //�S�[�X�g���Ƃ̌��݂̐i�񂾉�

    public List<List<bool>> isMoveSteps; //�v���C���[���ړ��������ǂ���


    private int ghostCount = 0;
    public GameObject ghost;
    public List<GameObject> ghosts;

    void Start() {
        instance = this;
        moveRecords = new List<List<MoveVector>>();
        stepIntervals = new List<List<float>>();
        stepTimers = new List<float>();
        nowSteps = new List<int>();
        isMoveSteps = new List<List<bool>>();
    }


    void Update() {
        if (Player.instance.isMoved == true) {
            for (int i = 0; i < stepTimers.Count; ++i) {
                stepTimers[i] += Time.deltaTime;
            }
        }
        Move();
    }

    public void Move() {
        for (int i = 0; i < ghostCount; i++) {
            bool isStepMax = nowSteps[i] >= stepIntervals[i].Count - 1;

            //stepTimers�����͑ҋ@���Ԃ𒴂�����ړ�����
            if (stepIntervals[i][nowSteps[i]] < stepTimers[i] && isStepMax == false) {
                stepTimers[i] = 0;
                MoveNextStep(i);
                nowSteps[i]++;
            }

            //�ړ����ő吔�𒴂��Ă�����ŏ��̈ʒu�ɖ߂�
            if (stepIntervals[i].Count > 0 && stepIntervals[i][nowSteps[i]] < stepTimers[i]) {
                if (isStepMax == true) {
                    ResetGhost(i);
                }
            }
        }
    }

    //�S�ẴS�[�X�g�����Z�b�g
    public void ResetStage() {
        for (int i = 0; i < ghostCount; i++) {
            ghosts[i].transform.position = Player.instance.startPosition;
            stepTimers[i] = 0;
            nowSteps[i] = 0;
        }

    }

    //�S�[�X�g�̈ʒu�����Z�b�g
    private void ResetGhost(int i) {
        ghosts[i].transform.position = Player.instance.startPosition;
        ghosts[i].transform.rotation = Quaternion.identity;
        nowSteps[i] = 0;
        stepTimers[i] = 0;
    }

    public void DeleteGhost() {
        for (int i = 0; i < ghosts.Count; ++i) {
            Destroy(ghosts[i].gameObject);
        }

        moveRecords.Clear();
        stepIntervals.Clear();
        stepTimers.Clear();
        nowSteps.Clear();
        isMoveSteps.Clear();
        ghosts.Clear();
        ghostCount = 0;
    }


    //moveRecords�����ƂɎ��̏ꏊ�Ɉړ�����
    public void MoveNextStep(int i) {
        switch (moveRecords[i][nowSteps[i]]) {
            case MoveVector.UP:
                if (isMoveSteps[i][nowSteps[i]] == true) {
                    ghosts[i].transform.position += Vector3.forward;
                }
                ghosts[i].transform.localEulerAngles = Vector3.up * 0;
                break;
            case MoveVector.DOWN:
                if (isMoveSteps[i][nowSteps[i]] == true) {
                    ghosts[i].transform.position += Vector3.back;
                }
                ghosts[i].transform.localEulerAngles = Vector3.up * 180;
                break;
            case MoveVector.LEFT:
                if (isMoveSteps[i][nowSteps[i]] == true) {
                    ghosts[i].transform.position += Vector3.left;
                }
                ghosts[i].transform.localEulerAngles = Vector3.up * -90;
                break;
            case MoveVector.RIGHT:
                if (isMoveSteps[i][nowSteps[i]] == true) {
                    ghosts[i].transform.position += Vector3.right;
                }
                ghosts[i].transform.localEulerAngles = Vector3.up * 90;
                break;
        }
    }


    public void AddGhost() {
        ghosts.Add(Instantiate(ghost, new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y), Quaternion.identity));
        ghostCount++;
        stepTimers.Add(0);
        nowSteps.Add(0);
    }


}
