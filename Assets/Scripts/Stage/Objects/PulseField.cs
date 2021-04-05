using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseField : MonoBehaviour {

    [SerializeField]
    private Renderer render;

    private float pulseTimer;
    public float modeInterval = 2.0f; // パルス発生間隔
    public float delay = 0;           // 遅延
    private bool isPulse = false;

    private bool isDamage = true;
    private float count = 0.0f;
    private float DamageInterval = 1.0f;

    // Start is called before the first frame update
    void Start() {
        render = GetComponentInChildren<Renderer>();
        pulseTimer = -delay;
    }

    // Update is called once per frame
    void Update() {
        pulseTimer += Time.deltaTime;

        if (pulseTimer >= modeInterval) {
            pulseTimer = 0;
            isPulse = !isPulse; // パルスONOFFを切り替える
        }

        if (isPulse == true) { // パルスONの時はダメージブロックと同等の処理
            Color color = render.material.color;
            color.a = 1.0f;
            render.material.color = color;
            if (transform.position == Player.instance.transform.position) {
                if (isDamage == false) {
                    count += Time.deltaTime;
                    if (count >= DamageInterval) { // インターバルカウント
                        count = 0.0f;
                        isDamage = true;
                        color = render.material.color;
                        color.g = 1.0f;
                        render.material.color = color;
                    }
                } else {
                    Player.instance.Damage(4); // ダメージ処理
                    isDamage = false;
                    color = render.material.color;
                    color.g = 0.0f;
                    render.material.color = color;
                }
            } else { // プレイヤーが離れたらリセット
                count = 0.0f;
                isDamage = true;
                color = render.material.color;
                color.g = 1.0f;
                render.material.color = color;
            }
        } else {
            Color color = render.material.color;
            color.a = 0.2f;
            render.material.color = color;
        }
    }
}
