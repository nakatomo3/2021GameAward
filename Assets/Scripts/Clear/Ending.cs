using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ending : MonoBehaviour {

    public Image fade;

    private float timer = 0;



    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;

        if (timer >= 3 && Input.anyKeyDown) {
            var fadeScript = fade.gameObject.AddComponent<FadeOut>();
            fadeScript.nextStagePath = "Title";
        }
    }
}
