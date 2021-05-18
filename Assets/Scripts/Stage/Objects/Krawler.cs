using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class Krawler : DetailBase {

    public static Krawler instance;

    [SerializeField]
    private Transform krawler;

    private Animator animator;
    private float destroyTimer = 0;

    private Vector3 defaultPos;
    private Vector3 newStepPos;
    private Vector3 oldStepPos;
    private float moveTimer = 0;
    private float moveTimerMax = 0.2f;

    private int krawlerTimerX;
    private int krawlerTimerZ;
    private int _moveRangeX;
    public int moveRangeX { // X座標移動範囲
        get { return _moveRangeX; }
        set {
            _moveRangeX = value;
            _moveRangeX = Mathf.Max(-10, _moveRangeX);
            _moveRangeX = Mathf.Min(10, _moveRangeX);
        }
    }
    private int _moveRangeZ;
    public int moveRangeZ { // Y座標移動範囲
        get { return _moveRangeZ; }
        set {
            _moveRangeZ = value;
            _moveRangeZ = Mathf.Max(-10, _moveRangeZ);
            _moveRangeZ = Mathf.Min(10, _moveRangeZ);
        }
    }
    private int intervalTimer;
    private int _interval;
    public int interval { // インターバル
        get { return _interval; }
        set {
            _interval = value;
            _interval = Mathf.Max(0, _interval);
            _interval = Mathf.Min(10, _interval);
        }
    }
    private int _damage;
    public int damage { // ダメージ量
        get { return _damage; }
        set {
            _damage = value;
            _damage = Mathf.Max(0, _damage);
            _damage = Mathf.Min(10, _damage);
        }
    }
    private int _phaseCount;
    public int phaseCount {
        get { return _phaseCount; }
        set {
            var count = value;
            count = Mathf.Max(0, count);
            count = Mathf.Min(127, count);
            _phaseCount = count;
        }
    }
    bool isReverseX;
    bool isReverseZ;

    public bool isDie = false;

    // Start is called before the first frame update
    void Start() {
        instance = this;
        krawler = GetComponent<Transform>();
        animator = transform.GetChild(0).GetComponent<Animator>();

        krawlerTimerX = 0;
        krawlerTimerZ = 0;
        intervalTimer = 0;

        isReverseX = false;
        isReverseZ = false;

        if (moveRangeX < 0) {
            isReverseX = true;
        }
        if (moveRangeZ < 0) {
            isReverseZ = true;
        }

        newStepPos = transform.GetChild(0).transform.localPosition;
        oldStepPos = transform.GetChild(0).transform.localPosition;
        defaultPos= transform.GetChild(0).transform.localPosition;
    }

    // Update is called once per frame
    void Update() {
        //フェーズによって消えたりするやつ(^_-)
        if (Stage.instance.isEditorMode == true) {
            transform.GetChild(0).gameObject.SetActive((phaseCount & (int)Mathf.Pow(2, StageEditor.editorPhase)) > 0 || StageEditor.editorPhase == 7);
            ResetEnemy();
            isDie = false;
        } else {
            if (isDie == true) {
                destroyTimer += Time.deltaTime;
                if (destroyTimer > 5) {
                    transform.GetChild(0).gameObject.SetActive(false);
                    ResetEnemy();
                }

            } else if (isDie == false) {
                transform.GetChild(0).gameObject.SetActive((phaseCount & (int)Mathf.Pow(2, Player.instance.phase)) > 0);

                Vector3 pos = Vector3.Lerp(oldStepPos, newStepPos, moveTimer / moveTimerMax);
                pos.y += Mathf.Sin((moveTimer / moveTimerMax) * 3.14f) * 0.5f;
                transform.GetChild(0).transform.localPosition = pos;

                if (moveTimer / moveTimerMax < 1) {
                    moveTimer += Time.deltaTime;
                    animator.SetBool("isMove", true);
                    animator.SetBool("isIdel", false);
                } else {
                    moveTimer = moveTimerMax;
                    animator.SetBool("isMove", false);
                    animator.SetBool("isIdel", true);
                }
            }
        }
        animator.SetBool("isDie", isDie);
    }

    public override void Action() { // ターンごとに呼ばれる
        intervalTimer++;
        if (intervalTimer <= interval) {
            return;
        }
        intervalTimer = 0;
        if (moveRangeX != 0) {
            krawlerTimerX++;
            if (krawlerTimerX > Math.Abs(moveRangeX)) {
                krawlerTimerX = -Math.Abs(moveRangeX) + 1;
                isReverseX = !isReverseX;
            }
            if (isReverseX == true) {
                oldStepPos = newStepPos;
                newStepPos += Vector3.left;
                transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 90, 0);
                moveTimer = 0;
            } else {
                oldStepPos = newStepPos;
                newStepPos += Vector3.right;
                transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 270, 0);
                moveTimer = 0;
            }
        }
        if (moveRangeZ != 0) {
            krawlerTimerZ++;
            if (krawlerTimerZ > Math.Abs(moveRangeZ)) {
                krawlerTimerZ = -Math.Abs(moveRangeZ) + 1;
                isReverseZ = !isReverseZ;
            }
            if (isReverseZ == true) {
                oldStepPos = newStepPos;
                newStepPos += Vector3.back;
                transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 0, 0);
                moveTimer = 0;
            } else {
                oldStepPos = newStepPos;
                newStepPos += Vector3.forward;
                transform.GetChild(0).transform.localEulerAngles = new Vector3(0, 180, 0);
                moveTimer = 0;
            }
        }
        Stage.instance.DestroyEnemy();
    }

    public override string ToFileString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("K_Krawler");
        sb.AppendLine("pos:" + ConvertPos());
        sb.AppendLine("moveRangeX:" + moveRangeX);
        sb.AppendLine("moveRangeZ:" + moveRangeZ);
        sb.AppendLine("interval:" + interval);
        sb.AppendLine("damage:" + damage);
        sb.AppendLine("phaseCount:" + phaseCount);
        return sb.ToString();
    }

    public override string ToEditorString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("X方向移動範囲：" + moveRangeX);
        sb.AppendLine("Z方向移動範囲：" + moveRangeZ);
        sb.AppendLine("インターバル：" + interval);
        sb.AppendLine("ダメージ量：" + damage);
        var tmp = phaseCount;
        var s = "";
        int count = 1;
        while (tmp > 0) {
            if ((tmp & 1) > 0) {
                s += count.ToString() + ",";
            }
            tmp >>= 1;
            count++;
        }
        if (s == "") {
            s = "エラー";
        }
        sb.AppendLine("フェーズ:" + s);
        return sb.ToString();
    }

    public override void SetData(string information) {
        var options = information.Split('\n');
        for (int i = 0; i < options.Length; i++) {
            var option = options[i];
            if (option.Contains(":")) {
                var key = option.Split(':')[0];
                var value = option.Split(':')[1];
                switch (key) {
                    case "moveRangeX":
                        moveRangeX = int.Parse(value);
                        break;
                    case "moveRangeZ":
                        moveRangeZ = int.Parse(value);
                        break;
                    case "interval":
                        interval = int.Parse(value);
                        break;
                    case "damage":
                        damage = int.Parse(value);
                        break;
                    case "phaseCount":
                        phaseCount = int.Parse(value);
                        break;
                }
            }
        }
    }

    public void ResetEnemy() {
        moveTimer = 0;
        krawlerTimerX = 0;
        krawlerTimerZ = 0;
        intervalTimer = 0;
        destroyTimer = 0;

        newStepPos = defaultPos;
        oldStepPos = defaultPos;
        transform.GetChild(0).transform.position = defaultPos;

    }
}
