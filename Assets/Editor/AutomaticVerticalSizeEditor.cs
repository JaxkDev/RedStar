/******************************************
 * License - OSL-3.0                      *
 * Created by JaxkDev (JaxkDev@gmail.com) *
 ******************************************/

using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(AutomaticVerticalSize))]
public class AutomaticVerticalSizeEditor : Editor {

	public override void OnInspectorGUI(){
		DrawDefaultInspector();

		if (GUILayout.Button("Re-calculate size")) {
			AutomaticVerticalSize script = ((AutomaticVerticalSize)target);
			script.AdjustSize();
		}
	}
}
