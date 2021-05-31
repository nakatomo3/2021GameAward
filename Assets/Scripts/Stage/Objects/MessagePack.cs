using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class MessagePack : DetailBase {

    #region data
    private int _index;
    public int index {
        get { return _index; }
        set {
            var num = value;
            num = (num + images.Count) % images.Count;
            _index = num;
        }
    }

    //入れる
    public List<Sprite> images;

    #endregion

    private float timer = 0;
    private const float maxTime = 0.3f;

    private static Image window;

    private bool isOnPlayer;

    private void Start() {
        if (window == null) {
            window = GameObject.Find("MessagePackWindow").GetComponent<Image>();
        }
    }

    public override void Action() {

    }

    void Update() {
        if (transform.position == Player.instance.newStepPos) {
            isOnPlayer = true;
            window.sprite = images[index];
        } else {
            isOnPlayer = false;
        }

        if (isOnPlayer) {
            timer += Time.deltaTime;
            window.color = Color.white;
        }
        if (timer > 0 && isOnPlayer == false) { //離れたらフェードアウト
            timer -= Time.deltaTime;
        }
        if (timer >= 1) {
            timer = 1;
        }
        if (window == null) {
            window = GameObject.Find("MessagePackWindow").GetComponent<Image>();
        } else {
            window.rectTransform.sizeDelta = new Vector2(200, 300) * EaseOutElastic(timer);
        }
    }

    public override string ToFileString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Z_MessagePack");
        sb.AppendLine("pos:" + ConvertPos());
        sb.AppendLine("image:" + index);
        return sb.ToString();
    }

    public override string ToEditorString() {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("画像インデックス:" + index);
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
                    case "image":
                        index = int.Parse(value);
                        break;
                }
            }
        }
    }


    private float EaseOutElastic(float x) {
        const float c5 = (2 * Mathf.PI) / 4.5f;

        return x == 0
          ? 0
          : x == 1
          ? 1
          : x < 0.5f
          ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2
          : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
    }
}
