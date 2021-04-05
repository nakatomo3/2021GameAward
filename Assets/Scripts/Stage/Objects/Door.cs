using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Door : ChannelBase {

	[SerializeField]
    private Renderer render;

	//チャンネル

	[Tooltip("反転モード")]
	public bool isReverse = false;

	private int _rotate;
	//見た目の回転角。90度単位。2なら 90 * 2 = 180
	public int rotate {
		get { return _rotate; }
		set { _rotate = (value + 4) % 4; }
	}


    // Start is called before the first frame update
    void Start() {
        render = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        if (SwitchManager.instance.channel[channel] == !isReverse) {
            Color color = render.material.color;
            color.a = 0.6f;
            render.material.color = color;
        } else {
            Color color = render.material.color;
            color.a = 1.0f;
            render.material.color = color;
        }
    }

    public override string ToFileString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("B_Door");
        sb.AppendLine("pos:" + ConvertPos());
        sb.AppendLine("channel:" + channel);
		sb.AppendLine("reverse:" + isReverse);
        return sb.ToString();
    }
    public override string ToEditorString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("チャンネル：　" + channel);
		sb.AppendLine("反転モード：　" + isReverse);
		sb.AppendLine("回転(見た目)：　" + rotate * 90);
        return sb.ToString();
    }

	public override void SetData(string information) {
		var options = information.Split('\n');
		for(int i = 0; i < options.Length; i++) {
			var option = options[i];
			if (option.Contains(":")) {
				var key = option.Split(':')[0];
				var value = option.Split(':')[1];
				switch (key) {
					case "channel":
						channel = uint.Parse(value);
						break;
					case "reverse":
						if(value == "True") {
							isReverse = true;
						}
						break;
				}
			}
		}
	}
}
