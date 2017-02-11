using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour {

    int index = 0;

    [SerializeField]
    string[] options;

	void OnEnable()
    {
        Story_UI_TextElement.OnChangeRequest += Story_UI_TextElement_OnChangeRequest;
    }

    void OnDisable()
    {
        Story_UI_TextElement.OnChangeRequest -= Story_UI_TextElement_OnChangeRequest;

    }

    private void Story_UI_TextElement_OnChangeRequest(Story_UI_TextElement elem)
    {        
        elem.Message = options[index];
        index++;
        index %= options.Length;
    }
}
