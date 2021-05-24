using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour {

    [SerializeField]
    private GameObject viewRangeVisual;

    [SerializeField]
    protected float viewAngle;

    [SerializeField]
    protected float viewDist;

    [HideInInspector]
    public int id = -1; //GhostManager.csのList参照用

    protected bool CheckViewPlayer(Vector3 viewDirection, Vector3 playerDistance) {
        if (Player.instance.isAlive == false) {
            return false;
        }

        var obj = Stage.instance.GetStageObject(Player.instance.newStepPos);
        if (obj != null) {
            var code = obj.name[0];
            int phase;
            if (code == 'I') {
                phase = obj.GetComponent<StartBlock>().phaseCount;
                if (((int)Mathf.Pow(2, Player.instance.phase) & phase) > 0) {
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
                    } else if (obj.name[0] == 'I') {
                        var phase = obj.GetComponent<StartBlock>().phaseCount;
                        if (((int)Mathf.Pow(2, Player.instance.phase) & phase) > 0) {
                            return false;
                        }
                    } else if (obj.name[0] == 'J') {
                        var phase = obj.GetComponent<Goal>().phaseCount;
                        if (((int)Mathf.Pow(2, Player.instance.phase) & phase) > 0) {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
    protected void ChangeViewRange() {
        GameObject obj;
        Vector3 defaultSize = new Vector3(0.1f, 1, 0.3f);
        Vector3 defaultPos = new Vector3(0, -0.5f, 2);
        for (int i = 0; i < 4; i++) {
            Vector3 getBlockPos = transform.position + transform.forward * i;
            getBlockPos.x = Mathf.Round(getBlockPos.x);
            getBlockPos.y = Mathf.Round(getBlockPos.y);
            getBlockPos.z = Mathf.Round(getBlockPos.z);
            obj = Stage.instance.GetStageObject(getBlockPos);

            bool isOcclution = false;//遮蔽物があるか

            if (obj != null) {
                if (obj.name[0] == '2') {
                    isOcclution = true;

                } else if (obj.name[0] == '3') {
                    isOcclution = true;
                } else if (obj.name[0] == 'I') {
                    var phase = obj.GetComponent<StartBlock>().phaseCount;
                    if (((int)Mathf.Pow(2, Player.instance.phase) & phase) > 0) {
                        isOcclution = true;
                    }
                } else if (obj.name[0] == 'J') {
                    var phase = obj.GetComponent<Goal>().phaseCount;
                    if (((int)Mathf.Pow(2, Player.instance.phase) & phase) > 0) {
                        isOcclution = true;
                    }
                }
            }

            if (isOcclution == true) {
                Vector3 dist = obj.transform.position - transform.position;
                float len = Vector3.Magnitude(dist);
                viewRangeVisual.transform.localPosition = new Vector3(defaultPos.x, defaultPos.y, len / 2);
                viewRangeVisual.transform.localScale = new Vector3(defaultSize.x, defaultSize.y, len * 0.1f);
                break;
            } else {
                viewRangeVisual.transform.localPosition = defaultPos;
                viewRangeVisual.transform.localScale = defaultSize;
            }

        }


    }
}
