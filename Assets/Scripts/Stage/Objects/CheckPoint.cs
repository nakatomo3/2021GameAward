using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class CheckPoint : ChannelBase {

	[SerializeField]
	private GameObject switchObj;

	private float _time;
	public float time {
		get { return _time; }
		set {
			_time = value;
			_time = Mathf.Max(1.0f, _time);
			_time = Mathf.Min(99.0f, _time);
		}
	}
	private int _loopMax;
	public int loopMax {
		get { return _loopMax; }
		set {
			_loopMax = value;
			_loopMax = Mathf.Max(0, _loopMax);
		}
	}

	void Start() {
		if(time >= 0) {
			time = 20;
		}
	}

	void Update() {
		if(transform.position == Player.instance.transform.position && SwitchManager.instance.channel[channel] == false) {
			SwitchManager.instance.channel[channel] = true;
			switchObj.transform.localPosition = new Vector3(0, -0.54f, 0);
			Player.instance.CheckPoint(time, loopMax);
		}
	}

	public override string ToEditorString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("チャンネル：　" + channel);
		sb.AppendLine("時間：　" + time);
		sb.AppendLine("ループ回数：　" + loopMax);
		return sb.ToString();
	}

	public override string ToFileString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("B_Door");
		sb.AppendLine("pos:" + ConvertPos());
		sb.AppendLine("channel:" + channel);
		sb.AppendLine("time:" + time);
		sb.AppendLine("loop:" + loopMax);
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
					case "channel":
						channel = uint.Parse(value);
						break;
					case "time":
						time = float.Parse(value);
						break;
					case "loop":
						loopMax = int.Parse(value);
						break;
				}
			}
		}
	}

    public override void Action() {
        
    }
}
