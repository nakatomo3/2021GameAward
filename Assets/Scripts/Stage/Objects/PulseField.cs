using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class PulseField : DetailBase {

    [SerializeField]
    private Renderer render;

    private float pulseTimer;
	private float _modeInterval;
    public float modeInterval { // モード切替間隔
		get { return _modeInterval; }
		set {
			_modeInterval = Mathf.Round(value * 10) / 10;
			if(value < 0.1f) {
				_modeInterval = 0.1f;
			}
			if(value > 10) {
				_modeInterval = 10;
			}
		}
	}
	private float _delay;
    public float delay { // 遅延
		get { return _delay; }
		set {
			_delay = Mathf.Round(value * 10) / 10;
			if (value < 0.0f) {
				_delay = 0.0f;
			}
			if (value > 10) {
				_delay = 10;
			}
		}
	}
    private bool isPulse = false;

    private bool isDamage = true;
    private float timer = 0.0f;
    private float damageInterval = 1.0f;

    // Start is called before the first frame update
    void Start() {
        render = GetComponentInChildren<Renderer>();
        pulseTimer = -delay;
		if(modeInterval == 0) {
			modeInterval = 2.0f;
		}
    }

    // Update is called once per frame
    void Update() {
        pulseTimer += Time.deltaTime;

        if (pulseTimer >= modeInterval) {
            pulseTimer = 0;
            isPulse = !isPulse; // モードを切り替える
        }

        if (isPulse == true) { // パルスONの時はダメージブロックと同等の処理
            Color color = render.material.color;
            color.a = 1.0f;
            render.material.color = color;
            if (transform.position == Player.instance.transform.position) {
                if (isDamage == false) {
                    timer += Time.deltaTime;
                    if (timer >= damageInterval) { // インターバルカウント
                        timer = 0.0f;
                        isDamage = true;
                        color = render.material.color;
                        color.g = 1.0f;
                        render.material.color = color;
                    }
                } else {
                    Player.instance.Damage(1); // ダメージ処理
                    isDamage = false;
                    color = render.material.color;
                    color.g = 0.0f;
                    render.material.color = color;
                }
            } else { // プレイヤーが離れたらリセット
                timer = 0.0f;
                isDamage = true;
                color = render.material.color;
                color.g = 1.0f;
                render.material.color = color;
            }
        } else {
            Color color = render.material.color;
            color.a = 0.2f;
            render.material.color = color;
        }
    }
	
	public override string ToFileString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("E_PulseField");
		sb.AppendLine("pos:" + ConvertPos());
		sb.AppendLine("interval:" + modeInterval);
		sb.AppendLine("delay:" + delay);
		return sb.ToString();
	}

	public override string ToEditorString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("インターバル：　" + modeInterval);
		sb.AppendLine("遅延：　" + delay);
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
					case "interval":
						modeInterval = float.Parse(value);
						break;
					case "delay":
						delay = float.Parse(value);
						break;
				}
			}
		}
	}

    public override void Action() {
        
    }
}