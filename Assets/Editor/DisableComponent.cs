using System.Linq;
using UnityEditor;
using UnityEngine;

public static class DisableComponent {
	private const int WIDTH = 16;

	[InitializeOnLoadMethod]
	private static void Example() {
		EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
	}

	private static void OnGUI(int instanceID, Rect selectionRect) {
		var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

		if (go == null) {
			return;
		}

		var isWarning = go
			.GetComponents<MonoBehaviour>()
			.Any(c => c == null);

		if (!isWarning) {
			return;
		}

		var pos = selectionRect;
		pos.x = 1000;
		pos.width = WIDTH;

		GUI.Label(pos, "!");
	}
}