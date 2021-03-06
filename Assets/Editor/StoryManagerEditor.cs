﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(StoryManager))]
public class StoryManagerEditor : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.Space();
        DrawStory(serializedObject.FindProperty("storyKeys"), serializedObject.FindProperty("storyItems"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }

    void DrawStory(SerializedProperty keys, SerializedProperty values)
    {

        keys.isExpanded = EditorGUILayout.Foldout(keys.isExpanded, keys.isExpanded ? "THE STORY" : string.Format("THE STORY ({0} items)", keys.arraySize));

        if (keys.isExpanded)
        {
            EditorGUI.indentLevel += 1;
            for (int i = 0; i < keys.arraySize; i++)
            {
                while (values.arraySize < i + 1)
                {
                    int l = values.arraySize;
                    values.InsertArrayElementAtIndex(l);                    
                    
                }

                DrawStoryItem(i, keys.GetArrayElementAtIndex(i), values.GetArrayElementAtIndex(i));
            }
            EditorGUI.indentLevel -= 1;
            NewKey(keys);    
        }

    }

    int editKeyIndex = -1;
    string editKeyText;

    void DrawStoryItem(int index, SerializedProperty key, SerializedProperty item)
    {
        
        
        SerializedProperty pieces = item.FindPropertyRelative("pieces");

        EditorGUILayout.BeginHorizontal();
        if (editKeyIndex != index)
        {
            item.isExpanded = EditorGUILayout.Foldout(item.isExpanded, item.isExpanded ? key.stringValue : string.Format("{0} ({1} pieces)", key.stringValue, pieces.arraySize));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Edit"))
            {
                editKeyIndex = index;
                editKeyText = key.stringValue;
            }
        } else
        {
            editKeyText = EditorGUILayout.TextField(editKeyText);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Update"))
            {
                key.stringValue = editKeyText;
                editKeyIndex = -1;
            }
            if (GUILayout.Button("Cancel"))
            {
                editKeyIndex = -1;
            }
        }
        EditorGUILayout.EndHorizontal();
        if (item.isExpanded)
        {
            pieces.arraySize = EditorGUILayout.IntField("Pieces:", pieces.arraySize);
            
            EditorGUI.indentLevel += 1;
            for (int i = 0, l = pieces.arraySize; i < l; i++)
            {
                EditorGUILayout.PropertyField(pieces.GetArrayElementAtIndex(i));
            }
            EditorGUI.indentLevel -= 1;
        }
        
    }

    bool editingNewKey = false;
    string editKey = "";

    void NewKey(SerializedProperty keys)
    {
        EditorGUILayout.BeginHorizontal();
        if (editingNewKey)
        {
            editKey = EditorGUILayout.TextField(new GUIContent("New item:"), editKey);
            if (GUILayout.Button("Add"))
            {
                int l = keys.arraySize;
                keys.InsertArrayElementAtIndex(l);
                keys.GetArrayElementAtIndex(l).stringValue = editKey;
                editKey = "";
                editingNewKey = false;
            }
        }
        else
        {
            editingNewKey = GUILayout.Button("+ Add Story   Item");
            if (editingNewKey)
            {
                editKey = "";
            }
            GUILayout.FlexibleSpace();            
        }
        EditorGUILayout.EndHorizontal();
    }


}
