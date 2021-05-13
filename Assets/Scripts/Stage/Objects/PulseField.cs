using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class PulseField : DetailBase {

    [SerializeField]
    private Transform spike;

    private int pulseTimer;
    private int _modeIntervalOn;
    public int modeIntervalOn { // ���[�h�I���Ԋu
        get { return _modeIntervalOn; }
        set {
            _modeIntervalOn = value + 10 % 10;
            if (value < 1) {
                _modeIntervalOn = 1;
            }
            if (value > 99) {
                _modeIntervalOn = 99;
            }
        }
    }
    private int _modeIntervalOff;
    public int modeIntervalOff { // ���[�h�I�t�Ԋu
        get { return _modeIntervalOff; }
        set {
            _modeIntervalOff = value + 10 % 10;
            if (value < 1) {
                _modeIntervalOff = 1;
            }
            if (value > 99) {
                _modeIntervalOff = 99;
            }
        }
    }
    private int _delay;
    public int delay { // �x��
        get { return _delay; }
        set {
            _delay = value + 10 % 10;
            if (value < 0) {
                _delay = 0;
            }
            if (value > 99) {
                _delay = 99;
            }
        }
    }
    private bool isPulse = true;
    private bool isDamage = true;

    [SerializeField]
    private Text pulseCount;
    private int pCount;

    // Start is called before the first frame update
    void Start() {
        pulseTimer = -delay;
        if (modeIntervalOn == 0) {
            modeIntervalOn = 2;
        }
        if (modeIntervalOff == 0) {
            modeIntervalOff = 2;
        }
        pCount = modeIntervalOff;
    }

    // Update is called once per frame
    void Update() {
        if (isPulse == true) {
            pCount = modeIntervalOn - pulseTimer;
        } else {
            pCount = modeIntervalOff - pulseTimer;
        }
        pulseCount.text = "" + pCount;

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

    public override void Action() { // �^�[�����ƂɌĂ΂��
        pulseTimer++;
        if (isPulse == true) { // �p���X�I��
            if (pulseTimer >= modeIntervalOn) {
                pulseTimer = 0;
                isPulse = !isPulse;
                spike.transform.position +=  new Vector3(0, 0.8f, 0);
                if (transform.position == Player.instance.transform.position) {
                    if (isDamage == false) {
                        Player.instance.Damage(1); // �_���[�W����
                        isDamage = false;
                    }
                } else { // �v���C���[�����ꂽ�烊�Z�b�g
                    isDamage = true;
                }
            }
        } else { // �p���X�I�t
            if (pulseTimer >= modeIntervalOff) {
                spike.transform.position -= new Vector3(0, 0.8f, 0);
                pulseTimer = 0;
                isPulse = !isPulse;
            }
        }
    }

    public override string ToFileString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("E_PulseField");
        sb.AppendLine("pos:" + ConvertPos());
        sb.AppendLine("intervalOn:" + modeIntervalOn);
        sb.AppendLine("intervalOff:" + modeIntervalOff);
        sb.AppendLine("delay:" + delay);
        return sb.ToString();
    }

    public override string ToEditorString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("�C���^�[�o���I���F" + modeIntervalOn);
        sb.AppendLine("�C���^�[�o���I�t�F" + modeIntervalOff);
        sb.AppendLine("�x���F�@" + delay);
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
                        modeIntervalOn = int.Parse(value);
                        break;
                    case "intervalOff":
                        modeIntervalOff = int.Parse(value);
                        break;
                    case "delay":
                        delay = int.Parse(value);
                        break;
                }
            }
        }
    }
}