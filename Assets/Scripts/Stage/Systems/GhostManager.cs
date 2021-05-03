using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostManager : MonoBehaviour {
    public static GhostManager instance;

    [HideInInspector]
    public List<List<ActionRecord>> moveRecords;//[ゴースト番号][移動方向]
    private List<int> nowSteps;               //ゴーストごとの現在の進んだ回数

    private List<Vector3> startPositions;

    private List<Vector3> newPos;
    private List<Vector3> oldPos;

    private float moveStepRate = 0;//移動の線形補完で使う

    private int ghostCount = 0;
    public GameObject ghost;
    public List<GameObject> ghosts;

    void Start() {
        instance = this;
        moveRecords = new List<List<ActionRecord>>();
        nowSteps = new List<int>();
        startPositions = new List<Vector3>();

        newPos = new List<Vector3>();
        oldPos = new List<Vector3>();
    }


    void Update() {
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

    public void Action() {
        for (int i = 0; i < ghosts.Count; i++) {
            //最大ステップになったら移動しない
            if (nowSteps[i] < moveRecords[i].Count) {
                MoveNextStep(i);
                nowSteps[i]++;
            }


            if (Stage.instance.GetStageObject(ghosts[i].transform.position).name[0] == 'J') {
                ResetGhost(i);
            }
        }
    }

    //全てのゴーストをリセット
    public void ResetStage() {
        for (int i = 0; i < ghostCount; i++) {
            ghosts[i].transform.position = startPositions[i];
            newPos[i] = startPositions[i];
            oldPos[i] = startPositions[i];
            nowSteps[i] = 0;
        }
    }

    //ゴーストの位置をリセット
    private void ResetGhost(int i) {
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
        ghostCount = 0;
    }


    //moveRecordsをもとに次の場所に移動する
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
                //攻撃モーション
                break;

            case ActionRecord.DAMAGE:
                //ダメージモーション
                break;
        }
    }


    public void AddGhost(Vector3 startPos) {
        ghosts.Add(Instantiate(ghost, startPos, Quaternion.identity));
        ghostCount++;
        nowSteps.Add(0);
        startPositions.Add(startPos);
        oldPos.Add(startPos);
        newPos.Add(startPos);

        oldPos[startPositions.Count - 1] = startPos;
        newPos[startPositions.Count - 1] = startPos;
    }
}
