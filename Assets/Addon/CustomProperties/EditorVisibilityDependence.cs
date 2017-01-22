using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorVisibilityDependence : PropertyAttribute {


    public string propertyName;
    public int value;

    public EditorVisibilityDependence(string propertyName, int value) {
        this.propertyName = propertyName;
        this.value = value;
    }
}
