using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ActionRecord {
    UP,
    DOWN,
    LEFT,
    RIGHT,
    ATTACK,
    DAMAGE,
    NONE,
}



/// <summary>
/// �v���C���[�N���X�B���̉���PlayerMove��PlayerStatus�ȂǕʃN���X�����\��
/// </summary>
public class Player : MonoBehaviour {

    public static Player instance;

    public int mPhase;//m�͑����搶��m

    [SerializeField]
    private float moveIntervalMax;

    [HideInInspector]
    public bool isMoved = false; //�����n�߂���


    [HideInInspector]
    public bool canAction = true;

    public float stepTimer = 0;
    public float remainingTime = 20; //�v���C���[�̎c�莞��
    public float remainingTimeMax = 10;

    private float moveIntervalTimer = 0;

    [SerializeField]
    private float turnIntervalMax;
    private float turnIntervalTimer = 0;
    private bool canMove = true;

    [HideInInspector]
    public Vector3 oldStepPos;
    [HideInInspector]
    public Vector3 newStepPos;
    [HideInInspector]
    public List<ActionRecord> actionRecord;
    [HideInInspector]
    public List<float> stepTimers;
  
    [HideInInspector]
    public Vector3 startPosition;

    private bool isEnemyCol;
    private bool isSafe;

    private string canStepCode = "1289ACdEhIJYZ"; //B�AF�AG�AH�͑���̏�Ԃ��ς��̂Ŋ֐������Ŕ���



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
        actionRecord = new List<ActionRecord>();
        stepTimers = new List<float>();
        startPosition = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
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
        Action();
        SettingStepInterval();
        UpdateTurn();
    }

    private void FixedUpdate() {
        isEnemyCol = false;
        isSafe = false;

    }

    void Action() {

        if (moveIntervalTimer > moveIntervalMax) {
            transform.position = newStepPos;
            oldStepPos = newStepPos;
            canMove = true;
        } else {
            //���̈ړ���ɐ��`�⊮�ňړ�����
            canMove = false;
            moveIntervalTimer += Time.deltaTime;
            transform.position = Vector3.Lerp(oldStepPos, newStepPos, moveIntervalTimer / moveIntervalMax);

        }


        if (canAction == true) {
            if (InputManager.GetKey(Keys.LEFT)) {
                transform.localEulerAngles = Vector3.up * -90;
                isMoved = true;
                

                if (CanStep(transform.position + Vector3.left)) {
                    //�ړ������
                    actionRecord.Add(ActionRecord.LEFT);
                    newStepPos = transform.position + Vector3.left;
                    UseTurn();
                } else {
                    actionRecord.Add(ActionRecord.NONE);
                    UseTurn();
                }
                moveIntervalTimer = 0;


            }
            if (InputManager.GetKey(Keys.UP)) {
                
                transform.localEulerAngles = Vector3.up * 0;
                isMoved = true;
                if (CanStep(transform.position + Vector3.forward)) {
                    actionRecord.Add(ActionRecord.UP);
                    newStepPos = transform.position + Vector3.forward;
                    UseTurn();
                } else {
                    actionRecord.Add(ActionRecord.NONE);
                    UseTurn();
                }
                moveIntervalTimer = 0;


            }
            if (InputManager.GetKey(Keys.RIGHT)) {
                
                transform.localEulerAngles = Vector3.up * 90;
                isMoved = true;
                if (CanStep(transform.position + Vector3.right)) {
                    actionRecord.Add(ActionRecord.RIGHT);
                    newStepPos = transform.position + Vector3.right;
                    UseTurn();
                } else {
                    actionRecord.Add(ActionRecord.NONE);
                    UseTurn();
                }
                moveIntervalTimer = 0;


            }
            if (InputManager.GetKey(Keys.DOWN)) {
             
                transform.localEulerAngles = Vector3.up * 180;
                isMoved = true;
                if (CanStep(transform.position + Vector3.back)) {
                    actionRecord.Add(ActionRecord.DOWN);
                    newStepPos = transform.position + Vector3.back;
                    UseTurn();
                } else {
                    actionRecord.Add(ActionRecord.NONE);
                    UseTurn();
                }
                moveIntervalTimer = 0;

            }
        }


        GoalCheck();
    }

    bool CanAttack(Vector3 pos) {
       /* if (���W���G�̏ꏊ��������)*/ {
           // return true;
        }
        return false;
    }
    void Attack() {
        actionRecord.Add(ActionRecord.ATTACK);
    }

    //�v���C���[��Action()���ōs���������ɌĂ�
    void UseTurn() {
        turnIntervalTimer = 0;
        canAction = false;
        Stage.instance.Action();
        Stage.instance.AddTurn(-1);

    }

    //���͂̑҂����Ԃ��L�^����
    void SettingStepInterval() {
        bool isMoveKey =
            InputManager.GetKey(Keys.LEFT) ||
            InputManager.GetKey(Keys.UP) ||
            InputManager.GetKey(Keys.RIGHT) ||
            InputManager.GetKey(Keys.DOWN);

        stepTimer += Time.deltaTime;

        //�ړ��L�[������������͑҂����Ԃ��L�^����
        if (isMoveKey == true && canMove == true) {
            stepTimers.Add(stepTimer);
            stepTimer = 0;
        }

    }





    bool CanStep(Vector3 pos) {
        bool canStep = false;
        var obj = Stage.instance.GetStageObject(pos);
        if (obj == null) {
            if (pos != Vector3.zero) {
                Player.instance.Fall();
            }
            return true; //�ޗ��ł��i�߂邯�Ǘ�����
        }
        var code = Stage.GetObjectCode(obj.name);
        if (canStepCode.Contains(code.ToString())) {
            canStep = true;
        }
        if (code == 'B') { //�V���b�^�[
            var door = obj.GetComponent<Door>();
            var channel = door.channel;
            canStep = (SwitchManager.instance.channel[channel] ^ door.isReverse);

        } else if (code == 'F') {//�s�X�g��
            canStep = false;
        } else {
            //�s�X�g�����ˏo����Ă����Ԃ̔���
            Vector3[] pistonPos = new Vector3[8];
            pistonPos[0] = this.transform.position + Vector3.right + Vector3.forward;//�E�O
            pistonPos[1] = this.transform.position + Vector3.right + Vector3.back;//�E��
            pistonPos[2] = this.transform.position + Vector3.left + Vector3.back;//�����
            pistonPos[3] = this.transform.position + Vector3.left + Vector3.forward;//���O

            pistonPos[4] = this.transform.position + Vector3.forward * 2;//�O
            pistonPos[5] = this.transform.position + Vector3.back * 2;//���
            pistonPos[6] = this.transform.position + Vector3.right * 2;//�E
            pistonPos[7] = this.transform.position + Vector3.left * 2;//��

            for (int i = 0; i < 8; ++i) {
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

                            //�ˏo����Ă���Ƃ��͍s���Ȃ�
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

    void UpdateTurn() {
        turnIntervalTimer += Time.deltaTime;
        if (turnIntervalTimer >= 0.5f) {
            canAction = true;
        }


        if (isMoved == true) {
            // remainingTime -= Time.deltaTime;
        }
        var intPart = Mathf.Floor(Stage.instance.GetTurn()); //��������
                                                             //�����������ꉞ�c���Ă���
                                                             //var fractionalPart = Mathf.Floor((remainingTime - intPart) * 10);
        if (Stage.instance.GetTurn() >= 10) {
            var ten = Mathf.FloorToInt(Stage.instance.GetTurn() / 10);
            var one = Mathf.FloorToInt(Stage.instance.GetTurn() - ten * 10);
            tenDigit.sprite = timerNumbers[ten];
            oneDigit.sprite = timerNumbers[one];
            tenDigit.enabled = true;
            oneDigit.enabled = true;
            oneOnly.enabled = false;
        } else if (Stage.instance.GetTurn() > 0) {
            var one = Mathf.FloorToInt(Stage.instance.GetTurn());
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
            //���̏�Ԃ����ߓx���������Ԃ��ۂ�
            //���݂̎��Ԃ�2.9�b����ǂꂾ������Ă��邩�A�����0.5f�Ŋ������l�̐������������������ߓx��������
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
        // if (transform.position == Stage.instance.goalPosition) {
        GameObject obj = Stage.instance.GetStageObject(this.transform.position);
        if (obj!=null && obj.name[0] == 'J') {
            //Stage.instance.nowMode = Stage.Mode.CLEAR;
            GhostManager.instance.ResetStage();

           
           GhostManager.instance.AddGhost(Stage.instance.startBlockList[mPhase].transform.position);



            mPhase++;
            if(mPhase>= Stage.instance.startBlockList.Count) {
                Stage.instance.nowMode = Stage.Mode.CLEAR;
                return;
            }
            Enemy.imaikiteru = true;





            //--�ړ������Ɠ��͑҂����Ԃ�GhostManager�ɋL�^����---//
            stepTimers.Add(stepTimer + 5);
            List<float> temp = stepTimers;
            List<ActionRecord> temp2 = actionRecord;

            //GhostManager.instance.stepIntervals.Add(temp);
            GhostManager.instance.moveRecords.Add(temp2);

            for (int i = 0; i < stepTimers.Count; ++i) {
                stepTimers = new List<float>();
                actionRecord = new List<ActionRecord>();
            }

            //GhostManager.instance.AddGhost();
            isMoved = false;
            oldStepPos = Stage.instance.startBlockList[mPhase].transform.position;
            newStepPos = Stage.instance.startBlockList[mPhase].transform.position;
            transform.position = Stage.instance.startBlockList[mPhase].transform.position;
            GhostManager.instance.ResetStage();

        }
    }

    public void CheckPoint(float time, int _loopMax) {
        remainingTime = time;
        remainingTimeMax = time;
        startPosition = this.transform.position;

    }

    //�_���[�W���󂯂�ƕb��������
    public void Damage(float value) {
        remainingTime -= value;
    }

    //�^�C���A�b�v���o
    private void TimeUp() {
        Stage.instance.nowMode = Stage.Mode.DEAD;
    }

    //�S�[�X�g�ŃQ�[���I�[�o�[���o
    public void GhostGameOver() {
        Stage.instance.nowMode = Stage.Mode.DEAD;
    }

    //�������̉��o
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
