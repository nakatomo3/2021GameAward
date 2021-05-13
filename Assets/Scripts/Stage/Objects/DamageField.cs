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
            Player.instance.Damage(1); // �_���[�W����
            isDamage = false;
        } else { // �v���C���[�����ꂽ�烊�Z�b�g
            isDamage = true;
        }
    }
}
