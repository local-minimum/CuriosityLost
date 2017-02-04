using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName ="Story_CharEffect", menuName ="Story/CharacterEffect", order = 0)]
public class Story_CharacterEffect : ScriptableObject {

    public string description = "";
    public string internal_description = "";
    public Sprite effectIcon;
    public bool stacks;
    public string effectMethod;
    public float effect;
}
