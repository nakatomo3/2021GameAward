using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

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
            _moveRangeX = value + (10 % 10);
            if (value < 0) {
                _moveRangeX = 0;
            }
            if (value > 10) {
                _moveRangeX = 10;
            }
        }
    }
    private int _moveRangeZ;
    public int moveRangeZ { // Y座標移動範囲
        get { return _moveRangeZ; }
        set {
            _moveRangeZ = value + (10 % 10);
            if (value < 0) {
                _moveRangeZ = 0;
            }
            if (value > 10) {
                _moveRangeZ = 10;
            }
        }
    }
    private int intervalTimer;
    private int _interval;
    public int interval { // インターバル
        get { return _interval; }
        set {
            _interval = value + (10 % 10);
            if (value < 0) {
                _interval = 0;
            }
            if (value > 10) {
                _interval = 10;
            }
        }
    }
    private int _damage;
    public int damage { // ダメージ量
        get { return _damage; }
        set {
            _damage = value + (10 % 10);
            if (value < 0) {
                _damage = 0;
            }
            if (value > 10) {
                _damage = 10;
            }
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
    }

    // Update is called once per frame
    void Update() {
        if (krawler.transform.position != Player.instance.oldStepPos && krawler.transform.position == Player.instance.transform.position) { // 同じ座標にいるとき
            Player.instance.newStepPos = Player.instance.oldStepPos; // プレイヤーを前の座標に戻す
            Player.instance.Damage(damage); // ダメージ処理
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
            if (krawlerTimerX > moveRangeX) {
                krawlerTimerX = -moveRangeX + 1;
                isReverseX = !isReverseX;
            }
            if (isReverseX == true) {
                krawler.transform.position += Vector3.right;
            } else {
                krawler.transform.position += Vector3.left;
            }
        }
        if (moveRangeZ != 0) {
            krawlerTimerZ++;
            if (krawlerTimerZ > moveRangeZ) {
                krawlerTimerZ = -moveRangeZ + 1;
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
        return sb.ToString();
    }

    public override string ToEditorString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("X方向移動範囲：" + moveRangeX);
        sb.AppendLine("Z方向移動範囲：" + moveRangeZ);
        sb.AppendLine("インターバル：" + interval);
        sb.AppendLine("ダメージ量：" + damage);
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
                }
            }
        }
    }
}
