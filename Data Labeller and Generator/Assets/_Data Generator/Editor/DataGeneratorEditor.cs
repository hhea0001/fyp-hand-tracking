using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DataGenerator))]
public class DataGeneratorEditor : RandomiserEditor
{
	protected override Transform ViewTarget => Camera.main.transform;
	protected override bool ResetView => true;
}
