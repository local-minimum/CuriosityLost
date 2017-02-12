using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Story_Option", menuName = "StoryOption", order = 0)]
public class Story_Option : ScriptableObject
{
    public string text;
    public Story_OptionCondition condition;

    public bool CanBeUsed
    {
        get
        {
            return true;
        }
    }
}
