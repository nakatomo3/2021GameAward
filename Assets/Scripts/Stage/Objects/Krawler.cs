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
    public int moveRangeX { // X���W�ړ��͈�
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
    public int moveRangeY { // Y���W�ړ��͈�
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
    public int interval { // �C���^�[�o��
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
    public int damage { // �_���[�W��
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
        if (krawler.transform.position == Player.instance.oldStepPos) { // �v���C���[�����܂�o�O���

        } else if (krawler.transform.position == Player.instance.transform.position) { // �������W�ɂ���Ƃ�
            Player.instance.newStepPos = Player.instance.oldStepPos; // �v���C���[��O�̍��W�ɖ߂�
            Player.instance.Damage(damage); // �_���[�W����
        }
    }

    public override void Action() { // �^�[�����ƂɌĂ΂��
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
        sb.AppendLine("X�����ړ��͈́F" + moveRangeX);
        sb.AppendLine("Y�����ړ��͈́F" + moveRangeY);
        sb.AppendLine("�C���^�[�o���F" + interval);
        sb.AppendLine("�_���[�W�ʁF" + damage);
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
