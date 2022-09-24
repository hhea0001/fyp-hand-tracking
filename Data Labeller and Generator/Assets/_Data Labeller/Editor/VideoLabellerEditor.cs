using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VideoLabeller))]
public class VideoLabellerEditor : AdvancedEditor
{
	protected override Transform ViewTarget => Camera.main.transform;
	protected override bool ResetView => true;

	private VideoLabeller _labeller;
	private bool _mouseDown = false;

	public override void OnInspectorGUI()
	{
		_labeller = target as VideoLabeller;

		DrawDefaultInspector();
		EditorGUILayout.Space();
		DrawOpenVideoButton();
		DrawSaveDataButton();
	}

	private void OnSceneGUI()
	{
		HandleInput();
	}

	private void DrawOpenVideoButton()
	{
		if (GUILayout.Button("Open Video"))
		{
			string filename = EditorUtility.OpenFilePanelWithFilters("Open Video", "", new string[]
			{
				"Video Files", "mp4,mov,m4v,avi",
			});

			if (filename != string.Empty)
			{
				_labeller.OpenVideo(filename);
			}
		}
	}

	private void DrawSaveDataButton()
	{
		if (GUILayout.Button("Save Data"))
		{
			string folder = EditorUtility.OpenFolderPanel("Save Directory", "", "");

			if (folder != string.Empty)
			{
				_labeller.SaveData(folder);
			}
		}
	}

	private void HandleInput()
	{
		Event e = Event.current;

		if (e.type == EventType.MouseDown)
		{
			_mouseDown = true;
			_labeller.SetCurrentPosition(GetMousePosition());
			_labeller.Play();
		}

		if (e.type == EventType.MouseUp)
		{
			_mouseDown = false;
			_labeller.Pause();
		}

		if (e.type == EventType.Repaint && _mouseDown)
		{
			_labeller.SetCurrentPosition(GetMousePosition());
		}

		ConsumeInput();
	}

	private void ConsumeInput()
	{
		Event e = Event.current;

		if (e.type == EventType.Layout)
		{
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(GetHashCode(), FocusType.Passive));
		}
		else if (e.type != EventType.Repaint)
		{
			e.Use();
		}
	}

	private Vector2 GetMousePosition()
	{
		SceneView sceneView = SceneView.lastActiveSceneView;

		Vector3 mousePosition = Event.current.mousePosition;
		mousePosition.z = 1;

		Vector3 worldPosition = sceneView.camera.ScreenToWorldPoint(mousePosition);
		Vector3 cameraPosition = Camera.main.WorldToViewportPoint(worldPosition);
		return cameraPosition;
	}
}
