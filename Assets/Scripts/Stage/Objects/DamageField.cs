using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageField : MonoBehaviour {

    private bool isDamage = true;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (transform.position == Player.instance.transform.position && isDamage == true) {
            Player.instance.Damage(1); // ダメージ処理
            isDamage = false;
        } else { // プレイヤーが離れたらリセット
            isDamage = true;
        }
    }
}
