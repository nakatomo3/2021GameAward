using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class PauseManager : MonoBehaviour {

	/* 
	 * ポーズ　┬　ステージセレクト　　　　　　　　　 ┬　本当に戻りますか？(はい)
	 * 　　　　│　　　　　　　　　　　　　　　　　　 └　いいえ
	 * 　　　　├　オプション　　　  ┬　各種設定
	 * 　　　　│ 　　　　　　　　　 ├　設定を保存　　┬　本当に保存する？(はい)
	 * 　　　　│ 　　　　　　　　　 │　　　　　　　　└　いいえ
	 * 　　　　│ 　　　　　　　　　 └　データ初期化　┬　本当に消去する？(はい)
	 * 　　　　│ 　　　　　　　　　　　　　　　　　  └　いいえ
	 * 　　　　└　ゲーム終了　　　　　　　　　　　　 ┬　ゲームを終了する？(はい)
	 * 　　　　　　　　　　　　　　 　　　　　　　　 └　いいえ
	 */

	#region インスペクタ参照

	[SerializeField]
	private GameObject mainWindow;

	[SerializeField]
	private GameObject checkWindow;

	[SerializeField]
	private GameObject optionWindow;

	[SerializeField]
	private RectTransform cursor;

	[SerializeField]
	private Image cursorImage;

	[SerializeField]
	private Text description;

	[SerializeField]
	private Text message;

	[SerializeField]
	private Text cancelText;

	[SerializeField]
	private GameObject buttonY;

	[SerializeField]
	private Image circleY;

	[Header("--------------------")]
	[Space(20)]

	[SerializeField]
	private Text screenSizeText;

	[SerializeField]
	private Text fullScreenText;

	[SerializeField]
	private Text bgmVolumeText;

	[SerializeField]
	private Text seVolumeText;

	#endregion

	#region データ部

	//メインメニュー関係
	private bool isMain = true;
	private int _mainIndex;
	private int mainIndex {
		get { return _mainIndex; }
		set { _mainIndex = (value + 3) % 3; }
	}

	private enum PauseMode {
		STAGE_SELECT,
		OPTION,
		EXIT
	}
	private PauseMode nowMode = PauseMode.STAGE_SELECT;

	//確認画面関係
	private bool isCheckOn = false;

	//オプション関係
	private int _optionIndex;
	private int optionIndex {
		get { return _optionIndex; }
		set { _optionIndex = (value + 5) % 5; }
	}

	private enum OptionMode {
		MAIN,
		CONFIRM_CHECK,
		DELETE_CHECK
	}
	private OptionMode optionMode = OptionMode.MAIN;

	private enum ScreenSize {
		SD,  //720*480
		HD,  //1280*720
		FHD, //1920*1080
		QHD, //2560*1440
	}
	private ScreenSize _nowSize;
	private ScreenSize nowSize {
		get { return _nowSize; }
		set {
			var end = (int)ScreenSize.QHD + 1;
			_nowSize = (ScreenSize)(((int)value + end) % end);
		}
	}

	private bool isFullScreen;

	private int _bgmVolume;
	private int bgmVolume {
		get { return _bgmVolume; }
		set {
			var num = value;
			num = Mathf.Max(num, 0);
			num = Mathf.Min(num, 100);
			_bgmVolume = num;
		}
	}
	private int seVolume;

	//テキスト関係
	private string[] descriptionStrings = {
		"ステージセレクトに戻ると現在のステージの進行状況は破棄されます。",
		"ゲーム内の設定を変更します。",
		"ゲームを終了します。現在のステージの進行状況は破棄されます。",
		//ここからオプション
		"ウィンドウサイズを変更します。",
		"フルスクリーンモードのONOFFを行います。",
		"ゲーム内のBGMのボリュームを調節できます。",
		"ゲーム内のSEのボリュームを調節できます。",
		"オプションで行った変更を反映、保存します。"
	};

	private string[] messageTexts = {
		"本当に戻りますか？",
		"ゲームを終了する？",
		"設定を保存する？",
		"……本当に消す？"
	};

	private float cursorTimer = 0;

	private float dataEraseTimer;

	#endregion

	private void OnEnable() {
		isMain = true;
		mainIndex = 0;
		optionIndex = 0;

		isCheckOn = false;
	}

	void Update() {
		//カーソル関係
		cursorTimer += Time.deltaTime;
		cursorImage.color = new Color(1, 1, 1, Mathf.Abs(cursorTimer - 1) * 0.5f + 0.5f);
		cursor.sizeDelta = new Vector2(120, Mathf.Sin(Mathf.Abs(cursorTimer - 1) * Mathf.PI) * 120);
		if (cursorTimer >= 2) {
			cursorTimer = 0;
		}

		//最初に全部非表示にしておいて、必要なときに表示にする
		mainWindow.SetActive(false);
		checkWindow.SetActive(false);
		optionWindow.SetActive(false);
		buttonY.SetActive(false);

		message.text = "";

		if (isMain) {
			MainWindow();
		} else {
			switch (nowMode) {
				case PauseMode.STAGE_SELECT:
					message.text = messageTexts[0];
					SubmitWindow(() => SceneManager.LoadScene("Title"), () => isMain = true);
					break;
				case PauseMode.OPTION:
					EraseCheck();
					switch (optionMode) {
						case OptionMode.MAIN:
							optionWindow.SetActive(true);

							if (InputManager.GetKeyDown(Keys.A)) {
								isMain = true;
							}

							ChangeOption();
							DrawOption();
							break;
						case OptionMode.CONFIRM_CHECK:
							message.text = messageTexts[2];
							SubmitWindow(() => ConfirmOption(), () => optionMode = OptionMode.MAIN);
							break;
						case OptionMode.DELETE_CHECK:
							message.text = messageTexts[3];
							void Func() {
								PlayerPrefs.DeleteAll();
								SceneManager.LoadScene("Title");
							}
							SubmitWindow(Func, () => optionMode = OptionMode.MAIN);
							break;
					}

					break;
				case PauseMode.EXIT:
					message.text = messageTexts[1];
					SubmitWindow(SystemSupporter.ExitGame, () => isMain = true);
					break;
			}
		}
	}

	private void MainWindow() {
		mainWindow.SetActive(true);
		description.text = descriptionStrings[mainIndex];

		if (InputManager.GetKeyDown(Keys.UP)) {
			mainIndex--;
		}
		if (InputManager.GetKeyDown(Keys.DOWN)) {
			mainIndex++;
		}
		cursor.anchoredPosition = new Vector2(-440, 175 + 120 * -mainIndex);

		if (InputManager.GetKeyDown(Keys.A)) { //戻るボタン
			Stage.instance.isOptionMode = false;
		}

		if (InputManager.GetKeyDown(Keys.B)) { //決定
			nowMode = (PauseMode)mainIndex;
			isMain = false;

			nowSize = (ScreenSize)PlayerPrefs.GetInt("ScreenSize", (int)ScreenSize.HD);
			isFullScreen = PlayerPrefs.GetInt("IsFullScreen", 0) == 1 ? true : false;
			bgmVolume = PlayerPrefs.GetInt("BGMVolume", 50);
			seVolume = PlayerPrefs.GetInt("SEVolume", 50);
		}
	}

	private void EraseCheck() {
		buttonY.SetActive(true);
		if (InputManager.GetKey(Keys.Y)) {
			dataEraseTimer += Time.deltaTime;
			Debug.Log("Y");
		} else {
			dataEraseTimer -= Time.deltaTime * 5;
		}
		if (dataEraseTimer < 0) {
			dataEraseTimer = 0;
		}
		if (dataEraseTimer > 3) {
			optionMode = OptionMode.DELETE_CHECK;
			dataEraseTimer = 0;
		}
		circleY.fillAmount = dataEraseTimer / 3;
	}

	private void ChangeOption() {
		if (InputManager.GetKeyDown(Keys.UP)) {
			optionIndex--;
		}
		if (InputManager.GetKeyDown(Keys.DOWN)) {
			optionIndex++;
		}
		cursor.anchoredPosition = new Vector2(-440, 295 + 120 * -optionIndex);

		bool isRightInput = InputManager.GetKeyDown(Keys.RIGHT);
		bool isLeftInput = InputManager.GetKeyDown(Keys.LEFT);
		switch (optionIndex) {
			case 0:
				if (isRightInput) {
					nowSize++;
				}
				if (isLeftInput) {
					nowSize--;
				}
				break;
			case 1:
				if (isRightInput || isLeftInput) {
					isFullScreen = !isFullScreen;
				}
				break;
			case 2:
				if (isRightInput) {
					bgmVolume += 5;
				}
				if (isLeftInput) {
					bgmVolume -= 5;
				}
				break;
			case 3:
				if (isRightInput) {
					seVolume += 5;
				}
				if (isLeftInput) {
					seVolume -= 5;
				}
				break;
			case 4:
				cursor.anchoredPosition = new Vector2(-440, -250);
				if (InputManager.GetKeyDown(Keys.B)) {
					optionMode = OptionMode.CONFIRM_CHECK;
				}
				break;
		}
	}

	private void DrawOption() {
		var size = "";
		switch (nowSize) {
			case ScreenSize.SD:
				size = "◀720×480▶";
				break;
			case ScreenSize.HD:
				size = "◀1280×720▶";
				break;
			case ScreenSize.FHD:
				size = "◀1920×1080▶";
				break;
			case ScreenSize.QHD:
				size = "◀2560×1480▶";
				break;
		}
		screenSizeText.text = size;
		if (isFullScreen) {
			fullScreenText.text = "◀ON▶";
		} else {
			fullScreenText.text = "◀OFF▶";
		}
		bgmVolumeText.text = "◀" + bgmVolume + "%▶";
		seVolumeText.text = "◀" + seVolume + "%▶";
	}

	private void ConfirmOption() {
		PlayerPrefs.SetInt("ScreenSize", (int)nowSize);
		var screenMode = isFullScreen ? 1 : 0;
		PlayerPrefs.SetInt("IsFullScreen", screenMode);
		PlayerPrefs.SetInt("BGMVolume", bgmVolume);
		PlayerPrefs.SetInt("SEVolume", seVolume);

		optionMode = OptionMode.MAIN;
		isMain = true;
	}

	private void SubmitWindow(Action onFunc, Action offFunc) {
		checkWindow.SetActive(true);
		ChangeSubmit();
		if (InputManager.GetKeyDown(Keys.B)) {
			if (isCheckOn) {
				onFunc();
			} else {
				offFunc();
			}
		}
		if (InputManager.GetKeyDown(Keys.A)) {
			offFunc();
		}
	}

	private void ChangeSubmit() {
		if (InputManager.GetKeyDown(Keys.UP) || InputManager.GetKeyDown(Keys.DOWN)) {
			isCheckOn = !isCheckOn;
		}
		if (isCheckOn) {
			cursor.anchoredPosition = new Vector2(-440, 175);
		} else {
			cursor.anchoredPosition = new Vector2(-440, 55);
		}
	}
}
