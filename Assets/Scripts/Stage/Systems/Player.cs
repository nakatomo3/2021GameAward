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
/// �v���C���[�N���X�B���̉���PlayerMove��PlayerStatus�ȂǕʃN���X�����\��
/// </summary>
public class Player : MonoBehaviour {

    public static Player instance;


    // public Text stepText;
    [SerializeField]
    private int stepMax;
    private int stepCount;

    private float stepTimer = 0;

    // new public GameObject camera;

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

        if (Input.GetKeyDown(KeyCode.Space) || stepCount > stepMax) {
            ResetStage();
        }

        // stepText.text = "�c�莞�ԁF" + (stepMax - stepTimer).ToString("0.0");
        // camera.transform.position = new Vector3(0, 10, 0) + transform.position;
    }

    private void FixedUpdate() {
        if (isEnemyCol == true && isSafe == false) {
            ResetStage();
        }
        isEnemyCol = false;
        isSafe = false;

        for (int i = 0; i < moveRecord.Count; ++i) {

        }
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

    //���͂̑҂����Ԃ��L�^����
    void SettingStepInterval() {
        bool isMoveKey = InputManager.GetKeyDown(Keys.LEFT) ||
            InputManager.GetKeyDown(Keys.UP) ||
            InputManager.GetKeyDown(Keys.RIGHT) ||
            InputManager.GetKeyDown(Keys.DOWN);

        stepTimer += Time.deltaTime;

        //�ړ��L�[������������͑҂����Ԃ��L�^����
        if (isMoveKey == true) {
            stepTimers.Add(stepTimer);
            stepTimer = 0;
        }

    }

    void ResetStage() {
        //--�ړ������Ɠ��͑҂����Ԃ�GhostManager�ɋL�^����---//
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
        // Switch.ResetStage();
        GhostManager.instance.ResetStage();
    }

    private void OnTriggerStay(Collider other) {
        //if (other.gameObject.CompareTag("EnemyCol") || other.gameObject.CompareTag("Ghost")) {
        //    isEnemyCol = true;
        //    Debug.Log("enemy");
        //}
        //if (other.gameObject.CompareTag("Safety")) {
        //    isSafe = true;
        //    Debug.Log("safe");
        //}
    }
}
