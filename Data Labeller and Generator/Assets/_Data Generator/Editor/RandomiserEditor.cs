using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Randomiser), true)]
public class RandomiserEditor : AdvancedEditor
{
	public override void OnInspectorGUI()
	{
		if (GUILayout.Button("Randomise"))
		{
			(target as Randomiser).Randomise();
		}

		base.OnInspectorGUI();
	}
}
