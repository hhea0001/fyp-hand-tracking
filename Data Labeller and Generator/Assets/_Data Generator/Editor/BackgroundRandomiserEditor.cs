using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(BackgroundRandomiser))]
public class BackgroundRandomiserEditor : RandomiserEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Load Textures"))
		{
			string directory = EditorUtility.OpenFolderPanel("Open Folder", "Assets/", "");

			if (directory != string.Empty)
			{
				LoadFolder(directory);
			}
		}
	}

	private void LoadFolder(string directory)
	{
		string path = ProjectPath(directory);

		string[] guids = AssetDatabase.FindAssets("t:texture2d t:cubemap", new string[] { path });

		IEnumerable<Texture> textures = guids
			.Select(guid => AssetDatabase.GUIDToAssetPath(guid))
			.Select(assetPath => AssetDatabase.LoadAssetAtPath<Texture>(assetPath));

		(target as BackgroundRandomiser).backgroundCubemaps = textures
			.Where(asset => asset is Cubemap)
			.Select(asset => asset as Cubemap)
			.ToArray();

		(target as BackgroundRandomiser).backgroundTextures = textures
			.Where(asset => asset is Texture2D)
			.Select(asset => asset as Texture2D)
			.ToArray();
	}

	private string ProjectPath(string directory)
	{
		return "Assets" + directory.Replace(Application.dataPath, "").Replace("\\", "/");
	}
}
