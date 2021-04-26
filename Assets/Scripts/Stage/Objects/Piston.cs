using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Piston : DetailBase {

    [SerializeField]
    private Transform piston;

    private int pistonTimer;
    private int _pistonIntervalOn;
    public int pistonIntervalOn { // ピストンオン間隔
        get { return _pistonIntervalOn; }
        set {
            _pistonIntervalOn = value + 10 % 10;
            if (value < 1) {
                _pistonIntervalOn = 1;
            }
            if (value > 10) {
                _pistonIntervalOn = 10;
            }
        }
    }
    private int _pistonIntervalOff;
    public int pistonIntervalOff { // ピストンオフ間隔
        get { return _pistonIntervalOff; }
        set {
            _pistonIntervalOff = value + 10 % 10;
            if (value < 1) {
                _pistonIntervalOff = 1;
            }
            if (value > 10) {
                _pistonIntervalOff = 10;
            }
        }
    }
    private int _delay;
    public int delay { // 遅延
        get { return _delay; }
        set {
            _delay = value + 10 % 10;
            if (value < 0) {
                _delay = 0;
            }
            if (value > 10) {
                _delay = 10;
            }
        }
    }
    public enum Direction { // 向き
        Front,
        Back,
        Right,
        Left
    }
    private int directionMax = 4; // 方向最大値
    private Direction _direction;
    public Direction direction {
        get { return _direction; }
        set { _direction = (Direction)(((int)value + directionMax) % directionMax); }
    }
    private Vector3 targetPosition;
    private Vector3 addForce;
    [SerializeField]
    public bool isPush { get; private set; }

    // Start is called before the first frame update
    void Start() {
        isPush = false;
        pistonTimer = -delay;
        if (pistonIntervalOn == 0) {
            pistonIntervalOn = 5;
        }
        if (pistonIntervalOff == 0) {
            pistonIntervalOff = 5;
        }
    }

    // Update is called once per frame
    private void Update() {
        if (isPush == true) {
            piston.transform.localPosition = new Vector3(1.0f, 0, 0);
        } else {
            piston.transform.localPosition = new Vector3(0.1f, 0, 0);
        }

        switch (direction) {
            case Direction.Front:
                targetPosition = transform.position + new Vector3(0, 0, -1);
                addForce = new Vector3(0, 0, -1);
                transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            case Direction.Back:
                targetPosition = transform.position + new Vector3(0, 0, 1);
                addForce = new Vector3(0, 0, 1);
                transform.eulerAngles = new Vector3(0, 270, 0);
                break;
            case Direction.Right:
                targetPosition = transform.position + new Vector3(1, 0, 0);
                addForce = new Vector3(1, 0, 0);
                transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case Direction.Left:
                targetPosition = transform.position + new Vector3(-1, 0, 0);
                addForce = new Vector3(-1, 0, 0);
                transform.eulerAngles = new Vector3(0, 180, 0);
                break;
        }
        if (InputManager.GetKeyDown(Keys.LEFT)) {
            Action();
        }
        if (InputManager.GetKeyDown(Keys.RIGHT)) {
            Action();
        }
        if (InputManager.GetKeyDown(Keys.UP)) {
            Action();
        }
        if (InputManager.GetKeyDown(Keys.DOWN)) {
            Action();
        }
    }

    public override void Action() {
        pistonTimer++;
        if (isPush == false) {
            if (pistonTimer >= pistonIntervalOff) {
                if (Player.instance.transform.position == targetPosition && isPush == false) {
                    Player.instance.newStepPos += addForce; // プレイヤーを押し出す
                    if (Stage.instance.GetStageObject(transform.position + addForce * 2) == null) { // 押し出した先が奈落なら死
                        Player.instance.Fall();
                    }
                }
                pistonTimer = 0;
                isPush = true;
            }
        } else {
            if (pistonTimer >= pistonIntervalOn) {
                pistonTimer = 0;
                isPush = false;
            }
        }
    }


    public override string ToFileString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("F_Piston");
        sb.AppendLine("pos:" + ConvertPos());
        sb.AppendLine("intervalOn:" + pistonIntervalOn);
        sb.AppendLine("intervalOff:" + pistonIntervalOff);
        sb.AppendLine("delay:" + delay);
        sb.AppendLine("direction:" + (int)direction);
        return sb.ToString();
    }

    public override string ToEditorString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("インターバル凸：" + pistonIntervalOn);
        sb.AppendLine("インターバル凹：" + pistonIntervalOff);
        sb.AppendLine("遅延：　" + delay);
        sb.AppendLine("向き：　" + direction);
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
                    case "intervalOn":
                        pistonIntervalOn = int.Parse(value);
                        break;
                    case "intervalOff":
                        pistonIntervalOff = int.Parse(value);
                        break;
                    case "delay":
                        delay = int.Parse(value);
                        break;
                    case "direction":
                        direction = (Direction)int.Parse(value);
                        break;
                }
            }
        }
    }
}
