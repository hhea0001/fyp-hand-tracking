using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class AdvancedEditor : Editor
{
	[SerializeField, HideInInspector] private Vector3 _oldCameraPosition;
	[SerializeField, HideInInspector] private Quaternion _oldCameraRotation;

	protected virtual Transform ViewTarget => null;
	protected virtual bool SetView => ViewTarget != null;
	protected virtual bool ResetView => false;

	private void OnEnable()
	{
		AlignSceneViewToViewTarget();
	}

	private void OnDisable()
	{
		ResetSceneView();
	}

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		Type type = target.GetType();
		MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

		foreach (MethodInfo method in methods)
		{
			ButtonAttribute button = method.GetCustomAttribute<ButtonAttribute>();
			if (button != null && GUILayout.Button(button.text))
			{
				method.Invoke(target, null);
			}
		}
	}

	private void AlignSceneViewToViewTarget()
	{
		if (!SetView) return;

		try
		{
			SceneView sceneView = SceneView.lastActiveSceneView;
			_oldCameraPosition = sceneView.camera.transform.position;
			_oldCameraRotation = sceneView.camera.transform.rotation;
			sceneView.AlignViewToObject(ViewTarget);
		}
		catch { }
	}

	private void ResetSceneView()
	{
		if (!SetView || !ResetView) return;

		try
		{
			SceneView sceneView = SceneView.lastActiveSceneView;
			sceneView.camera.transform.SetPositionAndRotation(_oldCameraPosition, _oldCameraRotation);
			sceneView.AlignViewToObject(sceneView.camera.transform);
		}
		catch { }
	}
}