using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class Krawler : DetailBase {

    public static Krawler instance;

    [SerializeField]
    private Transform krawler;

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

    // Start is called before the first frame update
    void Start() {
        instance = this;
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
    }

    // Update is called once per frame
    void Update() {
        if (krawler.transform.position != Player.instance.oldStepPos && krawler.transform.position == Player.instance.transform.position) { // 同じ座標にいるとき
			Stage.instance.DestroyEnemy(this);
        }
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
                krawler.transform.position += Vector3.left;
            } else {
                krawler.transform.position += Vector3.right;
            }
        }
        if (moveRangeZ != 0) {
            krawlerTimerZ++;
            if (krawlerTimerZ > Math.Abs(moveRangeZ)) {
                krawlerTimerZ = -Math.Abs(moveRangeZ) + 1;
                isReverseZ = !isReverseZ;
            }
            if (isReverseZ == true) {
                krawler.transform.position += Vector3.back;
            } else {
                krawler.transform.position += Vector3.forward;
            }
        }
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
}
