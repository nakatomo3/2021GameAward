using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Switch : ChannelBase {

	[SerializeField]
	private GameObject switchOn;
	[SerializeField]
	private GameObject switchOff;

	private bool isUnderPlayer;

	// Start is called before the first frame update
	void Start() {
		switchOn.SetActive(false);
		switchOff.SetActive(true);
	}

	// Update is called once per frame
	void Update() {
		switchOn.SetActive(SwitchManager.instance.channel[channel]);
		switchOff.SetActive(!SwitchManager.instance.channel[channel]);

		if (this.transform.position == Player.instance.transform.position && isUnderPlayer == false) {
			SwitchManager.instance.channel[channel] = !SwitchManager.instance.channel[channel];
			isUnderPlayer = true;
		} else if (transform.position != Player.instance.transform.position) {
			isUnderPlayer = false;
		}

	}

	public override string ToFileString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("A_Switch");
		sb.AppendLine("pos:" + Mathf.CeilToInt(transform.position.x) + "," + Mathf.CeilToInt(transform.position.z));
		sb.AppendLine("channel:" + channel);
		return sb.ToString();
	}

	public override string ToEditorString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("チャンネル：　" + channel);
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
		return;
	}

	public override void Action() {

	}
}
