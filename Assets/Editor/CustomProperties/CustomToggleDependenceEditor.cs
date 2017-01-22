using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(EditorVisibilityDependence))]
public class CustomToggleDependenceEditor : PropertyDrawer {

    bool showing = true;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorVisibilityDependence myAtt = attribute as EditorVisibilityDependence;
        if (property.serializedObject.FindProperty(myAtt.propertyName).intValue == myAtt.value)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(position, property);
            EditorGUI.EndProperty();
            showing = true;
        }
        else
        {
            showing = false;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return showing ? base.GetPropertyHeight(property, label) : -2f;
    }
}
