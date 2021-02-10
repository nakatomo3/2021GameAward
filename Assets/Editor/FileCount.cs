using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class FileCount {
	private const string REMOVE_STR = "Assets";

	private static readonly int mRemoveCount = REMOVE_STR.Length;
	private static readonly Color mColor = new Color(0.635f, 0.635f, 0.635f, 1);

	[InitializeOnLoadMethod]
	private static void Example() {
		EditorApplication.projectWindowItemOnGUI += OnGUI;
	}

	private static void OnGUI(string guid, Rect selectionRect) {
		var dataPath = Application.dataPath;
		var startIndex = dataPath.LastIndexOf(REMOVE_STR);
		var dir = dataPath.Remove(startIndex, mRemoveCount);
		var path = dir + AssetDatabase.GUIDToAssetPath(guid);

		if (!Directory.Exists(path)) {
			return;
		}

		var fileCount = Directory
			.GetFiles(path)
			.Where(c => !c.EndsWith(".meta"))
			.Count();

		var dirCount = Directory
			.GetDirectories(path)
			.Length;

		var count = fileCount + dirCount;

		if (count == 0) {
			return;
		}

		var text = count.ToString();
		var label = EditorStyles.label;
		var content = new GUIContent(text);
		var width = label.CalcSize(content).x;

		var pos = selectionRect;
		pos.x = pos.xMax - width * 1.1f;
		pos.width = width;
		pos.yMin++;

		var color = GUI.color;
		GUI.color = mColor;
		//GUI.DrawTexture(pos, EditorGUIUtility.whiteTexture);
		GUI.color = color;
		GUI.Label(pos, text);
	}
}