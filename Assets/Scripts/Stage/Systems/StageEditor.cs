using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0108

public class StageEditor : MonoBehaviour {

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
			_editorPhase = (value + 8) % 8;
		}
	}

	private bool isDetailMode = false;

	private float inputTimer = 0;
	private const float firstInterval = 0.5f;
	private const float continuousInterval = 0.1f;

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

    private GameObject camera;

	#endregion


	void Start() {
        camera = Stage.instance.camera;
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
            camera.transform.position = new Vector3(transform.position.x, 10, transform.position.z);
            camera.transform.localEulerAngles = Vector3.right * 90;
		}

		if (InputManager.GetKeyDown(Keys.Y)) {
			editorPhase++;
		}
		phaseText.text = (editorPhase + 1).ToString();
		if (editorPhase == 7) {
			phaseText.text = "全部";
		}

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
						if (new Vector3(i, 0, -j) != Vector3.zero) {
							Stage.instance.GenerateObject(leftUp + new Vector3(i, 0, -j), Stage.instance.objectList[objIndex]);
						}
					}
				}
			} else {
				if (transform.position != Vector3.zero) {
					Stage.instance.GenerateObject(transform.position, Stage.instance.objectList[objIndex]);
				}
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
						if (new Vector3(i, 0, -j) != Vector3.zero) {
							DestroyObj(leftUp + new Vector3(i, 0, -j));
						}
					}
				}
			} else {
				if (transform.position != Vector3.zero) {
					DestroyObj(transform.position);
				}
			}



		}

		if (InputManager.GetKeyDown(Keys.X) && isAreaMode == false) {
			var obj = Stage.instance.GetStageObject(transform.position);
			if (obj == null) {
				return;
			}
			var code = Stage.instance.GetObjectCode(obj.name);
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
			case 'E': //pulseBlock
				optionMax = 3;
				switch (optionCount) {
					case 0: //インターバルオン
						if (isRightInput) {
							((PulseField)editingScript).modeIntervalOn ++;
						}
						if (isLeftInput) {
							((PulseField)editingScript).modeIntervalOn --;
						}
						break;
					case 1: //インターバルオフ
						if (isRightInput) {
							((PulseField)editingScript).modeIntervalOff ++;
						}
						if (isLeftInput) {
							((PulseField)editingScript).modeIntervalOff --;
						}
						break;
					case 2: //遅延
						if (isRightInput) {
							((PulseField)editingScript).delay ++;
						}
						if (isLeftInput) {
							((PulseField)editingScript).delay --;
						}
						break;
				}
				break;
			case 'F': //ピストン
				optionMax = 4;
				switch (optionCount) {
					case 0: //インターバルオン
						if (isRightInput) {
							((Piston)editingScript).pistonIntervalOn ++;
						}
						if (isLeftInput) {
							((Piston)editingScript).pistonIntervalOn --;
						}
						break;
					case 1: //インターバルオフ
						if (isRightInput) {
							((Piston)editingScript).pistonIntervalOff ++;
						}
						if (isLeftInput) {
							((Piston)editingScript).pistonIntervalOff --;
						}
						break;
					case 2: //遅延
						if (isRightInput) {
							((Piston)editingScript).delay ++;
						}
						if (isLeftInput) {
							((Piston)editingScript).delay --;
						}
						break;
					case 3: //方向
						if (isRightInput) {
							((Piston)editingScript).direction ++;
						}
						if (isLeftInput) {
							((Piston)editingScript).direction --;
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
						if (isRightInput || isLeftInput) {
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
            case 'K': //Krawler
                optionMax = 5;
                switch (optionCount) {
                    case 0: //X座標移動範囲
                        if (isRightInput) {
                            ((Krawler)editingScript).moveRangeX ++;
                        }
                        if (isLeftInput) {
                            ((Krawler)editingScript).moveRangeX --;
                        }
                        break;
                    case 1: //Z座標移動範囲
                        if (isRightInput) {
                            ((Krawler)editingScript).moveRangeZ ++;
                        }
                        if (isLeftInput) {
                            ((Krawler)editingScript).moveRangeZ --;
                        }
                        break;
                    case 2: //インターバル
                        if (isRightInput) {
                            ((Krawler)editingScript).interval ++;
                        }
                        if (isLeftInput) {
                            ((Krawler)editingScript).interval --;
                        }
                        break;
                    case 3: //ダメージ量
                        if (isRightInput) {
                            ((Krawler)editingScript).damage ++;
                        }
                        if (isLeftInput) {
                            ((Krawler)editingScript).damage --;
                        }
                        break;
                    case 4: // フェーズ
                        if (isRightInput) {
                            ((Krawler)editingScript).phaseCount++;
                        }
                        if (isLeftInput) {
                            ((Krawler)editingScript).phaseCount--;
                        }
                        break;
                }
                break;
        }
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
                case 'K':
                    detailObjectName.text = "クローラ";
                    break;
            }
		} else {
			detailWindow.SetActive(false);
		}
	}
}
