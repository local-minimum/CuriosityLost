using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour {

    int index = 0;

    [SerializeField]
    string[] rejectionOptions;

    [SerializeField]
    WalkController walker;

    [SerializeField]
    Story_UI storyUI;

	void OnEnable()
    {
        Story_UI_TextElement.OnChangeRequest += Story_UI_TextElement_OnChangeRequest;
        walker.OnModeChange += Walker_OnModeChange;
    }

    void OnDisable()
    {
        Story_UI_TextElement.OnChangeRequest -= Story_UI_TextElement_OnChangeRequest;
        walker.OnModeChange -= Walker_OnModeChange;

    }

    private void Story_UI_TextElement_OnChangeRequest(Story_UI_TextElement elem)
    {
        //TODO replace with actual options
        elem.Message = RejectMsg;
    }    

    private void Walker_OnModeChange(SpacerMode mode)
    {
        if (mode == SpacerMode.Investigating)
        {
            StartCoroutine(Investigate());
        }
    }

    string RejectMsg
    {
        get
        {
            index++;
            index %= rejectionOptions.Length;
            return rejectionOptions[index];
        }
    }
    [SerializeField]
    float delayShowStory = 1f;

    Story_Piece activePiece;

    IEnumerator<WaitForSeconds> Investigate()
    {
        string discoverableType = walker.DiscoverableKey;

        Debug.Log(discoverableType);

        yield return new WaitForSeconds(delayShowStory);

        if (activePiece == null)
        {
            MessageBar.instance.Prompt(RejectMsg.ToUpper());
            Discoverable.SetAllDiscovered(discoverableType);
            walker.SetSpacerMode(SpacerMode.Standing);
        }
        else {
            storyUI.ShowStory();
        }
    }
}
