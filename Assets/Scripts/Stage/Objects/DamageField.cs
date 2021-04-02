using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageField : MonoBehaviour {

    [SerializeField]
    private Renderer render;

    private bool isOn = true;
    private float count = 0.0f; 
    private float interval = 2.0f;

    // Start is called before the first frame update
    void Start() {
        render = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update() {
        if (transform.position == Player.instance.transform.position) {
            if (isOn == false) {
                count += Time.deltaTime;
                if (count >= interval) { // インターバルカウント
                    count = 0.0f;
                    isOn = true;
                    Color color = render.material.color;
                    color.g = 1.0f;
                    render.material.color = color;
                }
            } else {
                Player.instance.Damage(4); // ダメージ処理
                isOn = false;
                Color color = render.material.color;
                color.g = 0.0f;
                render.material.color = color;
            }
        } else {
            count = 0.0f;
            isOn = true;
            Color color = render.material.color;
            color.g = 1.0f;
            render.material.color = color;
        }
    }
}
