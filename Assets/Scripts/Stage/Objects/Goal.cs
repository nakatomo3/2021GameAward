using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Goal : DetailBase {

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

    // Update is called once per frame
    void Update() {

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
        sb.AppendLine("フェーズ:" + phaseCount);
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
