using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable CS0108

public class NormalBlock : MonoBehaviour {

	[SerializeField]
	private float percent;
	[SerializeField]
	private Renderer renderer;

	[SerializeField]
	private Material[] materials;

	private static int beforeRand;

	[SerializeField]
	private MeshFilter meshFilter;

	[SerializeField]
	private Mesh plate;

	private bool wasSet = false;
	private static int plateCount = 0;

	// Start is called before the first frame update
	void Start() {
		renderer.material = materials[Stage.instance.visualMode];

		var rand = 0;
		while(beforeRand == rand) {
			rand = Random.Range(0, 8);
		}
		beforeRand = rand;
		if(Random.Range(0, 100.0f) <= percent) {
			renderer.material.mainTextureOffset = new Vector2(rand % 4 * 0.25f, rand / 4 * 0.75f);
		}
	}

	private void Update() {
		plateCount = 0;
	}

	private void LateUpdate() {
		if(wasSet == false && plateCount <= 10) { //プレートチェックは1フレームに10個まで
			var isPlate = true;
			for(int i = 0; i < 3; i++) {
				for(int j = 0; j < 3; j++) {
					if(i == 1 && j == 1) {
						continue;
					}
					if (Stage.instance.GetStageObject(transform.position + Vector3.right * (i - 1) + Vector3.forward * (j - 1)) == null) {
						isPlate = false;
						break;
					}
				}
			}
			if (isPlate) {
				meshFilter.mesh = plate;
			}
			plateCount++;
			Destroy(this);
		}
	}
}
