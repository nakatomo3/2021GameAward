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
	private MonoBehaviour editingScript;
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
		if (InputManager.GetKey(Keys.A)) {
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

		if (InputManager.GetKey(Keys.B)) {
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

		if (InputManager.GetKeyDown(Keys.Y) && isAreaMode == false) {
			var obj = Stage.instance.GetStageObject(transform.position);
			if (obj == null) {
				return;
			}
			var code = Stage.GetObjectCode(obj.name);
			if (code >= 'A' && code <= 'Z') { //大文字なら詳細編集可能なコード
				isDetailMode = true;
				editingScript = obj.GetComponent<MonoBehaviour>();
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
		if (InputManager.GetKeyUp(Keys.Y)) {
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

		if ((ChannelBase)editingScript != null && optionCount == 0) {
			if (isLeftInput) {
				((ChannelBase)editingScript).channel--;
			}
			if (isRightInput) {
				((ChannelBase)editingScript).channel++;
			}
		}
		optionCursor.anchoredPosition = new Vector2(-125, 105 + optionCount * -30);

		switch (editingChar) {
			case 'A':
				optionMax = 1;
				break;
			case 'B':
				optionMax = 1;
				//2:反転モード
				//3:ディレイ設定
				break;
		}
	}

	private void CameraControl() {
		if (InputManager.GetKeyDown(Keys.X)) {
			cameraMode++;
			cameraMode = (cameraMode + cameraMax) % cameraMax;
		}

		Stage.instance.camera.transform.position = transform.position + new Vector3(0, cameraBaseRange + cameraMode * cameraStepRange, 0);

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
			switch (editingChar) {
				default:
					Debug.LogError("不明なオブジェクトの詳細編集をしています");
					detailObjectName.text = "不明なオブジェクト";
					detailOptionText.text = "バグ報告をしてください";
					break;
				case 'A':
					detailObjectName.text = "スイッチ";
					detailOptionText.text = ((Switch)editingScript).ToEditorString();
					break;
				case 'B':
					detailObjectName.text = "スイッチ";
					detailOptionText.text = ((Door)editingScript).ToEditorString();
					break;
			}
		} else {
			detailWindow.SetActive(false);
		}
	}
}
