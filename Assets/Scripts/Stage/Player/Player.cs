using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MoveVector {
	UP,
	DOWN,
	LEFT,
	RIGHT
}


/// <summary>
/// プレイヤークラス。この下にPlayerMoveやPlayerStatusなど別クラスを作る予定
/// </summary>
public class Player : MonoBehaviour {

	public static Player instance;

	[SerializeField]
	private int stepMax;
	private int stepCount;

	private float stepTimer = 0;
	private float remainingTime = 10; //プレイヤーの残り時間
	private float remainingTimeMax = 10;

	private List<MoveVector> moveRecord;
	private List<float> stepTimers;
	private List<bool> isMoveStep;

	private bool isEnemyCol;
	private bool isSafe;

	private string canStepCode = "12A";

	[SerializeField]
	private Text timerText;

	private void Awake() {
		moveRecord = new List<MoveVector>();
		stepTimers = new List<float>();
		isMoveStep = new List<bool>();

		if (instance != null) {
			Destroy(instance);
		}
		instance = this;
	}

	public void Start() {

	}

	// Update is called once per frame
	void Update() {
		Move();
		SettingStepInterval();

		UpdateTimer();

		if (InputManager.GetKeyDown(Keys.A) || stepCount > stepMax) {
			ResetStage();
		}

	}

	private void FixedUpdate() {
		if (isEnemyCol == true && isSafe == false) {
			ResetStage();
		}
		isEnemyCol = false;
		isSafe = false;

	}

	void Move() {
		if (InputManager.GetKeyDown(Keys.LEFT)) {
			moveRecord.Add(MoveVector.LEFT);
			transform.localEulerAngles = Vector3.up * -90;
			stepCount++;
			if (CanStep(transform.position + Vector3.left)) {
				transform.position += Vector3.left;
				isMoveStep.Add(true);
			} else {
				isMoveStep.Add(false);
			}
		}
		if (InputManager.GetKeyDown(Keys.UP)) {
			moveRecord.Add(MoveVector.UP);
			transform.localEulerAngles = Vector3.up * 0;
			stepCount++;
			if (CanStep(transform.position + Vector3.forward)) {
				transform.position += Vector3.forward;
				isMoveStep.Add(true);
			} else {
				isMoveStep.Add(false);
			}
		}
		if (InputManager.GetKeyDown(Keys.RIGHT)) {
			moveRecord.Add(MoveVector.RIGHT);
			transform.localEulerAngles = Vector3.up * 90;
			stepCount++;
			if(CanStep(transform.position + Vector3.right)) {
				transform.position += Vector3.right;
				isMoveStep.Add(true);
			} else {
				isMoveStep.Add(false);
			}
		}
		if (InputManager.GetKeyDown(Keys.DOWN)) {
			moveRecord.Add(MoveVector.DOWN);
			transform.localEulerAngles = Vector3.up * 180;
			stepCount++;
			if(CanStep(transform.position + Vector3.back)) {
				transform.position += Vector3.back;
				isMoveStep.Add(true);
			} else {
				isMoveStep.Add(false);
			}
		}
	}

	//入力の待ち時間を記録する
	void SettingStepInterval() {
		bool isMoveKey = 
			InputManager.GetKeyDown(Keys.LEFT)  ||
			InputManager.GetKeyDown(Keys.UP)    ||
			InputManager.GetKeyDown(Keys.RIGHT) ||
			InputManager.GetKeyDown(Keys.DOWN);

		stepTimer += Time.deltaTime;

		//移動キーを押したら入力待ち時間を記録する
		if (isMoveKey == true) {
			stepTimers.Add(stepTimer);
			stepTimer = 0;
		}

	}

	void ResetStage() {
		//--移動方向と入力待ち時間をGhostManagerに記録する---//
		List<float> temp = stepTimers;
		List<MoveVector> temp2 = moveRecord;

		GhostManager.instance.stepIntervals.Add(temp);
		GhostManager.instance.moveRecords.Add(temp2);

		for (int i = 0; i < stepTimers.Count; ++i) {
			stepTimers = new List<float>();
			moveRecord = new List<MoveVector>();
		}

		GhostManager.instance.AddGhost();
		stepCount = 0;
		transform.position = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
		GhostManager.instance.ResetStage();

		remainingTime = remainingTimeMax;
	}

	bool CanStep(Vector3 pos) {
		var obj = Stage.instance.GetStageObject(pos);
		if (obj == null) {
			return true; //奈落でも進めるけど落ちる
		}
		var code = Stage.GetObjectCode(obj.name);
		if (canStepCode.Contains(code.ToString())) {
			return true;
		}
		if (code == 'B') { //シャッター
			var door = obj.GetComponent<Door>();
			var channel = door.channel;
			return (SwitchManager.instance.channel[channel] ^ door.isReverse);
		}
		return false;
	}

	void UpdateTimer() {
		if(stepCount > 0) {
			remainingTime -= Time.deltaTime;
		}
		var intPart = Mathf.Floor(remainingTime); //整数部分
		var fractionalPart = Mathf.Floor((remainingTime - intPart) * 10);
		timerText.text = "<size=72>" + intPart + ".</size>" 
			+ "<size=32>" + fractionalPart + "</size>";
	}
}
