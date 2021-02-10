using System.Linq;
using UnityEditor;
using UnityEngine;

public static class ComponentIcon {
	private static readonly Color mDisabledColor = new Color(1, 1, 1, 0.5f);

	private const int WIDTH = 8;
	private const int HEIGHT = 8;

	[InitializeOnLoadMethod]
	private static void Example() {
		EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
	}

	private static void OnGUI(int instanceID, Rect selectionRect) {
		var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

		if (go == null) {
			return;
		}

		var pos = selectionRect;
		pos.x = pos.xMax - WIDTH;
		pos.width = WIDTH;
		pos.height = HEIGHT;

		var components = go
			.GetComponents<Component>()
			.Where(c => c != null)
			.Where(c => !(c is Transform))
			.Reverse();

		var current = Event.current;

		foreach (var c in components) {
			Texture image = AssetPreview.GetMiniThumbnail(c);

			if (image == null && c is MonoBehaviour) {
				var ms = MonoScript.FromMonoBehaviour(c as MonoBehaviour);
				var path = AssetDatabase.GetAssetPath(ms);
				image = AssetDatabase.GetCachedIcon(path);
			}

			if (image == null) {
				continue;
			}

			var color = GUI.color;
			GUI.color = c.IsEnabled() ? Color.white : mDisabledColor;
			GUI.DrawTexture(pos, image, ScaleMode.ScaleToFit);
			GUI.color = color;
			pos.x -= pos.width;
		}
	}

	public static bool IsEnabled(this Component self) {
		if (self == null) {
			return true;
		}

		var type = self.GetType();
		var property = type.GetProperty("enabled", typeof(bool));

		if (property == null) {
			return true;
		}

		return (bool)property.GetValue(self, null);
	}

}