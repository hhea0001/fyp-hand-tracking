using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataGenerator))]
public class DataGeneratorEditor : RandomiserEditor
{
	protected override Transform ViewTarget => Camera.main.transform;
	protected override bool ResetView => true;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Generate Data Set"))
		{
			string directory = EditorUtility.OpenFolderPanel("Save Data to...", "Assets/", "");

			if (directory != string.Empty)
			{
				(target as DataGenerator).GenerateData(directory);
			}
		}
	}
}
