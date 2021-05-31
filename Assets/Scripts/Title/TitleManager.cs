using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

    [SerializeField]
    private GameObject fade;

    // Start is called before the first frame update
    void Start() {
        AudioManeger.instance.PlayBGM(4);
    }

    // Update is called once per frame
    void Update() {
        if (Input.anyKeyDown) {
            var fadeOut = fade.AddComponent<FadeOut>();
            fadeOut.nextStagePath = "StageSelect";
            AudioManeger.instance.Play("GameStart");
        }
    }
}
