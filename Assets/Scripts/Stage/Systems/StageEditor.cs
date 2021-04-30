using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEditor : MonoBehaviour {

	[SerializeField]
	private bool isController = false;

	#region インスペクタ参照部
	[Disable]
	[SerializeField]
	private GameObject cursor;

	[Disable]
	[SerializeField]
	private Text postionText;

	[Disable]
	[SerializeField]
	private Text phaseText;

	[Disable]
	[SerializeField]
	private Text objInfo;

	[Disable]
	[SerializeField]
	private List<Text> adjacentObjInfo;
	[Disable]
	[SerializeField]
	private GameObject objList;

	[Disable]
	[SerializeField]
	private GameObject editingArea;

	[SerializeField]
	private GameObject detailWindow;
	[SerializeField]
	private Text detailObjectName;
	[SerializeField]
	private Text detailOptionText;
	[SerializeField]
	private RectTransform optionCursor;
	#endregion


	#region データ部
	private static int _editorPhase;
	public static int editorPhase {
		get { return _editorPhase; }
		set {
			_editorPhase = (value + 7) % 7;
		}
	}

	private bool isDetailMode = false;

	private float inputTimer = 0;
	private const float firstInterval = 0.5f;
	private const float continuousInterval = 0.1f;

	//カメラの遠さの求め方=BaseRange + cameraMode * StepRange
	private float cameraBaseRange = 10;
	private int cameraMode = 0;
	private int cameraMax = 3;
	private float cameraStepRange = 5;

	private int _objIndex;
	private int objIndex {
		get { return _objIndex; }
		set { _objIndex = (value + Stage.instance.objectIndex.Count) % Stage.instance.objectIndex.Count; }
	}
	private float objTimer = 0;
	private bool isObjListMode = false;

	private bool isAreaMode = false;
	private Vector3 startPos = new Vector3();

	private char editingChar = '0';
	private DetailBase editingScript;
	private int optionMax = 1;
	private int _optionCount;
	private int optionCount {
		get { return _optionCount; }
		set { _optionCount = (value + optionMax) % optionMax; }
	}

	#endregion


	void Start() {

	}


	void Update() {
		if (isDetailMode) {
			DetailMode();
		} else {
			if (isObjListMode) {
				ObjectListMode();
			} else {
				PlaceMode();
			}
			objList.SetActive(isObjListMode);
		}

		if (InputManager.GetKeyDown(Keys.Y)) {
			editorPhase++;
		}
		phaseText.text = (editorPhase + 1).ToString();

		CameraControl();
		InformationUpdate();
	}

	private void PlaceMode() {

		//移動
		if (InputManager.GetKey(Keys.LEFT)) {
			inputTimer += Time.unscaledDeltaTime; //念のためunscaled
			if (InputManager.GetKeyDown(Keys.LEFT)) {
				transform.position += Vector3.left;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				transform.position += Vector3.left;
			}
		} else if (InputManager.GetKey(Keys.RIGHT)) {
			inputTimer += Time.unscaledDeltaTime; //念のためunscaled
			if (InputManager.GetKeyDown(Keys.RIGHT)) {
				transform.position += Vector3.right;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				transform.position += Vector3.right;
			}
		} else if (InputManager.GetKey(Keys.UP)) {
			inputTimer += Time.unscaledDeltaTime; //念のためunscaled
			if (InputManager.GetKeyDown(Keys.UP)) {
				transform.position += Vector3.forward;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				transform.position += Vector3.forward;
			}
		} else if (InputManager.GetKey(Keys.DOWN)) {
			inputTimer += Time.unscaledDeltaTime; //念のためunscaled
			if (InputManager.GetKeyDown(Keys.DOWN)) {
				transform.position += Vector3.back;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				transform.position += Vector3.back;
			}
		} else {
			inputTimer = 0;
		}

		//オブジェクト切り替え
		if (InputManager.GetKeyDown(Keys.L)) {
			objIndex--;
		}
		if (InputManager.GetKeyDown(Keys.R)) {
			objIndex++;
		}
		if (InputManager.GetKey(Keys.L) || InputManager.GetKey(Keys.R)) {
			objTimer += Time.unscaledDeltaTime;
		} else {
			objTimer = 0;
		}
		if (objTimer > 1) {
			isObjListMode = true;
		}

		if (InputManager.GetKeyDown(Keys.TRIGGER)) {
			isAreaMode = true;
			editingArea.SetActive(true);
			startPos = transform.position;
		}
		if (InputManager.GetKeyUp(Keys.TRIGGER)) {
			isAreaMode = false;
			editingArea.SetActive(false);
		}
		editingArea.transform.localScale = new Vector3(Mathf.Abs(startPos.x - transform.position.x) + 1, 1, Mathf.Abs(startPos.z - transform.position.z) + 1);
		editingArea.transform.position = (transform.position + startPos) * 0.5f;

		//オブジェクト置く消す
		if (InputManager.GetKey(Keys.B)) {
			if (isAreaMode == true) {
				Vector3 leftUp;
				if (transform.position.x > startPos.x) {
					if (transform.position.z > startPos.z) {
						leftUp = new Vector3(startPos.x, 0, transform.position.z);
					} else {
						leftUp = startPos;
					}
				} else {
					if (transform.position.z > startPos.z) {
						leftUp = transform.position;
					} else {
						leftUp = new Vector3(transform.position.x, 0, startPos.z);
					}
				}
				for (int i = 0; i < Mathf.Abs(startPos.x - transform.position.x) + 1; i++) {
					for (int j = 0; j < Mathf.Abs(startPos.z - transform.position.z) + 1; j++) {
						Stage.instance.GenerateObject(leftUp + new Vector3(i, 0, -j), Stage.instance.objectList[objIndex]);
					}
				}
			} else {
				Stage.instance.GenerateObject(transform.position, Stage.instance.objectList[objIndex]);
			}
		}

		if (InputManager.GetKey(Keys.A)) {
			void DestroyObj(Vector3 pos) {
				var obj = Stage.instance.GetStageObject(pos);
				if (obj != null) {
					Destroy(obj);
				}
			}

			if (isAreaMode == true) {
				Vector3 leftUp;
				if (transform.position.x > startPos.x) {
					if (transform.position.z > startPos.z) {
						leftUp = new Vector3(startPos.x, 0, transform.position.z);
					} else {
						leftUp = startPos;
					}
				} else {
					if (transform.position.z > startPos.z) {
						leftUp = transform.position;
					} else {
						leftUp = new Vector3(transform.position.x, 0, startPos.z);
					}
				}
				for (int i = 0; i < Mathf.Abs(startPos.x - transform.position.x) + 1; i++) {
					for (int j = 0; j < Mathf.Abs(startPos.z - transform.position.z) + 1; j++) {
						DestroyObj(leftUp + new Vector3(i, 0, -j));
					}
				}
			} else {
				DestroyObj(transform.position);
			}



		}

		if (InputManager.GetKeyDown(Keys.X) && isAreaMode == false) {
			var obj = Stage.instance.GetStageObject(transform.position);
			if (obj == null) {
				return;
			}
			var code = Stage.GetObjectCode(obj.name);
			if (code >= 'A' && code <= 'Z') { //大文字なら詳細編集可能なコード
				isDetailMode = true;
				editingScript = obj.GetComponent<DetailBase>();
				editingChar = code;
			}
		}
	}

	private void ObjectListMode() {
		if (!InputManager.GetKey(Keys.L) && !InputManager.GetKey(Keys.R)) {
			isObjListMode = false;
		}

		if (InputManager.GetKey(Keys.LEFT)) {
			inputTimer += Time.unscaledDeltaTime; //念のためunscaled
			if (InputManager.GetKeyDown(Keys.LEFT)) {
				objIndex--;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				objIndex--;
			}
		} else if (InputManager.GetKey(Keys.RIGHT)) {
			inputTimer += Time.unscaledDeltaTime; //念のためunscaled
			if (InputManager.GetKeyDown(Keys.RIGHT)) {
				objIndex++;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				objIndex++;
			}
		} else if (InputManager.GetKey(Keys.UP)) {
			inputTimer += Time.unscaledDeltaTime; //念のためunscaled
			if (InputManager.GetKeyDown(Keys.UP)) {
				objIndex--;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				objIndex--;
			}
		} else if (InputManager.GetKey(Keys.DOWN)) {
			inputTimer += Time.unscaledDeltaTime; //念のためunscaled
			if (InputManager.GetKeyDown(Keys.DOWN)) {
				objIndex++;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				objIndex++;
			}
		} else {
			inputTimer = 0;
		}


	}

	private void DetailMode() {
		if (InputManager.GetKeyUp(Keys.X)) {
			isDetailMode = false;
		}

		bool isLeftInput = false;
		bool isRightInput = false;

		if (InputManager.GetKeyDown(Keys.UP)) {
			optionCount--;
		} else if (InputManager.GetKeyDown(Keys.DOWN)) {
			optionCount++;
		} else if (InputManager.GetKey(Keys.LEFT)) {
			inputTimer += Time.unscaledDeltaTime;
			if (InputManager.GetKeyDown(Keys.LEFT)) {
				isLeftInput = true;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				isLeftInput = true;
			}
		} else if (InputManager.GetKey(Keys.RIGHT)) {
			inputTimer += Time.unscaledDeltaTime;
			if (InputManager.GetKeyDown(Keys.RIGHT)) {
				isRightInput = true;
			}
			if (inputTimer >= firstInterval + continuousInterval) {
				inputTimer = firstInterval;
				isRightInput = true;
			}
		} else {
			inputTimer = 0;
		}
		if (optionCount >= optionMax) {
			optionCount = 0;
		}

		

		//チャンネル設定を一括で出来るように
		if (editingScript is ChannelBase && optionCount == 0) {
			if (isLeftInput) {
				((ChannelBase)editingScript).channel--;
			}
			if (isRightInput) {
				((ChannelBase)editingScript).channel++;
			}
		}
		optionCursor.anchoredPosition = new Vector2(-125, 105 + optionCount * -30);

		switch (editingChar) {
			case 'A': //Switch
				optionMax = 1;
				break;
			case 'B': //Door
				optionMax = 3;
				switch (optionCount) {
					case 0: //チャンネル編集
						break;
					case 1: // 反転
						if (isLeftInput || isRightInput) {
							((Door)editingScript).isReverse = !((Door)editingScript).isReverse;
						}
						break;
					case 2: //回転
						if (isLeftInput) {
							((Door)editingScript).rotate--;
						}
						if (isRightInput) {
							((Door)editingScript).rotate++;
						}
						break;
					default:
						break;
				}
				break;
			case 'E': //palseBlock
				optionMax = 3;
				switch (optionCount) {
					case 0: //インターバルオン
						if (isRightInput) {
							((PulseField)editingScript).modeIntervalOn += 1;
						}
						if (isLeftInput) {
							((PulseField)editingScript).modeIntervalOn -= 1;
						}
						break;
                    case 1: //インターバルオフ
                        if (isRightInput) {
                            ((PulseField)editingScript).modeIntervalOff += 1;
                        }
                        if (isLeftInput) {
                            ((PulseField)editingScript).modeIntervalOff -= 1;
                        }
                        break;
                    case 2: //遅延
						if (isRightInput) {
							((PulseField)editingScript).delay += 1;
						}
						if (isLeftInput) {
							((PulseField)editingScript).delay -= 1;
						}
						break;
				}
				break;
            case 'F': //ピストン
                optionMax = 4;
                switch (optionCount) {
                    case 0: //インターバルオン
                        if (isRightInput) {
                            ((Piston)editingScript).pistonIntervalOn += 1;
                        }
                        if (isLeftInput) {
                            ((Piston)editingScript).pistonIntervalOn -= 1;
                        }
                        break;
                    case 1: //インターバルオフ
                        if (isRightInput) {
                            ((Piston)editingScript).pistonIntervalOff += 1;
                        }
                        if (isLeftInput) {
                            ((Piston)editingScript).pistonIntervalOff -= 1;
                        }
                        break;
                    case 2: //遅延
                        if (isRightInput) {
                            ((Piston)editingScript).delay += 1;
                        }
                        if (isLeftInput) {
                            ((Piston)editingScript).delay -= 1;
                        }
                        break;
                    case 3: //方向
                        if (isRightInput) {
                            ((Piston)editingScript).direction += 1;
                        }
                        if (isLeftInput) {
                            ((Piston)editingScript).direction -= 1;
                        }
                        break;
                }
				break;
			case 'I': //Start
				optionMax = 3;
				switch (optionCount) {
					case 0:
						if (isRightInput) {
							((StartBlock)editingScript).phaseCount++;
						}
						if (isLeftInput) {
							((StartBlock)editingScript).phaseCount--;
						}
						break;
					case 1:
						if (isRightInput) {
							((StartBlock)editingScript).turnMax++;
						}
						if (isLeftInput) {
							((StartBlock)editingScript).turnMax--;
						}
						break;
					case 2:
						if(isRightInput || isLeftInput) {
							((StartBlock)editingScript).isTutorial = !((StartBlock)editingScript).isTutorial;
						}
						break;
				}
				break;
			case 'J': //Goal
				optionMax = 1;
				if (isRightInput) {
					((Goal)editingScript).phaseCount++;
				}
				if (isLeftInput) {
					((Goal)editingScript).phaseCount--;
				}
				break;
		}
	}

	private void CameraControl() {
		Stage.instance.camera.transform.position = transform.position + new Vector3(0, cameraBaseRange + cameraMode * cameraStepRange, 0);
		Stage.instance.camera.transform.localEulerAngles = new Vector3(90, 0, 0);
	}

	private void InformationUpdate() {
		postionText.text = transform.position.x + "," + transform.position.z;

		objInfo.text = Stage.instance.objectList[objIndex].name;

		if (isObjListMode) {
			for (int i = 0; i < adjacentObjInfo.Count; i++) {
				int index = (objIndex + i - (adjacentObjInfo.Count / 2) + Stage.instance.objectList.Count) % Stage.instance.objectList.Count;
				adjacentObjInfo[i].text = Stage.instance.objectList[index].name;
			}
		}

		if (isDetailMode) {
			detailWindow.SetActive(true);
			detailOptionText.text = editingScript.ToEditorString(); //詳細編集のタイトル以外のテキスト
			switch (editingChar) {
				default:
					Debug.LogError("不明なオブジェクトの詳細編集をしています");
					detailObjectName.text = "不明なオブジェクト";
					detailOptionText.text = "バグ報告をしてください";
					break;
				case 'A':
					detailObjectName.text = "スイッチ";
					break;
				case 'B':
					detailObjectName.text = "ドア";
					break;
				case 'E':
					detailObjectName.text = "パルスフィールド";
					break;
                case 'F':
                    detailObjectName.text = "ピストン";
                    break;
				case 'I':
					detailObjectName.text = "スタート";
					break;
				case 'J':
					detailObjectName.text = "ゴール";
					break;
			}
		} else {
			detailWindow.SetActive(false);
		}
	}
}
