using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBlock : MonoBehaviour {

    [SerializeField]
    private Renderer render;

    private float breakCount = 0;
    private float generationCount = 0;
    private float breakInterval = 3.0f;         // 壊れるまでの時間
    private float regenerationInterval = 7.0f;  // 壊れてから戻る時間

    private bool isBreak = false;
    private bool isBroken = false;

    // Start is called before the first frame update
    void Start() {
        isBreak = false;
        isBroken = false;
        render = GetComponentInChildren<Renderer>();
    }

    // Update is called once per frame
    void FixedUpdate() {
        
        if (isBroken == true) { // 崩落している状態
            if (transform.position == Player.instance.transform.position) {
                Player.instance.Fall(); // 死
                return;
            }
            if(generationCount >= regenerationInterval) {
                generationCount = 0;
                isBroken = false; // 復旧
                Color color = render.material.color;
                color.r = 0.3f;
                color.g = 0.3f;
                color.b = 0.3f;
                color.a = 1.0f;
                render.material.color = color;
            }
            generationCount += Time.deltaTime;

        } else if (transform.position == Player.instance.transform.position && isBreak == false) {
            isBreak = true; // 崩落カウント開始
            Color color = render.material.color;
            color.r = 1.0f;
            color.g = 0;
            color.b = 0;
            render.material.color = color;

        } else if (isBreak == true) { // もうすぐ崩落する状態
            if (breakCount >= breakInterval) {
                breakCount = 0;
                isBreak = false;
                isBroken = true; // 崩落状態
                Color color = render.material.color;
                color.a = 0;
                render.material.color = color;
            }
            breakCount += Time.fixedDeltaTime;
        }
    }
}
