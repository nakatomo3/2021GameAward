using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Door : ChannelBase {

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	public override string ToFileString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("B_Door");
		sb.AppendLine("pos:" + Mathf.CeilToInt(transform.position.x) + "," + Mathf.CeilToInt(transform.position.z));
		sb.AppendLine("channel:" + channel);
		return sb.ToString();
	}

	public override string ToEditorString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("チャンネル：　" + channel);
		return sb.ToString();
	}
}
