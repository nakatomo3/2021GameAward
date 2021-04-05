using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseField : MonoBehaviour {

    [SerializeField]
    private Renderer render;

    private float pulseTimer;
    public float modeInterval = 2.0f; // モード切替間隔
    public float delay = 0;           // 遅延
    private bool isPulse = false;

    private bool isDamage = true;
    private float timer = 0.0f;
    private float damageInterval = 1.0f;

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
            isPulse = !isPulse; // モードを切り替える
        }

        if (isPulse == true) { // パルスONの時はダメージブロックと同等の処理
            Color color = render.material.color;
            color.a = 1.0f;
            render.material.color = color;
            if (transform.position == Player.instance.transform.position) {
                if (isDamage == false) {
                    timer += Time.deltaTime;
                    if (timer >= damageInterval) { // インターバルカウント
                        timer = 0.0f;
                        isDamage = true;
                        color = render.material.color;
                        color.g = 1.0f;
                        render.material.color = color;
                    }
                } else {
                    Player.instance.Damage(1); // ダメージ処理
                    isDamage = false;
                    color = render.material.color;
                    color.g = 0.0f;
                    render.material.color = color;
                }
            } else { // プレイヤーが離れたらリセット
                timer = 0.0f;
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
