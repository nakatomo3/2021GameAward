using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum Skill {
	LOOP,
	RESET,
	RUNE,
	VEIL,
	MAX
}
public class PlayerSkill : MonoBehaviour {

	private enum SkillGauge {
		YELLOW,
		WHITE,
		BLUE,
		MAX
	}


	[SerializeField]
	private float[] skillChargeMax;
	private float[] skillChargeTimer;

	[SerializeField]
	private float[] skillReChargeMax;
	private float[] skillReChargeTimer;

	[SerializeField]
	public int[] skillMax;
	[HideInInspector]
	public int[] skillNum;


	private bool[] isSkillCharge = new bool[(int)Skill.MAX];
	private bool[] isSkillReCharge = new bool[(int)Skill.MAX];


	[SerializeField]
	private GameObject rune;
	[SerializeField]
	private GameObject veil;

	[SerializeField]
	private Image[] loopUI = new Image[(int)SkillGauge.MAX];
	[SerializeField]
	private Image[] resetUI = new Image[(int)SkillGauge.MAX];
	[SerializeField]
	private Image[] runeUI = new Image[(int)SkillGauge.MAX];
	[SerializeField]
	private Image[] veilUI = new Image[(int)SkillGauge.MAX];

	[SerializeField]
	private Sprite[] numsSprite = new Sprite[10];
	[SerializeField]
	private Image loopNum;

	private Color[] uiDefaultColor = new Color[(int)SkillGauge.MAX];

	private void Awake() {
		skillChargeTimer = new float[(int)Skill.MAX];
		skillReChargeTimer = new float[(int)Skill.MAX];
		skillNum = new int[(int)Skill.MAX];

		for (int i = 0; i < (int)Skill.MAX; ++i) {
			skillNum[i] = skillMax[i];
			skillReChargeTimer[i] = skillReChargeMax[i];
		}
	}

	private void Start() {
		for (int i = 0; i < (int)SkillGauge.MAX; ++i) {
			uiDefaultColor[i] = loopUI[i].color;
		}
	}

	// Update is called once per frame
	void Update() {
		UseSkill();
		CheckReCharge();
		SkillUI();
	}



	#region スキルの処理
	void UseSkill() {
		bool[] isSkillButton = new bool[(int)Skill.MAX];
		Action[] action = new Action[(int)Skill.MAX]; //関数を入れる用

		//判定を入れる
		isSkillButton[(int)Skill.LOOP] = InputManager.GetKeyDown(Keys.Y) &&
										 skillReChargeTimer[(int)Skill.LOOP] >= skillReChargeMax[(int)Skill.LOOP] &&
										 skillNum[(int)Skill.LOOP] > 0;

		isSkillButton[(int)Skill.RESET] = InputManager.GetKeyDown(Keys.X) &&
										  skillReChargeTimer[(int)Skill.RESET] >= skillReChargeMax[(int)Skill.RESET] &&
										  skillNum[(int)Skill.RESET] > 0;

		isSkillButton[(int)Skill.RUNE] = InputManager.GetKeyDown(Keys.B) &&
										 skillReChargeTimer[(int)Skill.RUNE] >= skillReChargeMax[(int)Skill.RUNE] &&
										 skillNum[(int)Skill.RUNE] > 0;

		isSkillButton[(int)Skill.VEIL] = InputManager.GetKeyDown(Keys.A) &&
										 skillReChargeTimer[(int)Skill.VEIL] >= skillReChargeMax[(int)Skill.VEIL] &&
										 skillNum[(int)Skill.VEIL] > 0;

		//関数を入れる
		action[(int)Skill.LOOP] = Loop;
		action[(int)Skill.RESET] = ResetStage;
		action[(int)Skill.RUNE] = Rune;
		action[(int)Skill.VEIL] = Veil;


		for (int i = 0; i < (int)Skill.MAX; ++i) {
			//スキルを使うか判定
			if (isSkillButton[i] == true) {
				isSkillCharge[i] = true;
			}

			//スキルの処理
			if (isSkillCharge[i] == true) {
				action[i]();

				//スキルをチャージする
				skillChargeTimer[i] += Time.deltaTime;
				if (skillChargeTimer[i] > skillChargeMax[i]) {
					skillChargeTimer[i] = skillChargeMax[i];
				}
			} else {
				skillChargeTimer[i] = 0;

			}

			//スキルのリチャージ
			if (skillNum[i] > 0 && isSkillReCharge[i] == true) {
				skillReChargeTimer[i] += Time.deltaTime;
				if (skillReChargeTimer[i] >= skillReChargeMax[i]) {
					skillReChargeTimer[i] = skillReChargeMax[i];
				}
			}

		}

	}
	void CheckReCharge() {
		if (skillReChargeTimer[(int)Skill.LOOP] < skillReChargeMax[(int)Skill.LOOP]) {
			isSkillReCharge[(int)Skill.LOOP] = true;
		} else {
			isSkillReCharge[(int)Skill.LOOP] = false;
		}


		if (skillReChargeTimer[(int)Skill.RESET] < skillReChargeMax[(int)Skill.RESET]) {
			isSkillReCharge[(int)Skill.RESET] = true;
		} else {
			isSkillReCharge[(int)Skill.RESET] = false;
		}

		if (skillReChargeTimer[(int)Skill.RUNE] < skillReChargeMax[(int)Skill.RUNE]) {
			isSkillReCharge[(int)Skill.RUNE] = true;
		} else {
			isSkillReCharge[(int)Skill.RUNE] = false;
		}

		if (skillReChargeTimer[(int)Skill.VEIL] < skillReChargeMax[(int)Skill.VEIL]) {
			isSkillReCharge[(int)Skill.VEIL] = true;
		} else {
			isSkillReCharge[(int)Skill.VEIL] = false;
		}

	}

	void Loop() {
		//Yボタンを話したらチャージ終了
		if (InputManager.GetKeyUp(Keys.Y)) {
			isSkillCharge[(int)Skill.LOOP] = false;
		}

		//一定時間チャージしたら発動
		if (skillChargeTimer[(int)Skill.LOOP] >= skillChargeMax[(int)Skill.LOOP]) {
			skillNum[(int)Skill.LOOP]--;

			
			List<ActionRecord> temp2 = Player.instance.actionRecord;

			//GhostManager.instance.stepIntervals.Add(temp);
			GhostManager.instance.moveRecords.Add(temp2);

			for (int i = 0; i < Player.instance.actionRecord.Count; ++i) {
				Player.instance.actionRecord = new List<ActionRecord>();
			}

			//GhostManager.instance.AddGhost();
			Player.instance.isMoved = false;
			Player.instance.oldStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
			Player.instance.newStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
			transform.position = Player.instance.startPosition;
			GhostManager.instance.ResetStage();


			skillReChargeTimer[(int)Skill.LOOP] = 0;
			skillChargeTimer[(int)Skill.LOOP] = 0;
			isSkillCharge[(int)Skill.LOOP] = false;
		}
	}
	public void ResetStage() {
		if (InputManager.GetKeyUp(Keys.X)) {
			isSkillCharge[(int)Skill.RESET] = false;
		}


		if (skillChargeTimer[(int)Skill.RESET] >= skillChargeMax[(int)Skill.RESET]) {

			Player.instance.isMoved = false;
			Player.instance.oldStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
			Player.instance.newStepPos = new Vector3(Stage.instance.startPosition.x, 0, Stage.instance.startPosition.y);
			transform.position = Player.instance.startPosition;
			transform.localEulerAngles = Vector3.zero;

			skillReChargeTimer[(int)Skill.RESET] = 0;
			isSkillCharge[(int)Skill.RESET] = false;

			GhostManager.instance.DeleteGhost();
			Player.instance.actionRecord.Clear();


			for (int i = 0; i < (int)Skill.MAX; ++i) {
				skillNum[i] = skillMax[i];
			}
		}
	}
	void Rune() {


		if (InputManager.GetKeyUp(Keys.B) && skillChargeTimer[(int)Skill.RUNE] >= skillChargeMax[(int)Skill.RUNE]) {

			Instantiate(rune, this.transform.position, transform.rotation);
			skillReChargeTimer[(int)Skill.RUNE] = 0;
			isSkillCharge[(int)Skill.RUNE] = false;

		} else if (InputManager.GetKeyUp(Keys.B)) {
			isSkillCharge[(int)Skill.RUNE] = false;
		}
	}
	void Veil() {
		if (InputManager.GetKeyUp(Keys.A)) {

			Instantiate(veil, this.transform.position, transform.rotation).GetComponent<Veil>().shotPower = skillChargeTimer[(int)Skill.VEIL] / 0.5f;
			skillReChargeTimer[(int)Skill.VEIL] = 0;
			isSkillCharge[(int)Skill.VEIL] = false;

		}
	}
	#endregion

	#region スキルUIの処理
	void SkillUI() {
		LoopSkillUI();
		ResetSkillUI();
		RuneSkillUI();
		VeilSkillUI();

	}

	void LoopSkillUI() {
		loopUI[(int)SkillGauge.YELLOW].fillAmount = skillReChargeTimer[(int)Skill.LOOP] / skillReChargeMax[(int)Skill.LOOP];
		loopUI[(int)SkillGauge.BLUE].fillAmount = skillChargeTimer[(int)Skill.LOOP] / skillChargeMax[(int)Skill.LOOP];
		loopUI[(int)SkillGauge.WHITE].fillAmount = loopUI[(int)SkillGauge.BLUE].fillAmount + 0.02f;
		loopNum.sprite = numsSprite[(int)skillNum[(int)Skill.LOOP]];

		//リチャージ中は色を暗くする
		if (skillReChargeTimer[(int)Skill.LOOP] < skillReChargeMax[(int)Skill.LOOP]) {
			loopUI[(int)SkillGauge.YELLOW].color = uiDefaultColor[(int)SkillGauge.YELLOW] * 0.5f;
			loopUI[(int)SkillGauge.BLUE].color = uiDefaultColor[(int)SkillGauge.BLUE] * 0.5f;
			loopUI[(int)SkillGauge.WHITE].color = uiDefaultColor[(int)SkillGauge.WHITE] * 0.5f;
			loopNum.color = uiDefaultColor[(int)SkillGauge.WHITE] * 0.5f;
		} else {
			loopUI[(int)SkillGauge.YELLOW].color = uiDefaultColor[(int)SkillGauge.YELLOW];
			loopUI[(int)SkillGauge.BLUE].color = uiDefaultColor[(int)SkillGauge.BLUE];
			loopUI[(int)SkillGauge.WHITE].color = uiDefaultColor[(int)SkillGauge.WHITE];
			loopNum.color = uiDefaultColor[(int)SkillGauge.WHITE];
		}
	}
	void ResetSkillUI() {
		resetUI[(int)SkillGauge.YELLOW].fillAmount = skillReChargeTimer[(int)Skill.RESET] / skillReChargeMax[(int)Skill.RESET];
		resetUI[(int)SkillGauge.BLUE].fillAmount = skillChargeTimer[(int)Skill.RESET] / skillChargeMax[(int)Skill.RESET];
		resetUI[(int)SkillGauge.WHITE].fillAmount = resetUI[(int)SkillGauge.BLUE].fillAmount + 0.02f;

		//リチャージ中は色を暗くする
		if (skillReChargeTimer[(int)Skill.RESET] < skillReChargeMax[(int)Skill.RESET]) {
			resetUI[(int)SkillGauge.YELLOW].color = uiDefaultColor[(int)SkillGauge.YELLOW] * 0.5f;
			resetUI[(int)SkillGauge.BLUE].color = uiDefaultColor[(int)SkillGauge.BLUE] * 0.5f;
			resetUI[(int)SkillGauge.WHITE].color = uiDefaultColor[(int)SkillGauge.WHITE] * 0.5f;
		} else {
			resetUI[(int)SkillGauge.YELLOW].color = uiDefaultColor[(int)SkillGauge.YELLOW];
			resetUI[(int)SkillGauge.BLUE].color = uiDefaultColor[(int)SkillGauge.BLUE];
			resetUI[(int)SkillGauge.WHITE].color = uiDefaultColor[(int)SkillGauge.WHITE];
		}
	}
	void RuneSkillUI() {
		runeUI[(int)SkillGauge.YELLOW].fillAmount = skillReChargeTimer[(int)Skill.RUNE] / skillReChargeMax[(int)Skill.RUNE];
		runeUI[(int)SkillGauge.BLUE].fillAmount = skillChargeTimer[(int)Skill.RUNE] / skillChargeMax[(int)Skill.RUNE];
		runeUI[(int)SkillGauge.WHITE].fillAmount = runeUI[(int)SkillGauge.BLUE].fillAmount + 0.02f;

		//リチャージ中は色を暗くする
		if (skillReChargeTimer[(int)Skill.RUNE] < skillReChargeMax[(int)Skill.RUNE]) {
			runeUI[(int)SkillGauge.YELLOW].color = uiDefaultColor[(int)SkillGauge.YELLOW] * 0.5f;
			runeUI[(int)SkillGauge.BLUE].color = uiDefaultColor[(int)SkillGauge.BLUE] * 0.5f;
			runeUI[(int)SkillGauge.WHITE].color = uiDefaultColor[(int)SkillGauge.WHITE] * 0.5f;
		} else {
			runeUI[(int)SkillGauge.YELLOW].color = uiDefaultColor[(int)SkillGauge.YELLOW];
			runeUI[(int)SkillGauge.BLUE].color = uiDefaultColor[(int)SkillGauge.BLUE];
			runeUI[(int)SkillGauge.WHITE].color = uiDefaultColor[(int)SkillGauge.WHITE];
		}
	}
	void VeilSkillUI() {
		veilUI[(int)SkillGauge.YELLOW].fillAmount = skillReChargeTimer[(int)Skill.VEIL] / skillReChargeMax[(int)Skill.VEIL];
		veilUI[(int)SkillGauge.BLUE].fillAmount = skillChargeTimer[(int)Skill.VEIL] / skillChargeMax[(int)Skill.VEIL];
		veilUI[(int)SkillGauge.WHITE].fillAmount = veilUI[(int)SkillGauge.BLUE].fillAmount + 0.02f;

		//リチャージ中は色を暗くする
		if (skillReChargeTimer[(int)Skill.VEIL] < skillReChargeMax[(int)Skill.VEIL]) {
			veilUI[(int)SkillGauge.YELLOW].color = uiDefaultColor[(int)SkillGauge.YELLOW] * 0.5f;
			veilUI[(int)SkillGauge.BLUE].color = uiDefaultColor[(int)SkillGauge.BLUE] * 0.5f;
			veilUI[(int)SkillGauge.WHITE].color = uiDefaultColor[(int)SkillGauge.WHITE] * 0.5f;
		} else {
			veilUI[(int)SkillGauge.YELLOW].color = uiDefaultColor[(int)SkillGauge.YELLOW];
			veilUI[(int)SkillGauge.BLUE].color = uiDefaultColor[(int)SkillGauge.BLUE];
			veilUI[(int)SkillGauge.WHITE].color = uiDefaultColor[(int)SkillGauge.WHITE];
		}
	}
	#endregion
}
