using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Filter : MonoBehaviour {

	[SerializeField]
	private Renderer filter1;

	[SerializeField]
	private Renderer filter2;

	[SerializeField]
	private Renderer filter3;

	[SerializeField]
	private int stageType;

	private Vector2 getPosX;
	private Vector2 getPosY;

	private Material material1;
	private Material material2;
	private Material material3;

	private float scale = 0.05f;

	private void Start() {
		material1 = filter1.material;
		material2 = filter2.material;
		material3 = filter3.material;

		getPosX = new Vector2(Random.Range(0, 1.0f), Random.Range(0, 1.0f));
		getPosY = new Vector2(Random.Range(0, 1.0f), Random.Range(0, 1.0f));
	}

	// Update is called once per frame
	void Update() {
		getPosX += new Vector2(0.1f, 0.07f) * Time.deltaTime;
		getPosY += new Vector2(0.07f, 0.1f) * Time.deltaTime;
		var moveRangeX = Mathf.PerlinNoise(getPosX.x, getPosY.y) + 0.5f;
		var moveRangeY = Mathf.PerlinNoise(getPosX.x, getPosY.y) + 0.5f;
		switch (stageType) {
			case 1:
				material1.mainTextureOffset += new Vector2(moveRangeX, moveRangeY) * 1.0f * Time.deltaTime * scale;
				material2.mainTextureOffset += new Vector2(moveRangeX, moveRangeY) * 1.5f * Time.deltaTime * scale;
				material3.mainTextureOffset += new Vector2(moveRangeX, moveRangeY) * 2.0f * Time.deltaTime * scale;
				break;
		}
	}
}
