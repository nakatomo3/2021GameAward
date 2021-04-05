using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostNormal : Ghost {
    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        Vector3 dist = Player.instance.transform.position - this.transform.position;
        CheckViewPlayer(this.transform.forward, dist);

    }
}
