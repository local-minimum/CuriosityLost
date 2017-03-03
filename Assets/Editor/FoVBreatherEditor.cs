using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CamFoVBreather))]
public class FoVBreatherEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CamFoVBreather myTarget = target as CamFoVBreather;

        float val = EditorGUILayout.FloatField("Cycle Duration", myTarget.cycleDuration);
        if (val != myTarget.cycleDuration)
        {
            myTarget.cycleDuration = val;
            serializedObject.ApplyModifiedProperties();
        }
    }
}
