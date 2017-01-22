using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TransformRecorder))]
public class TransformRecorderEditor : Editor {

    int selectedIndex = -1;
    int editingIndex = -1;

    public override void OnInspectorGUI()
    {
        TransformRecorder tRec = (TransformRecorder)target;

        if (GUILayout.Button("Record"))
        {
            tRec.RecordNew();
        }

        bool isRecordedState = tRec.IsCurrentStateRecorded;
        if (!isRecordedState)
        {
            EditorGUILayout.HelpBox("Current transform settings not recorded!", MessageType.Warning);
        }

        GUILayout.Space(5);
        GUILayout.Label("Recorded states");
        EditorGUI.indentLevel+=2;
        SerializedProperty rData = serializedObject.FindProperty("recordedData");        
        for (int i=0, l=rData.arraySize; i< l; i++)
        {
            RecordedEntry(i, rData.GetArrayElementAtIndex(i), tRec);
        }
        EditorGUI.indentLevel-=2;

        if (editingIndex >= 0 && editingIndex < rData.arraySize)
        {
            GUILayout.Space(5);
            GUILayout.Label("Edit State");

            EditorGUI.BeginChangeCheck();
            SerializedProperty prop = rData.GetArrayElementAtIndex(editingIndex);

            SerializedProperty nProp = prop.FindPropertyRelative("name");
            nProp.stringValue = EditorGUILayout.TextField("Name", nProp.stringValue);

            if (!isRecordedState && GUILayout.Button("Re-record values"))
            {
                tRec.UpdateState(editingIndex);
                serializedObject.ApplyModifiedProperties();
                editingIndex = -1;
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

    int boxWidth = 20;

    void RecordedEntry(int index, SerializedProperty prop, TransformRecorder tRec)
    {

        bool isSelected = (selectedIndex == index);
        EditorGUILayout.BeginHorizontal();

        if (!isSelected) {
            if (GUILayout.Button(prop.FindPropertyRelative("name").stringValue)) {
                selectedIndex = index;
                tRec.ApplyState(index);
                editingIndex = -1;
            }
        } else
        {
            GUILayout.Label(prop.FindPropertyRelative("name").stringValue);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("E", GUILayout.Width(boxWidth)))
            {
                editingIndex = index;
            }

            if (GUILayout.Button("X", GUILayout.Width(boxWidth)))
            {
                editingIndex = -1;
                tRec.RemoveState(index);
                if (selectedIndex >= index)
                {
                    selectedIndex--;
                }
            }
            
        }

        EditorGUILayout.EndHorizontal();
    }
}
