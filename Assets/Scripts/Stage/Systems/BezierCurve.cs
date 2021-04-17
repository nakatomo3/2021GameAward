using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve {

	private Vector3[] points = new Vector3[3];

	/// <summary>
	/// 2ŽŸƒxƒWƒF
	/// </summary>
	public BezierCurve(Vector3 start, Vector3 center, Vector3 end) {
		points[0] = start;
		points[1] = center;
		points[2] = end;
	}

	public void SetPoint(Vector3 start, Vector3 center, Vector3 end) {
		points[0] = start;
		points[1] = center;
		points[2] = end;
	}

	public Vector3 GetPoint(float t) {
		var value = (1 - t) * (1 - t) * points[0] +
					2 * t * (1 - t) * points[1] +
					t * t * points[2];
		return value;
	}
}
