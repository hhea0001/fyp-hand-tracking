using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

[CustomEditor(typeof(Hand))]
public class HandEditor : RandomiserEditor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Setup Controller"))
		{
            SetupController();
		}
	}

    private void SetupController()
    {
        List<AnimationClip> clips = (target as Hand).Clips;

        AnimatorControllerLayer[] layers = new AnimatorControllerLayer[clips.Count];

        for (int i = 0; i < layers.Length; i++)
        {
            // Create default state for layer
            ChildAnimatorState[] layerStates = new ChildAnimatorState[] { new() { state = new() { motion = clips[i] } } };

            // Create layer and apply default state
            layers[i] = new()
            {
                name = clips[i].name,
                defaultWeight = 0,
                stateMachine = new()
                {
                    name = clips[i].name,
                    states = layerStates,
                    defaultState = layerStates[0].state
                }
            };
        }

        // Apply layers and mark as dirty
        var runtimeController = (target as Hand).Animator.runtimeAnimatorController;
        var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(runtimeController));
        controller.layers = layers;
        EditorUtility.SetDirty(controller);
    }
}
