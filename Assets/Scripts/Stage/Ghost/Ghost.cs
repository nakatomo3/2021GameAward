using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

	[SerializeField]
	protected float viewAngle;

	[SerializeField]
	protected float viewDist;

	[HideInInspector]
	public int id = -1; //GhostManager.csのList参照用

	// Start is called before the first frame update
	void Start() {

	}

	// Update is called once per frame
	void Update() {

	}

	protected bool CheckViewPlayer(Vector3 viewDirection, Vector3 playerDistance) {
		var obj = Stage.instance.GetStageObject(Player.instance.transform.position);
		if (obj != null) {
			var code = obj.name[0];
			int phase;
			if (code == 'I') {
				phase = obj.GetComponent<StartBlock>().phaseCount;
				if(((int)Mathf.Pow(2, Player.instance.phase) & phase) > 0) {
					return false;
				}
			}
			if (code == 'J') {
				phase = obj.GetComponent<Goal>().phaseCount;
				if (((int)Mathf.Pow(2, Player.instance.phase) & phase) > 0) {
					return false;
				}
			}
		}

		//プレイヤーと同じ位置にいる
		if (GhostManager.instance.newPos[id] == Player.instance.newStepPos ||
			GhostManager.instance.oldPos[id] == Player.instance.newStepPos) {
			Player.instance.GhostGameOver();
			return true;
		}

		//視線方向にいるか
		Vector3 distance = playerDistance;
		Vector3 direction = Vector3.Normalize(distance);
		float len = Mathf.Pow(Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) + Mathf.Pow(distance.z, 2), 0.5f);

		//プレイヤーとの距離のベクトルと正面方向のベクトルの間の角度を求める
		float dot = Vector3.Dot(direction, viewDirection);
		float angle = Mathf.Acos(dot);

		bool isIntoView = Mathf.Abs(angle) < viewAngle / 180 * 3.14 && len < viewDist;
		bool isViewForward = viewDirection == direction && len <= viewDist;
		if (isIntoView == true || isViewForward == true) {
			Vector3 playerPos = Player.instance.newStepPos;
			Vector3 thisPos = this.transform.position;


			float loopAdd = 1 / viewDist;
			for (float j = loopAdd; j < 1; j += loopAdd) {
				Vector3 temp = Vector3.Lerp(thisPos, playerPos, j);
				temp.x = Mathf.Round(temp.x);
				temp.y = Mathf.Round(temp.y);
				temp.z = Mathf.Round(temp.z);

				obj = Stage.instance.GetStageObject(temp);

				//壁に隠れているか判定する
				if (obj != null) {
					if (obj.name[0] == '2') {
						return false;

					} else if (obj.name[0] == '3') {
						return false;
					}
				}
			}

			Player.instance.GhostGameOver();
			return true;
		}



		return false;
	}

}
