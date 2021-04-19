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

    [HideInInspector]
    public PlayerSkill skill;

    [SerializeField]
    private float moveIntervalMax;

    [HideInInspector]
    public bool isMoved = false;


    public float stepTimer = 0;
    public float remainingTime = 20; //プレイヤーの残り時間
    public float remainingTimeMax = 10;

    private float moveIntervalTimer = 0;
    private bool canMove = true;

    [HideInInspector]
    public Vector3 oldStepPos;
    [HideInInspector]
    public Vector3 newStepPos;
    [HideInInspector]
    public List<MoveVector> moveRecord;
    [HideInInspector]
    public List<float> stepTimers;
    [HideInInspector]
    public List<bool> isMoveStep;
    [HideInInspector]
    public Vector3 startPosition;

    private bool isEnemyCol;
    private bool isSafe;

    private string canStepCode = "1289ACdEhIYZ"; //B、F、G、Hは足場の状態が変わるので関数内部で判定



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
        startPosition = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        if (instance != null) {
            Destroy(instance);
        }
        instance = this;
    }

    public void Start() {
        skill = GetComponent<PlayerSkill>();
        filter = Stage.instance.lastMomentFilter.GetComponent<Image>();
        oldStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
        newStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
    }

    // Update is called once per frame
    void Update() {

        Move();
        SettingStepInterval();
        UpdateTimer();



    }

    private void FixedUpdate() {
        if (isEnemyCol == true && isSafe == false) {
            skill.ResetStage();
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
            if (InputManager.GetKey(Keys.LEFT)) {
                moveRecord.Add(MoveVector.LEFT);
                transform.localEulerAngles = Vector3.up * -90;
                isMoved = true;
                if (CanStep(transform.position + Vector3.left)) {
                    newStepPos = transform.position + Vector3.left;
                    isMoveStep.Add(true);
                } else {
                    isMoveStep.Add(false);
                }
                moveIntervalTimer = 0;
            }
            if (InputManager.GetKey(Keys.UP)) {
                moveRecord.Add(MoveVector.UP);
                transform.localEulerAngles = Vector3.up * 0;
                isMoved = true;
                if (CanStep(transform.position + Vector3.forward)) {
                    newStepPos = transform.position + Vector3.forward;
                    isMoveStep.Add(true);
                } else {
                    isMoveStep.Add(false);
                }
                moveIntervalTimer = 0;
            }
            if (InputManager.GetKey(Keys.RIGHT)) {
                moveRecord.Add(MoveVector.RIGHT);
                transform.localEulerAngles = Vector3.up * 90;
                isMoved = true;
                if (CanStep(transform.position + Vector3.right)) {
                    newStepPos = transform.position + Vector3.right;
                    isMoveStep.Add(true);
                } else {
                    isMoveStep.Add(false);
                }
                moveIntervalTimer = 0;
            }
            if (InputManager.GetKey(Keys.DOWN)) {
                moveRecord.Add(MoveVector.DOWN);
                transform.localEulerAngles = Vector3.up * 180;
                isMoved = true;
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
            InputManager.GetKey(Keys.LEFT) ||
            InputManager.GetKey(Keys.UP) ||
            InputManager.GetKey(Keys.RIGHT) ||
            InputManager.GetKey(Keys.DOWN);

        stepTimer += Time.deltaTime;

        //移動キーを押したら入力待ち時間を記録する
        if (isMoveKey == true && canMove == true) {
            stepTimers.Add(stepTimer);
            stepTimer = 0;
        }

    }





    bool CanStep(Vector3 pos) {
        bool canStep = true;
        var obj = Stage.instance.GetStageObject(pos);
        if (obj == null) {
            if (pos != Vector3.zero) {
                Fall();
            }
            return true; //奈落でも進めるけど落ちる
        }
        var code = Stage.GetObjectCode(obj.name);
        if (canStepCode.Contains(code.ToString())) {
            canStep = true;
        }
        if (code == 'B') { //シャッター
            var door = obj.GetComponent<Door>();
            var channel = door.channel;
            canStep = (SwitchManager.instance.channel[channel] ^ door.isReverse);

        } else if (code == 'F') {//ピストン
            canStep = false;
        } else {

            //ピストンが射出されている状態の判定
            Vector3[] pistonPos = new Vector3[4];
            pistonPos[0] = this.transform.position + Vector3.right + Vector3.forward;//右前
            pistonPos[1] = this.transform.position + Vector3.right + Vector3.back;//右下
            pistonPos[2] = this.transform.position + Vector3.left + Vector3.back;//左後ろ
            pistonPos[3] = this.transform.position + Vector3.left + Vector3.forward;//左前
            Debug.Log("kjdbflsjdfb;b");
            for (int i = 0; i < 4; ++i) {
                GameObject g = Stage.instance.GetStageObject(pistonPos[i]);
                if (g != null) {
                    if (g.name[0] == 'F') {
                        Piston piston = g.GetComponent<Piston>();

                        if (piston != null && piston.isPush == true) {
                            Vector3 addPos = Vector3.zero;

                            switch (piston.direction) {
                                case Piston.Direction.Front:
                                    addPos = Vector3.forward;
                                    break;

                                case Piston.Direction.Back:
                                    addPos = Vector3.back;
                                    break;

                                case Piston.Direction.Left:
                                    addPos = Vector3.left;
                                    break;

                                case Piston.Direction.Right:
                                    addPos = Vector3.right;
                                    break;
                            }

                            //射出されているとこは行けない
                            if (pos == pistonPos[i] + addPos) {
                                canStep = false;
                            }
                        }
                    }
                }
            }
        }

        return canStep;
    }

    void UpdateTimer() {
        if (isMoved == true) {
            remainingTime -= Time.deltaTime;
        }
        var intPart = Mathf.Floor(remainingTime); //整数部分
                                                  //小数部分を一応残しておく
                                                  //var fractionalPart = Mathf.Floor((remainingTime - intPart) * 10);
        if (remainingTime >= 10) {
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

    public void CheckPoint(float time, int _loopMax) {
        remainingTime = time;
        remainingTimeMax = time;
        skill.skillMax[(int)Skill.LOOP] = _loopMax;
        skill.skillNum[(int)Skill.LOOP] = skill.skillMax[(int)Skill.LOOP];
        startPosition = this.transform.position;

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

    public void SetPosition(Vector3 pos) {
        this.transform.position = pos;
        newStepPos = pos;
        oldStepPos = pos;
    }
    public void AddPosition(Vector3 pos) {
        this.transform.position += pos;
        newStepPos += pos;
        oldStepPos += pos;
    }
}
