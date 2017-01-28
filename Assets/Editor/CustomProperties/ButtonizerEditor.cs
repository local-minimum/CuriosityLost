using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Buttonizer))]
public class ButtonizerEditor : PropertyDrawer {

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        string buttonText = (this.fieldInfo.GetCustomAttributes(false)[0] as Buttonizer).name;

        if (GUI.Button(position, string.IsNullOrEmpty(buttonText) ? property.name : buttonText))
        {
            MonoBehaviour script = property.serializedObject.targetObject as MonoBehaviour;
            script.SendMessage(buttonText, SendMessageOptions.RequireReceiver);
        }
        
    }
}
