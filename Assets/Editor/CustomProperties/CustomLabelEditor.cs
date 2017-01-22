using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EditorColorLabel))]
public class CustomLabelEditor : PropertyDrawer {

    float height;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorColorLabel colorLabel = attribute as EditorColorLabel;

        Rect labelRect = new Rect(position);
        Rect propRect = new Rect(position);

        labelRect.x += 10;
        labelRect.width -= 20;
        labelRect.height = 24;        
        
        EditorGUI.DrawRect(labelRect, colorLabel.color);

        labelRect.y += 4;
        labelRect.height -= 4;

        EditorGUI.LabelField(labelRect, new GUIContent(colorLabel.text));

        propRect.y = labelRect.yMax + 2f;
        propRect.height -= 26f;
        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.PropertyField(propRect, property);
        EditorGUI.EndProperty();

        height = propRect.yMax - position.yMin;
        //position.height = height;
        position.height = height;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return base.GetPropertyHeight(property, label) + 26f;
    }
}
