using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Story_Piece", menuName ="Story/Piece", order = 0)]
public class Story_Piece : ScriptableObject {

    public string storyText;
    public string[] optionKeys;
    public Dictionary<string, Story_OptionCondition[]> optionStore;

}
