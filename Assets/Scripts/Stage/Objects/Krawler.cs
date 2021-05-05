using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Krawler : DetailBase {

    public static Krawler instance;

    [SerializeField]
    private Transform krawler;

    private int krawlerTimerX;
    private int krawlerTimerY;
    private int _moveRangeX;
    public int moveRangeX { // X座標移動範囲
        get { return _moveRangeX; }
        set {
            _moveRangeX = value + 10 % 10;
            if (value < 0) {
                _moveRangeX = 0;
            }
            if (value > 10) {
                _moveRangeX = 10;
            }
        }
    }
    private int _moveRangeY;
    public int moveRangeY { // Y座標移動範囲
        get { return _moveRangeY; }
        set {
            _moveRangeY = value + 10 % 10;
            if (value < 0) {
                _moveRangeY = 0;
            }
            if (value > 10) {
                _moveRangeY = 10;
            }
        }
    }
    private int intervalTimer;
    private int _interval;
    public int interval { // インターバル
        get { return _interval; }
        set {
            _interval = value + 10 % 10;
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
            _damage = value + 10 % 10;
            if (value < 0) {
                _damage = 0;
            }
            if (value > 10) {
                _damage = 10;
            }
        }
    }
    bool isReverseX;
    bool isReverseY;
    [HideInInspector]
    public Vector3 newStepPos;

    // Start is called before the first frame update
    void Start() {
        instance = this;
        krawlerTimerX = 0;
        krawlerTimerY = 0;
        intervalTimer = 0;

        isReverseX = false;
        isReverseY = false;
    }

    // Update is called once per frame
    void Update() {
        if (krawler.transform.position == Player.instance.oldStepPos) { // プレイヤーが埋まるバグ回避

        } else if (krawler.transform.position == Player.instance.transform.position) { // 同じ座標にいるとき
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
                krawler.transform.position += new Vector3(1, 0, 0);
            } else {
                krawler.transform.position -= new Vector3(1, 0, 0);
            }
        }
        if (moveRangeY != 0) {
            krawlerTimerY++;
            if (krawlerTimerY > moveRangeY) {
                krawlerTimerY = -moveRangeY + 1;
                isReverseY = !isReverseY;
            }
            if (isReverseY == true) {
                krawler.transform.position += new Vector3(0, 0, 1);
            } else {
                krawler.transform.position -= new Vector3(0, 0, 1);
            }
        }
    }

    public override string ToFileString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("K_Krawler");
        sb.AppendLine("pos:" + ConvertPos());
        sb.AppendLine("moveRangeX:" + moveRangeX);
        sb.AppendLine("moveRangeY:" + moveRangeY);
        sb.AppendLine("interval:" + interval);
        sb.AppendLine("damage:" + damage);
        return sb.ToString();
    }

    public override string ToEditorString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("X方向移動範囲：" + moveRangeX);
        sb.AppendLine("Y方向移動範囲：" + moveRangeY);
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
                    case "moveRangeY":
                        moveRangeY = int.Parse(value);
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
