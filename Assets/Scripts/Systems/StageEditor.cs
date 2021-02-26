using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEditor : MonoBehaviour {

	[SerializeField]
	private bool isController = false;

	#region インスペクタ参照部
	[SerializeField]
	private GameObject cursor;

	[SerializeField]
	private Text postionText;

	[SerializeField]
	private Text objInfo;

	[SerializeField]
	private List<Text> adjacentObjInfo;
	[SerializeField]
	private GameObject objList;
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

		if (InputManager.GetKeyDown(Keys.X)) {
			cameraMode++;
			cameraMode = (cameraMode + cameraMax) % cameraMax;
		}

		Stage.instance.camera.transform.position = transform.position + new Vector3(0, cameraBaseRange + cameraMode * cameraStepRange, 0);

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
		if(InputManager.GetKey(Keys.L) || InputManager.GetKey(Keys.R)) {
			objTimer += Time.unscaledDeltaTime;
		} else {
			objTimer = 0;
		}
		if(objTimer > 1) {
			isObjListMode = true;
		}

		//オブジェクト置く消す
		if (InputManager.GetKey(Keys.A)) {
			bool isExistObj = false;
			Transform child = transform; //未割当回避
			string childName = "";
			for (int i = 0; i < Stage.instance.stageParent.transform.childCount; i++) {
				child = Stage.instance.stageParent.transform.GetChild(i);
				if (child.transform.position == transform.position) {
					isExistObj = true;
					childName = child.name;
					break;
				}
			}
			if (isExistObj == false ) {
				Instantiate(Stage.instance.objectList[objIndex], transform.position, Quaternion.identity, Stage.instance.stageParent.transform);
			} else if(childName != Stage.instance.objectList[objIndex].name + "(Clone)"){
				Destroy(child.gameObject);
				Instantiate(Stage.instance.objectList[objIndex], transform.position, Quaternion.identity, Stage.instance.stageParent.transform);
			}
		}

		if (InputManager.GetKey(Keys.B)) {
			for (int i = 0; i < Stage.instance.stageParent.transform.childCount; i++) {
				var child = Stage.instance.stageParent.transform.GetChild(i);
				if (child.transform.position == transform.position) {
					Destroy(child.gameObject);
					break;
				}
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

	}

	private void InformationUpdate() {
		postionText.text = transform.position.x + "," + transform.position.z;

		objInfo.text = Stage.instance.objectList[objIndex].name;

		if (isObjListMode) {
			for(int i = 0; i < adjacentObjInfo.Count; i++) {
				int index = (objIndex + i - (adjacentObjInfo.Count / 2) + Stage.instance.objectList.Count) % Stage.instance.objectList.Count;
				adjacentObjInfo[i].text = Stage.instance.objectList[index].name;
			}
		}
	}
}
