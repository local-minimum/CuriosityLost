using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Story_MultipleChoice {

    public string choiceKey;
    public int defaultOption = 0;
    int optionIndex = -1;
    public List<Story_Option> options = new List<Story_Option>();

    public Story_Option NextOption()
    {
        if (optionIndex < 0)
        {
            optionIndex = defaultOption - 1;
        }

        int len = options.Count;
        int n = 0;

        do
        {
            optionIndex++;
            optionIndex %= len;
            
            if (options[optionIndex].CanBeUsed)
            {
                return options[optionIndex];
            }

            n++;

        } while (n < len);

        return options[defaultOption];
    }
}
