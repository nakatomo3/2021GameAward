using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostManager : MonoBehaviour {

    public static GhostManager instance;

    [HideInInspector]
    public List<List<MoveVector>> moveRecords;//[ゴースト番号][移動方向]

    [HideInInspector]
    public List<List<float>> stepIntervals;   //[ゴースト番号][入力待機時間]
    private List<float> stepTimers;           //ゴーストごとの入力待機時間のカウント
    private List<int> nowSteps;               //ゴーストごとの現在の進んだ回数

	[HideInInspector]
	public List<List<bool>> isMoveSteps; //プレイヤーが移動したかどうか

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
            //stepTimersが入力待機時間を超えたら移動する
            if (stepIntervals[i][nowSteps[i]] < stepTimers[i]) {
                stepTimers[i] = 0;
                MoveNextStep(i);
                nowSteps[i]++;
            }

            //最初の位置に戻す
            if (stepIntervals[i].Count > 0) {
                if (nowSteps[i] >= stepIntervals[i].Count - 1) {
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
    }

    //ゴーストの位置をリセット
    private void ResetGhost(int i) {
        ghosts[i].transform.position = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        ghosts[i].transform.rotation = Quaternion.identity;
        nowSteps[i] = 0;
        MoveNextStep(i);
    }


    //ゴーストの視界にプレイヤーが入っているか
    private void CheckViewPlayer() {
        for (int i = 0; i < ghostCount; ++i) {
            Vector3 direction = Player.instance.transform.position - ghosts[i].transform.position;
            direction = Vector3.Normalize(direction);

            //プレイヤーとの距離のベクトルと正面方向のベクトルの間の角度を求める
            float dot = Vector3.Dot(direction, ghosts[i].transform.forward);
            float angle = Mathf.Acos(dot);

            if (Mathf.Abs(dot) > 0.9 || Mathf.Abs(angle) < viewAngle / 180 * 3.14) {
                Debug.Log("ゴースト" + i + "がプレイヤーを見つけた");
            }
        }
    }


    //moveRecordsをもとに次の場所に移動する
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
