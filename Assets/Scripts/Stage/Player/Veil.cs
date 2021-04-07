using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Veil : MonoBehaviour {

    [HideInInspector]
    public float shotPower = 0;

    private Vector3 firstPos;

    // Start is called before the first frame update
    void Start() {
        firstPos = this.transform.position;
    }

    // Update is called once per frame
    void Update() {

    }

    void FixedUpdate() {
        this.transform.position += this.transform.forward * shotPower * Time.fixedDeltaTime;

        Vector3 distance = this.transform.position - firstPos;
        float len = Mathf.Pow(Mathf.Pow(distance.x, 2) + Mathf.Pow(distance.y, 2) + Mathf.Pow(distance.z, 2), 0.5f);

        if (len >= shotPower) {
            Destroy(this.gameObject);
        }
    }
}
