using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    public static bool isAlive = true;
    public static Vector3 pos;

    // Start is called before the first frame update
    void Start() {
        pos = this.transform.position;
    }

    // Update is called once per frame
    void Update() {
        this.transform.position = pos;
        if (Player.instance.transform.position == this.transform.position) {
            isAlive = false;
        }

        for (int i = 0; i < GhostManager.instance.ghosts.Count; ++i) {
            if (GhostManager.instance.ghosts[i].transform.position == this.transform.position) {
                isAlive = false;
            }
        }

        this.transform.GetChild(0).gameObject.SetActive(isAlive);
    }


}
