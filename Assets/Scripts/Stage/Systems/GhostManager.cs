using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostManager : MonoBehaviour {

    public static GhostManager instance;

    [HideInInspector]
    public List<List<MoveVector>> moveRecords;//[ゴースト番号][移動方向]

    public List<List<float>> stepIntervals;   //[ゴースト番号][入力待機時間]
    private List<float> stepTimers;           //ゴーストごとの入力待機時間のカウント
    private List<int> nowSteps;               //ゴーストごとの現在の進んだ回数

    public List<List<bool>> isMoveSteps; //プレイヤーが移動したかどうか

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

            //stepTimersが入力待機時間を超えたら移動する
            if (stepIntervals[i][nowSteps[i]] < stepTimers[i] && isStepMax == false) {
                stepTimers[i] = 0;
                nowSteps[i]++;
                MoveNextStep(i);

            }

            //移動が最大数を超えていたら最初の位置に戻す
            if (stepIntervals[i].Count > 0 && stepIntervals[i][nowSteps[i]] < stepTimers[i]) {
                if (isStepMax == true) {
                    ResetGhost(i);
                }
            }
        }
    }

    //全てのゴーストをリセット
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

    //ゴーストの位置をリセット
    private void ResetGhost(int i) {
        ghosts[i].transform.position = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        ghosts[i].transform.rotation = Quaternion.identity;
        nowSteps[i] = 0;
        stepTimers[i] = 0;
        MoveNextStep(i);
    }


    //ゴーストの視界にプレイヤーが入っているか
    private void CheckViewPlayer() {
        for (int i = 0; i < ghostCount; ++i) {
            Vector3 distance = Player.instance.transform.position - ghosts[i].transform.position;
            Vector3 direction = Vector3.Normalize(distance);
            float len = Mathf.Pow(Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) + Mathf.Pow(distance.z, 2), 0.5f);

            //プレイヤーとの距離のベクトルと正面方向のベクトルの間の角度を求める
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

                    //壁に隠れているか判定する
                    if (obj != null) {
                        if (obj.name[0] == '3') {
                            isHideWall = true;
                            break;
                        }
                    }
                }

                if (isHideWall == false) {
                    Debug.Log("ゴースト" + i + "がプレイヤーを見つけた");
                }
            }
        }
    }


    //moveRecordsをもとに次の場所に移動する
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
