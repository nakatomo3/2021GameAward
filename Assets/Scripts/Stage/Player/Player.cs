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
    private float moveIntervalMax;

    [SerializeField]
    private int stepMax;
    public int stepCount {
        get; private set;
    }

    private float stepTimer = 0;
    private float remainingTime = 20; //プレイヤーの残り時間
    private float remainingTimeMax = 10;

    private float moveIntervalTimer = 0;
    private bool canMove = true;
    private Vector3 oldStepPos;
    private Vector3 newStepPos;

    private List<MoveVector> moveRecord;
    private List<float> stepTimers;
    private List<bool> isMoveStep;

    private bool isEnemyCol;
    private bool isSafe;

    private string canStepCode = "1289ACdEIZ"; //B、F、G、Hは足場の状態が変わるので関数内部で判定

    [SerializeField]
    private Sprite[] timerNumbers;

    [SerializeField]
    private Image oneOnly;
    [SerializeField]
    private Image tenDigit;
    [SerializeField]
    private Image oneDigit;

    private Image filter;

    private void Awake() {
        moveRecord = new List<MoveVector>();
        stepTimers = new List<float>();
        isMoveStep = new List<bool>();

        if (instance != null) {
            Destroy(instance);
        }
        instance = this;
    }

    public void Start() {
        filter = Stage.instance.lastMomentFilter.GetComponent<Image>();
        oldStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        newStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
    }

    // Update is called once per frame
    void Update() {

        Move();
        SettingStepInterval();
        UpdateTimer();

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

        if (moveIntervalTimer > moveIntervalMax) {
            transform.position = newStepPos;
            oldStepPos = newStepPos;
            canMove = true;
        } else {
            //次の移動先に線形補完で移動する
            canMove = false;
            moveIntervalTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(oldStepPos, newStepPos, moveIntervalTimer / moveIntervalMax);

        }


        if (canMove == true) {
            if (InputManager.GetKeyDown(Keys.LEFT)) {
                moveRecord.Add(MoveVector.LEFT);
                transform.localEulerAngles = Vector3.up * -90;
                stepCount++;
                if (CanStep(transform.position + Vector3.left)) {
                    newStepPos = transform.position + Vector3.left;
                    isMoveStep.Add(true);
                } else {
                    isMoveStep.Add(false);
                }
                moveIntervalTimer = 0;
            }
            if (InputManager.GetKeyDown(Keys.UP)) {
                moveRecord.Add(MoveVector.UP);
                transform.localEulerAngles = Vector3.up * 0;
                stepCount++;
                if (CanStep(transform.position + Vector3.forward)) {
                    newStepPos = transform.position + Vector3.forward;
                    isMoveStep.Add(true);
                } else {
                    isMoveStep.Add(false);
                }
                moveIntervalTimer = 0;
            }
            if (InputManager.GetKeyDown(Keys.RIGHT)) {
                moveRecord.Add(MoveVector.RIGHT);
                transform.localEulerAngles = Vector3.up * 90;
                stepCount++;
                if (CanStep(transform.position + Vector3.right)) {
                    newStepPos = transform.position + Vector3.right;
                    isMoveStep.Add(true);
                } else {
                    isMoveStep.Add(false);
                }
                moveIntervalTimer = 0;
            }
            if (InputManager.GetKeyDown(Keys.DOWN)) {
                moveRecord.Add(MoveVector.DOWN);
                transform.localEulerAngles = Vector3.up * 180;
                stepCount++;
                if (CanStep(transform.position + Vector3.back)) {
                    newStepPos = transform.position + Vector3.back;
                    isMoveStep.Add(true);
                } else {
                    isMoveStep.Add(false);
                }
                moveIntervalTimer = 0;
            }
        }


        GoalCheck();
    }

    //入力の待ち時間を記録する
    void SettingStepInterval() {
        bool isMoveKey =
            InputManager.GetKeyDown(Keys.LEFT) ||
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
        stepTimers.Add(stepTimer + 5);
        List<float> temp = stepTimers;
        List<MoveVector> temp2 = moveRecord;

        GhostManager.instance.stepIntervals.Add(temp);
        GhostManager.instance.moveRecords.Add(temp2);

        for (int i = 0; i < stepTimers.Count; ++i) {
            stepTimers = new List<float>();
            moveRecord = new List<MoveVector>();
        }

        GhostManager.instance.AddGhost();
        GhostManager.instance.isMoveSteps.Add(isMoveStep);
        stepCount = 0;
        oldStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        newStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        transform.position = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        GhostManager.instance.ResetStage();

        isMoveStep = new List<bool>();
        remainingTime = remainingTimeMax;
    }

    bool CanStep(Vector3 pos) {
        var obj = Stage.instance.GetStageObject(pos);
        if (obj == null) {
            if (pos != Vector3.zero) {
                Fall();
            }
            return true; //奈落でも進めるけど落ちる
        }
        var code = Stage.GetObjectCode(obj.name);
        if (canStepCode.Contains(code.ToString())) {
            return true;
        }
        if (code == 'B') { //シャッター
            var door = obj.GetComponent<Door>();
            var channel = door.channel;
            return (SwitchManager.instance.channel[channel] ^ door.isReverse);
        }
        return false;
    }

    void UpdateTimer() {
        if (stepCount > 0) {
            remainingTime -= Time.deltaTime;
        }
        var intPart = Mathf.Floor(remainingTime); //整数部分
                                                  //小数部分を一応残しておく
                                                  //var fractionalPart = Mathf.Floor((remainingTime - intPart) * 10);
        if (remainingTime > 10) {
            var ten = Mathf.FloorToInt(remainingTime / 10);
            var one = Mathf.FloorToInt(remainingTime - ten * 10);
            tenDigit.sprite = timerNumbers[ten];
            oneDigit.sprite = timerNumbers[one];
            tenDigit.enabled = true;
            oneDigit.enabled = true;
            oneOnly.enabled = false;
        } else if (remainingTime > 0) {
            var one = Mathf.FloorToInt(remainingTime);
            oneOnly.sprite = timerNumbers[one];
            tenDigit.enabled = false;
            oneDigit.enabled = false;
            oneOnly.enabled = true;
        } else {
            oneOnly.sprite = timerNumbers[0];
        }

        if (remainingTime < 0) {
            TimeUp();
        }

        if (remainingTime < 2.8f) {
            //今の状態が透過度を下げる状態か否か
            //現在の時間が2.9秒からどれだけ離れているか、それを0.5fで割った値の整数部分が偶数→透過度を下げる
            var isDown = Mathf.FloorToInt((2.8f - remainingTime) / 0.25f) % 2 == 0;
            var alpha = (2.8f - remainingTime) / 0.25f - Mathf.Floor((2.8f - remainingTime) / 0.25f);
            if (isDown == false) {
                alpha = 1 - alpha;
            }
            filter.color = new Color(1, 1, 1, alpha);
        } else if (remainingTime < 7) {
            var isDown = Mathf.FloorToInt((7 - remainingTime) / 0.35f) % 2 == 0;
            var alpha = (7 - remainingTime) / 0.35f - Mathf.Floor((7 - remainingTime) / 0.35f);
            if (isDown == false) {
                alpha = 1 - alpha;
            }
            filter.color = new Color(1, 1, 1, alpha / 2);
        } else {
            filter.color = new Color(1, 1, 1, 0);
        }
    }

    void GoalCheck() {
        if (transform.position == Stage.instance.goalPosition) {
            Stage.instance.nowMode = Stage.Mode.CLEAR;
        }
    }

    //ダメージを受けると秒数が減る
    public void Damage(float value) {
        remainingTime -= value;
    }

    //タイムアップ演出
    private void TimeUp() {
        Stage.instance.nowMode = Stage.Mode.DEAD;
    }

    //ゴーストでゲームオーバー演出
    public void GhostGameOver() {
        Stage.instance.nowMode = Stage.Mode.DEAD;
    }

    //落ち時の演出
    public void Fall() {
        Stage.instance.nowMode = Stage.Mode.DEAD;
    }
}
