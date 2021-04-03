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

    [SerializeField]
    private float viewAngle;

    [SerializeField]
    private float viewDist;

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
        if (Player.instance.stepCount > 0) {
            for (int i = 0; i < stepTimers.Count; ++i) {
                stepTimers[i] += Time.deltaTime;
            }
        }
        Move();
        CheckViewPlayer();


    }

    public void Move() {
        for (int i = 0; i < ghostCount; i++) {
            bool isStepMax = nowSteps[i] >= stepIntervals[i].Count - 1;

            //stepTimers�����͑ҋ@���Ԃ𒴂�����ړ�����
            if (stepIntervals[i][nowSteps[i]] < stepTimers[i] && isStepMax == false) {
                stepTimers[i] = 0;
                nowSteps[i]++;
                MoveNextStep(i);

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
            ghosts[i].transform.position = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
            stepTimers[i] = 0;
            nowSteps[i] = 0;
        }


        for (int i = 0; i < ghosts.Count; ++i) {
            if (nowSteps[i] == 0) {
                MoveNextStep(i);
            }
        }
    }

    //�S�[�X�g�̈ʒu�����Z�b�g
    private void ResetGhost(int i) {
        ghosts[i].transform.position = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        ghosts[i].transform.rotation = Quaternion.identity;
        nowSteps[i] = 0;
        stepTimers[i] = 0;
        MoveNextStep(i);
    }


    //�S�[�X�g�̎��E�Ƀv���C���[�������Ă��邩
    private void CheckViewPlayer() {
        for (int i = 0; i < ghostCount; ++i) {
            Vector3 distance = Player.instance.transform.position - ghosts[i].transform.position;
            Vector3 direction = Vector3.Normalize(distance);
            float len = Mathf.Pow(Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) + Mathf.Pow(distance.z, 2), 0.5f);

            //�v���C���[�Ƃ̋����̃x�N�g���Ɛ��ʕ����̃x�N�g���̊Ԃ̊p�x�����߂�
            float dot = Vector3.Dot(direction, ghosts[i].transform.forward);
            float angle = Mathf.Acos(dot);

            bool isIntoView = Mathf.Abs(angle) < viewAngle / 180 * 3.14 && len < viewDist;
            bool isViewForward = ghosts[i].transform.forward == direction && len < viewDist;
            if (isIntoView == true || isViewForward == true) {
                Vector3 playerPos = Player.instance.transform.position;
                Vector3 thisPos = ghosts[i].transform.position;

                bool isHideWall = false;

                for (float j = 0.2f; j < 1; j += 0.2f) {
                    Vector3 temp = Vector3.Lerp(thisPos, playerPos, j);
                    temp.x = Mathf.Round(temp.x);
                    temp.y = Mathf.Round(temp.y);
                    temp.z = Mathf.Round(temp.z);

                    GameObject obj = Stage.instance.GetStageObject(temp);

                    //�ǂɉB��Ă��邩���肷��
                    if (obj != null) {
                        if (obj.name[0] == '3') {
                            isHideWall = true;
                            break;
                        }
                    }
                }

                if (isHideWall == false) {
                    Debug.Log("�S�[�X�g" + i + "���v���C���[��������");
					Player.instance.GhostGameOver();
                }
            }
        }
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
