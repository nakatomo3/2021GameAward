using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SystemSupporter {
	public static void ExitGame() {
		#if UNITY_EDITOR
			EditorApplication.isPlaying = false;
		#endif
	}

	
}
