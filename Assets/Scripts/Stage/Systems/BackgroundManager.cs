using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundManager : MonoBehaviour {

    [SerializeField]
    private Material world4Material;
    private Renderer background;

    private float timer;

    private bool wasCheck;

    // Start is called before the first frame update
    void Start() {
        background = GameObject.Find("Background").GetComponent<Renderer>();

        
    }

    // Update is called once per frame
    void Update() {
        if(wasCheck == false) {
            if (Stage.instance.visualMode == 3) {
                background.material = world4Material;
            }
            wasCheck = true;
        }
    }
}
