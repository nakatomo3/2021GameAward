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
		}
	}

	//入れる
	public List<Material> images;

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
		//isOnPlayerの更新
	}

	void Update() {
		if (isOnPlayer) {
			timer += Time.deltaTime;
		}
		window.color = new Color(1, 1, 1, timer / maxTime); //フェードインする
		if(timer > 0 && isOnPlayer == false) { //離れたらフェードアウト
			timer -= Time.deltaTime;
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
}
