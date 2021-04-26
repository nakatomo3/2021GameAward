using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class StartBlock : DetailBase {

	private int _phaseCount;
	public int phaseCount {
		get { return _phaseCount; }
		set {
			var count = value;
			count = Mathf.Max(0, count);
			count = Mathf.Min(9, count);
			_phaseCount = count;
		}
	}
	private int _turnMax;
	public int turnMax {
		get { return _turnMax; }
		set {
			var turn = value;
			turn = Mathf.Max(1, turn);
			turn = Mathf.Min(99, turn);
			_turnMax = turn;
		}
	}
	public bool isTutorial = false;

    public override void Action() {
        
    }
    public override string ToEditorString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("フェーズ:" + phaseCount);
		sb.AppendLine("ターン制限:" + turnMax);
		sb.AppendLine("制限なし:" + isTutorial);
		return sb.ToString();
	}

	public override string ToFileString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("I_Start");
		sb.AppendLine("pos:" + ConvertPos());
		sb.AppendLine("phaseCount:" + phaseCount);
		sb.AppendLine("turnMax:" + turnMax);
		sb.AppendLine("isTutorial:" + isTutorial);
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
					case "turnMax":
						turnMax = int.Parse(value);
						break;
					case "isTutorial":
						if(value.ToLower() == "true") {
							isTutorial = true;
						}
						break;
					default:
						break;
				}
			}
		}
		return;
	}
}
