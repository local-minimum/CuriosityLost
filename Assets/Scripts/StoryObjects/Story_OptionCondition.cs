using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Story_OptionCondition {
    public string text;
    public Story_CharacterEffect effect;

    public string[] requirements;
    public float[] requirementThresholds;
    
    public bool IsAvailable
    {
        get
        {
            return true;
        }
    }

    public void Select()
    {
        //Ask Narrator to use me
    }

}
