using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Story_Piece", menuName ="Story/Piece", order = 0)]
public class Story_Piece : ScriptableObject {

    public string storyText;    
    public bool hasBeenShown = false;
    public List<Story_MultipleChoice> choices = new List<Story_MultipleChoice>();
    
    public Story_Option GetNextOption(string choiceKey)
    {
        for (int i=0, l=choices.Count; i< l; i++)
        {
            if (choices[i].choiceKey == choiceKey)
            {
                return choices[i].NextOption();
            }
        }

        return null;
    }
}
