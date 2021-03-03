using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Switch : ChannelBase {

	[SerializeField]
	private Renderer render;

    private bool isUnderPlayer;

	// Start is called before the first frame update
	void Start() {
        render = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        if (this.transform.position == Player.instance.transform.position && isUnderPlayer == false) {
            if (SwitchManager.instance.channel[channel] == true) {
                SwitchManager.instance.channel[channel] = false;
                Color color = render.material.color;
                color.a = 1.0f;
                render.material.color = color;
            } else {
                SwitchManager.instance.channel[channel] = true;
                Color color = render.material.color;
                color.a = 0.6f;
                render.material.color = color;
            }
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
}
