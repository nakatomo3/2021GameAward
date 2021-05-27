using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleGate : MonoBehaviour {

    [SerializeField]
    private Renderer startObject;

    private float timer;

    // Start is called before the first frame update
    void Start() {
        startObject.material.EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        var colorChannel = Mathf.Pow(2, Mathf.Abs(Mathf.Sin(timer) * 2));
        startObject.material.SetColor("_EmissionColor", new Color(colorChannel, colorChannel, colorChannel));
    }
}
