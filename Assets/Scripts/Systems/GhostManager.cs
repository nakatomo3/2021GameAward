using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostManager : MonoBehaviour {

    public static GhostManager instance;

    [HideInInspector]
    public List<List<MoveVector>> moveRecords;//[�S�[�X�g�ԍ�][�ړ�����]

    [HideInInspector]
    public List<List<float>> stepIntervals;   //[�S�[�X�g�ԍ�][���͑ҋ@����]
    private List<float> stepTimers;           //�S�[�X�g���Ƃ̓��͑ҋ@���Ԃ̃J�E���g
    private List<int> nowSteps;               //�S�[�X�g���Ƃ̌��݂̐i�񂾉�

	[HideInInspector]
	public List<List<bool>> isMoveSteps; //�v���C���[���ړ��������ǂ���

    [SerializeField]
    private float viewAngle;

    private int ghostCount = 0;
    public GameObject ghost;
    public List<GameObject> ghosts;

    void Start() {
        instance = this;
        moveRecords = new List<List<MoveVector>>();
        stepIntervals = new List<List<float>>();
        stepTimers = new List<float>();
        nowSteps = new List<int>();

    }


    void Update() {
        for (int i = 0; i < stepTimers.Count; ++i) {
            stepTimers[i] += Time.deltaTime;

        }
        Move();
        CheckViewPlayer();
    }

    public void Move() {
        for (int i = 0; i < ghostCount; i++) {
            //stepTimers�����͑ҋ@���Ԃ𒴂�����ړ�����
            if (stepIntervals[i][nowSteps[i]] < stepTimers[i]) {
                stepTimers[i] = 0;
                MoveNextStep(i);
                nowSteps[i]++;
            }

            //�ŏ��̈ʒu�ɖ߂�
            if (stepIntervals[i].Count > 0) {
                if (nowSteps[i] >= stepIntervals[i].Count - 1) {
                    ResetGhost(i);
                }
            }
        }
    }

    //�S�ẴS�[�X�g�����Z�b�g
    public void ResetStage() {
        for (int i = 0; i < ghostCount; i++) {
            ghosts[i].transform.position = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
            stepTimers[i] = 0;
            nowSteps[i] = 0;
        }
    }

    //�S�[�X�g�̈ʒu�����Z�b�g
    private void ResetGhost(int i) {
        ghosts[i].transform.position = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        ghosts[i].transform.rotation = Quaternion.identity;
        nowSteps[i] = 0;
        MoveNextStep(i);
    }


    //�S�[�X�g�̎��E�Ƀv���C���[�������Ă��邩
    private void CheckViewPlayer() {
        for (int i = 0; i < ghostCount; ++i) {
            Vector3 direction = Player.instance.transform.position - ghosts[i].transform.position;
            direction = Vector3.Normalize(direction);

            //�v���C���[�Ƃ̋����̃x�N�g���Ɛ��ʕ����̃x�N�g���̊Ԃ̊p�x�����߂�
            float dot = Vector3.Dot(direction, ghosts[i].transform.forward);
            float angle = Mathf.Acos(dot);

            if (Mathf.Abs(dot) > 0.9 || Mathf.Abs(angle) < viewAngle / 180 * 3.14) {
                Debug.Log("�S�[�X�g" + i + "���v���C���[��������");
            }
        }
    }


    //moveRecords�����ƂɎ��̏ꏊ�Ɉړ�����
    void MoveNextStep(int i) {
        switch (moveRecords[i][nowSteps[i]]) {
            case MoveVector.UP:
                ghosts[i].transform.position += Vector3.forward;
                ghosts[i].transform.localEulerAngles = Vector3.up * 0;
                break;
            case MoveVector.DOWN:
                ghosts[i].transform.position += Vector3.back;
                ghosts[i].transform.localEulerAngles = Vector3.up * 180;
                break;
            case MoveVector.LEFT:
                ghosts[i].transform.position += Vector3.left;
                ghosts[i].transform.localEulerAngles = Vector3.up * -90;
                break;
            case MoveVector.RIGHT:
                ghosts[i].transform.position += Vector3.right;
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
