using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBlock : MonoBehaviour {

	[SerializeField]
	private float percent;
	[SerializeField]
	private Renderer renderer;

	private static int beforeRand;

	// Start is called before the first frame update
	void Start() {
		var rand = 0;
		while(beforeRand == rand) {
			rand = Random.Range(0, 8);
		}
		beforeRand = rand;
		if(Random.Range(0, 100.0f) <= percent) {
			renderer.material.mainTextureOffset = new Vector2(rand % 4 * 0.25f, rand / 4 * 0.25f);
		}
	}
}
