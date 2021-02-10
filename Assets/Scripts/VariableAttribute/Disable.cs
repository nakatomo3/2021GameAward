using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Disable))]
public class Drawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginDisabledGroup(true);
		EditorGUI.PropertyField(position, property, label);
		EditorGUI.EndDisabledGroup();
	}
}
#endif

public class Disable : PropertyAttribute {
	//空のクラス。属性がこれになる
	//Inspectorで見たいが、値は編集させたくないものに使う。
}