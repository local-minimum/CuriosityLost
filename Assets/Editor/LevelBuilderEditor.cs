using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StepTiler))]
public class LevelBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reseed & Generate"))
            {
                (target as StepTiler).Reseed();
            }
            if (GUILayout.Button("Regenerate"))
            {
                (target as StepTiler).Generate();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
