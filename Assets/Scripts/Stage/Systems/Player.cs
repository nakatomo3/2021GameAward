using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MoveVector {
    UP,
    DOWN,
    LEFT,
    RIGHT
}


/// <summary>
/// プレイヤークラス。この下にPlayerMoveやPlayerStatusなど別クラスを作る予定
/// </summary>
public class Player : MonoBehaviour {

    public static Player instance;

    [SerializeField]
    private int stepMax;
    private int stepCount;

    private float stepTimer = 0;
	//TODO:プレイヤーの残り時間

    private List<MoveVector> moveRecord;
    private List<float> stepTimers;

    private bool isEnemyCol;
    private bool isSafe;

    private void Awake() {
        moveRecord = new List<MoveVector>();
        stepTimers = new List<float>();

        if (instance != null) {
            Destroy(instance);
        }
        instance = this;
    }

    public void Start() {

    }

    // Update is called once per frame
    void Update() {
        Move();
        SettingStepInterval();

        if (InputManager.GetKeyDown(Keys.A) || stepCount > stepMax) {
            ResetStage();
        }

    }

    private void FixedUpdate() {
        if (isEnemyCol == true && isSafe == false) {
            ResetStage();
        }
        isEnemyCol = false;
        isSafe = false;

    }


    void Move() {
        if (InputManager.GetKeyDown(Keys.LEFT)) {
            transform.position += Vector3.left;
            moveRecord.Add(MoveVector.LEFT);
            transform.localEulerAngles = Vector3.up * -90;
            stepCount++;
        }
        if (InputManager.GetKeyDown(Keys.UP)) {
            transform.position += Vector3.forward;
            moveRecord.Add(MoveVector.UP);
            transform.localEulerAngles = Vector3.up * 0;
            stepCount++;
        }
        if (InputManager.GetKeyDown(Keys.RIGHT)) {
            transform.position += Vector3.right;
            moveRecord.Add(MoveVector.RIGHT);
            transform.localEulerAngles = Vector3.up * 90;
            stepCount++;
        }
        if (InputManager.GetKeyDown(Keys.DOWN)) {
            transform.position += Vector3.back;
            moveRecord.Add(MoveVector.DOWN);
            transform.localEulerAngles = Vector3.up * 180;
            stepCount++;
        }
    }

    //入力の待ち時間を記録する
    void SettingStepInterval() {
        bool isMoveKey = InputManager.GetKeyDown(Keys.LEFT) ||
            InputManager.GetKeyDown(Keys.UP) ||
            InputManager.GetKeyDown(Keys.RIGHT) ||
            InputManager.GetKeyDown(Keys.DOWN);

        stepTimer += Time.deltaTime;

        //移動キーを押したら入力待ち時間を記録する
        if (isMoveKey == true) {
            stepTimers.Add(stepTimer);
            stepTimer = 0;
        }

    }

    void ResetStage() {
        //--移動方向と入力待ち時間をGhostManagerに記録する---//
        List<float> temp = stepTimers;
        List<MoveVector> temp2 = moveRecord;

        GhostManager.instance.stepIntervals.Add(temp);
        GhostManager.instance.moveRecords.Add(temp2);

        for (int i = 0; i < stepTimers.Count; ++i) {
            stepTimers = new List<float>();
            moveRecord = new List<MoveVector>();
        }

        GhostManager.instance.AddGhost();
        stepCount = 0;
        transform.position = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        GhostManager.instance.ResetStage();
    }

}
