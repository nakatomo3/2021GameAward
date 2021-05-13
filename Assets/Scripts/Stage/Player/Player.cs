using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum ActionRecord {
	UP,
	DOWN,
	LEFT,
	RIGHT,
	ATTACK,
	DAMAGE,
	NONE,
}



/// <summary>
/// プレイヤークラス。この下にPlayerMoveやPlayerStatusなど別クラスを作る予定
/// </summary>
public class Player : MonoBehaviour {

	public static Player instance;

	public int phase = 0;

	public float moveIntervalMax;

	[HideInInspector]
	public bool isMoved = false; //動き始めたか

	[HideInInspector]
	public bool canAction = true;

	[Disable]
	public float stepTimer = 0;

	private float moveIntervalTimer = 0;

	[SerializeField]
	private float turnIntervalMax;
	private float turnIntervalTimer = 0;
	private bool canMove = true;

	[HideInInspector]
	public Vector3 oldStepPos;
	[HideInInspector]
	public Vector3 newStepPos;
	[HideInInspector]
	public List<ActionRecord> actionRecord;

	[HideInInspector]
	public Vector3 startPosition;

	private bool isEnemyCol;
	private bool isSafe;
	public bool canPhaseClear = false;

	public int enemyCount = 0;


	private string canStepCode = "1289ACdEhIJKYZ"; //B、F、G、Hは足場の状態が変わるので関数内部で判定

	public int nowTurn;

	[SerializeField]
	private Sprite[] timerNumbers;

	[Disable]
	[SerializeField]
	private Image oneOnly;
	[Disable]
	[SerializeField]
	private Image tenDigit;
	[Disable]
	[SerializeField]
	private Image oneDigit;

	private Image filter;

	[HideInInspector]
	public bool isAlive = true;

	public Material timeupIcon;
	public Material ghostIcon;
	public GameObject deathReasonIcon;

	private float rewindTimer;
	private const float rewindInterval = 0.2f;
	private int rewindIndex;

	private void Awake() {
		actionRecord = new List<ActionRecord>();
		startPosition = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
		if (instance != null) {
			Destroy(instance);
		}
		instance = this;
	}

	public void Start() {
		filter = Stage.instance.lastMomentFilter.GetComponent<Image>();
		oldStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
		newStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);

		GameObject startObj = Stage.instance.GetStageObject(this.transform.position);
		nowTurn = startObj.GetComponent<StartBlock>().turnMax;
	}

	// Update is called once per frame
	void Update() {
		if (isAlive) {
			Action();
			SettingStepInterval();
			UpdateTurn();
			Attack();
		} else {
			Rewind();
		}
	}

	private void FixedUpdate() {
		isEnemyCol = false;
		isSafe = false;
	}

	void Action() {

		if (moveIntervalTimer > moveIntervalMax / 5) {
			transform.position = newStepPos;
			oldStepPos = newStepPos;
			canMove = true;
		} else {
			//次の移動先に線形補完で移動する
			canMove = false;
			moveIntervalTimer += Time.deltaTime;
			transform.position = Vector3.Lerp(oldStepPos, newStepPos, moveIntervalTimer / moveIntervalMax) + Vector3.up * Mathf.Sin(moveIntervalTimer / moveIntervalMax * Mathf.PI);
		}


		if (canAction == true) {
			if (InputManager.GetKey(Keys.LEFT)) {
				transform.localEulerAngles = Vector3.up * -90;
				isMoved = true;


				if (CanStep(transform.position + Vector3.left)) {
					//移動するわ
					actionRecord.Add(ActionRecord.LEFT);
					newStepPos = transform.position + Vector3.left;
					UseTurn();
				} else {
					actionRecord.Add(ActionRecord.NONE);
					UseTurn();
				}
				moveIntervalTimer = 0;


			}
			if (InputManager.GetKey(Keys.UP)) {

				transform.localEulerAngles = Vector3.up * 0;
				isMoved = true;
				if (CanStep(transform.position + Vector3.forward)) {
					actionRecord.Add(ActionRecord.UP);
					newStepPos = transform.position + Vector3.forward;
					UseTurn();
				} else {
					actionRecord.Add(ActionRecord.NONE);
					UseTurn();
				}
				moveIntervalTimer = 0;

			}
			if (InputManager.GetKey(Keys.RIGHT)) {

				transform.localEulerAngles = Vector3.up * 90;
				isMoved = true;
				if (CanStep(transform.position + Vector3.right)) {
					actionRecord.Add(ActionRecord.RIGHT);
					newStepPos = transform.position + Vector3.right;
					UseTurn();
				} else {
					actionRecord.Add(ActionRecord.NONE);
					UseTurn();
				}
				moveIntervalTimer = 0;
			}
			if (InputManager.GetKey(Keys.DOWN)) {

				transform.localEulerAngles = Vector3.up * 180;
				isMoved = true;
				if (CanStep(transform.position + Vector3.back)) {
					actionRecord.Add(ActionRecord.DOWN);
					newStepPos = transform.position + Vector3.back;
					UseTurn();
				} else {
					actionRecord.Add(ActionRecord.NONE);
					UseTurn();
				}
				moveIntervalTimer = 0;

			}
		}


		GoalCheck();
	}

	public void Attack() {
		if (Stage.instance.DestroyEnemy() == true) {
			enemyCount++;
		}

		if (Stage.instance.enemyList[phase] != null) {
			if (enemyCount >= Stage.instance.enemyList[phase].Count) {
				canPhaseClear = true;
			}
		}
	}

	//プレイヤーのAction()内で行動した時に呼ぶ
	void UseTurn() {
		turnIntervalTimer = 0;
		canAction = false;
		Stage.instance.Action();
		nowTurn--;
	}

	//入力の待ち時間を記録する
	void SettingStepInterval() {
		bool isMoveKey =
			InputManager.GetKey(Keys.LEFT) ||
			InputManager.GetKey(Keys.UP) ||
			InputManager.GetKey(Keys.RIGHT) ||
			InputManager.GetKey(Keys.DOWN);

		stepTimer += Time.deltaTime;

		//移動キーを押したら入力待ち時間を記録する
		if (isMoveKey == true && canMove == true) {
			stepTimer = 0;
		}

	}

	bool CanStep(Vector3 pos) {
		bool canStep = false;
		var obj = Stage.instance.GetStageObject(pos);
		if (obj == null) {
			return false; //奈落でも進めるけど落ちる
		}
		var code = Stage.GetObjectCode(obj.name);
		if (canStepCode.Contains(code.ToString())) {
			canStep = true;
		}
		if (code == 'B') { //シャッター
			var door = obj.GetComponent<Door>();
			var channel = door.channel;
			canStep = (SwitchManager.instance.channel[channel] ^ door.isReverse);

		} else if (code == 'F') {//ピストン
			canStep = false;
		} else {
			//ピストンが射出されている状態の判定
			Vector3[] pistonPos = new Vector3[8];
			pistonPos[0] = this.transform.position + Vector3.right + Vector3.forward;//右前
			pistonPos[1] = this.transform.position + Vector3.right + Vector3.back;//右下
			pistonPos[2] = this.transform.position + Vector3.left + Vector3.back;//左後ろ
			pistonPos[3] = this.transform.position + Vector3.left + Vector3.forward;//左前

			pistonPos[4] = this.transform.position + Vector3.forward * 2;//前
			pistonPos[5] = this.transform.position + Vector3.back * 2;//後ろ
			pistonPos[6] = this.transform.position + Vector3.right * 2;//右
			pistonPos[7] = this.transform.position + Vector3.left * 2;//左

			for (int i = 0; i < 8; ++i) {
				GameObject g = Stage.instance.GetStageObject(pistonPos[i]);
				if (g != null) {
					if (g.name[0] == 'F') {
						Piston piston = g.GetComponent<Piston>();

						if (piston != null && piston.isPush == true) {
							Vector3 addPos = Vector3.zero;

							switch (piston.direction) {
								case Piston.Direction.Front:
									addPos = Vector3.forward;
									break;

								case Piston.Direction.Back:
									addPos = Vector3.back;
									break;

								case Piston.Direction.Left:
									addPos = Vector3.left;
									break;

								case Piston.Direction.Right:
									addPos = Vector3.right;
									break;
							}

							//射出されているとこは行けない
							if (pos.x == pistonPos[i].x + addPos.x && pos.z == pistonPos[i].z + addPos.z) {
								canStep = false;
							}
						}
					}
				}
			}
		}

		return canStep;
	}

	void UpdateTurn() {
		turnIntervalTimer += Time.deltaTime;
		if (turnIntervalTimer >= 0.5f) {
			canAction = true;
		}


		if (isMoved == true) {
			// remainingTime -= Time.deltaTime;
		}
		var intPart = Mathf.Floor(nowTurn); //整数部分
											//小数部分を一応残しておく
											//var fractionalPart = Mathf.Floor((remainingTime - intPart) * 10);
		if (nowTurn >= 10) {
			var ten = Mathf.FloorToInt(nowTurn / 10);
			var one = Mathf.FloorToInt(nowTurn - ten * 10);
			tenDigit.sprite = timerNumbers[ten];
			oneDigit.sprite = timerNumbers[one];
			tenDigit.enabled = true;
			oneDigit.enabled = true;
			oneOnly.enabled = false;
		} else if (nowTurn > 0) {
			var one = Mathf.FloorToInt(nowTurn);
			oneOnly.sprite = timerNumbers[one];
			tenDigit.enabled = false;
			oneDigit.enabled = false;
			oneOnly.enabled = true;
			if (nowTurn > 5) {
				oneOnly.color = Color.white;
			} else {
				oneOnly.color = new Color(1, 0.375f, 0, 1);
			}
		} else {
			oneOnly.sprite = timerNumbers[0];
			TimeUp();
		}

	}

	void GoalCheck() {
		GameObject obj = Stage.instance.GetStageObject(this.transform.position);
		if (obj != null && obj.name[0] == 'J') {
			var goalPhase = obj.GetComponent<Goal>().phaseCount;
			var isThisGoal = (goalPhase & (int)Mathf.Pow(2, phase)) > 0;
			if (isThisGoal == false) {
				return;
			}
			GhostManager.instance.ResetStage();

			int beforePhase = phase - 1;

			if (canPhaseClear == true) {
				if (phase >= Stage.instance.startBlockList.Count) {
					Stage.instance.nowMode = Stage.Mode.CLEAR;
					return;
				} else {
					phase++;
					canPhaseClear = false;
					enemyCount = 0;

				}

			} else {
				//フェーズがクリアできない処理
				GameObject beforStart = Stage.instance.startBlockList[phase];

				transform.position = beforStart.transform.position;
				nowTurn = beforStart.GetComponent<StartBlock>().turnMax;

				oldStepPos = Stage.instance.startBlockList[phase].transform.position;
				newStepPos = Stage.instance.startBlockList[phase].transform.position;
				transform.position = Stage.instance.startBlockList[phase].transform.position;

				Stage.instance.ResetEnemy();
				canPhaseClear = false;
				enemyCount = 0;
				return;
			}

			if (beforePhase == -1) {
				GhostManager.instance.AddGhost(Stage.instance.startPosition, transform.position);
			} else {
				GhostManager.instance.AddGhost(Stage.instance.startBlockList[beforePhase + 1].transform.position, transform.position);
			}

			//--移動方向とアクションをGhostManagerに記録する---//
			List<ActionRecord> temp2 = actionRecord;

			GhostManager.instance.moveRecords.Add(temp2);

			for (int i = 0; i < actionRecord.Count; ++i) {
				actionRecord = new List<ActionRecord>();
			}

			isMoved = false;

			if (beforePhase + 2 >= Stage.instance.startBlockList.Count) {
				return;
			}
			GameObject newStart = Stage.instance.startBlockList[beforePhase + 2];
			transform.position = newStart.transform.position;
			nowTurn = newStart.GetComponent<StartBlock>().turnMax;

			oldStepPos = Stage.instance.startBlockList[beforePhase + 2].transform.position;
			newStepPos = Stage.instance.startBlockList[beforePhase + 2].transform.position;
			transform.position = Stage.instance.startBlockList[beforePhase + 2].transform.position;
			GhostManager.instance.ResetStage();
		}
	}

	public void CheckPoint(float time, int _loopMax) {
		startPosition = this.transform.position;

	}

	//ダメージを受けると秒数が減る
	public void Damage(float value) {
	}

	//タイムアップ演出
	private void TimeUp() {
		GameOver(timeupIcon);
	}

	//ゴーストでゲームオーバー演出
	public void GhostGameOver() {
		GameOver(ghostIcon);
	}

	public void GameOver(Material material) {
		if(isAlive == false) {
			return;
		}
		isAlive = false;
		Stage.instance.nowMode = Stage.Mode.DEAD;
		var obj = Instantiate(deathReasonIcon, transform.position + new Vector3(0, 10f, -5) * 0.8f, Quaternion.identity);
		obj.GetComponent<Renderer>().material = material;
		rewindIndex = actionRecord.Count - 2;
		rewindTimer = -2;
	}

	private void Rewind() {
		rewindTimer += Time.deltaTime;
		if (rewindTimer >= rewindInterval) {
			switch (actionRecord[rewindIndex]) {
				case ActionRecord.UP:
					transform.position -= Vector3.forward; //逆再生なのでマイナス
					transform.localEulerAngles = Vector3.up * 180;
					break;
				case ActionRecord.DOWN:
					transform.position -= Vector3.back;
					transform.localEulerAngles = Vector3.up * -180;
					break;
				case ActionRecord.LEFT:
					transform.position -= Vector3.left;
					transform.localEulerAngles = Vector3.up * -90;
					break;
				case ActionRecord.RIGHT:
					transform.position -= Vector3.right;
					transform.localEulerAngles = Vector3.up * 90;
					break;
				case ActionRecord.ATTACK:
				case ActionRecord.DAMAGE:
				case ActionRecord.NONE:
					break;
			}
			rewindIndex--;
			rewindTimer = 0;
			//GhostManager.instance.Rewind();
		}
		if(rewindIndex < 0) {
			Stage.instance.nowMode = Stage.Mode.GAME;
			isAlive = true;
			Stage.instance.crt.enabled = false;
			GhostManager.instance.ResetStage();
			ResetStage();
		}
	}

	public void SetPosition(Vector3 pos) {
		this.transform.position = pos;
		newStepPos = pos;
		oldStepPos = pos;
	}
	public void AddPosition(Vector3 pos) {
		this.transform.position += pos;
		newStepPos += pos;
		oldStepPos += pos;
	}

	private void ResetStage() {
		GameObject beforStart = Stage.instance.startBlockList[phase];

		transform.position = beforStart.transform.position;
		nowTurn = beforStart.GetComponent<StartBlock>().turnMax;

		oldStepPos = Stage.instance.startBlockList[phase].transform.position;
		newStepPos = Stage.instance.startBlockList[phase].transform.position;
		transform.position = Stage.instance.startBlockList[phase].transform.position;

		Stage.instance.ResetEnemy();
		canPhaseClear = false;
		enemyCount = 0;
	}
}
