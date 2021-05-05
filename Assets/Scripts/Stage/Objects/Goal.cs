using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Goal : DetailBase {

	[Disable]
	[SerializeField]
	private GameObject goalObject;

	[Disable]
	[SerializeField]
	private GameObject core;

	private int _phaseCount;
	public int phaseCount {
		get { return _phaseCount; }
		set {
			var count = value;
			count = Mathf.Max(0, count);
			count = Mathf.Min(127, count);
			_phaseCount = count;
		}
	}

	private float time;

	// Update is called once per frame
	void Update() {
		goalObject.SetActive((phaseCount & (int)Mathf.Pow(2, Player.instance.phase)) > 0);
		if (Stage.instance.isEditorMode == true) {
			goalObject.SetActive((phaseCount & (int)Mathf.Pow(2, StageEditor.editorPhase)) > 0 || StageEditor.editorPhase == 7);
		}

		time += Time.deltaTime;
		core.transform.localPosition = Vector3.up * (Mathf.Sin(time) * 0.125f + 0.1f);
		core.transform.localEulerAngles = Vector3.up * time * 180;
	}

	public override void Action() {

	}

	public override string ToFileString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("J_Goal");
		sb.AppendLine("pos:" + ConvertPos());
		sb.AppendLine("phaseCount:" + phaseCount);
		return sb.ToString();
	}

	public override string ToEditorString() {
		StringBuilder sb = new StringBuilder();
		var tmp = phaseCount;
		var s = "";
		int count = 1;
		while (tmp > 0) {
			if ((tmp & 1) > 0) {
				s += count.ToString() + ",";
			}
			tmp >>= 1;
			count++;
		}
		if (s == "") {
			s = "エラー";
		}
		sb.AppendLine("フェーズ:" + s);
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
					case "phaseCount":
						phaseCount = int.Parse(value);
						break;
				}
			}
		}
	}
}
