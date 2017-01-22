using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorColorLabel : PropertyAttribute {

    public Color color = new Color(0.5f, 0.5f, 1.0f);
    public string text;

    public EditorColorLabel(string text)
    {
        this.text = text;        
    }
}
