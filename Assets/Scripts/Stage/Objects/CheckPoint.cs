using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class CheckPoint : ChannelBase {

	[SerializeField]
	private GameObject switchObj;

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {
		if(transform.position == Player.instance.transform.position) {
			SwitchManager.instance.channel[channel] = true;
		}
		if(SwitchManager.instance.channel[channel] == true) {
			switchObj.transform.localPosition = new Vector3(0, -0.54f, 0);
		}
	}

	public override string ToEditorString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("チャンネル：　" + channel);
		sb.AppendLine("時間：　編集不可");
		sb.AppendLine("ループ回数：　編集不可");
		return sb.ToString();
	}

	public override string ToFileString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("B_Door");
		sb.AppendLine("pos:" + ConvertPos());
		sb.AppendLine("channel:" + channel);
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
				}
			}
		}
	}
}
