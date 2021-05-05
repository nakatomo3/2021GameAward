using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Door : ChannelBase {

	[SerializeField]
    private Transform door;

	//�`�����l��

	[Tooltip("���]���[�h")]
	public bool isReverse = false;

	private int _rotate;
	//�����ڂ̉�]�p�B90�x�P�ʁB2�Ȃ� 90 * 2 = 180
	public int rotate {
		get { return _rotate; }
		set { _rotate = (value + 4) % 4; }
	}


    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (SwitchManager.instance.channel[channel] == !isReverse) {
            door.transform.localPosition = new Vector3(0, -1.0f, 0);
        } else {
            door.transform.localPosition = new Vector3(0, -0.2f, 0);
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
        sb.AppendLine("�`�����l���F�@" + channel);
		sb.AppendLine("���]���[�h�F�@" + isReverse);
		sb.AppendLine("��](������)�F�@" + rotate * 90);
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

    public override void Action() {
        
    }
}
