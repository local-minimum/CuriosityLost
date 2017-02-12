using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryManager : MonoBehaviour {

    int index = 0;

    [SerializeField]
    bool wipeProgressOnLoad = false;

    [SerializeField]
    string[] rejectionOptions;

    [SerializeField]
    WalkController walker;

    [SerializeField]
    Story_UI storyUI;

    [SerializeField, HideInInspector]
    List<string> storyKeys = new List<string>();

    [SerializeField, HideInInspector]
    List<Story_Item> storyItems = new List<Story_Item>();

    void Start()
    {
        if (wipeProgressOnLoad)
        {
            WipeProgress();
        }
    }

    public void WipeProgress()
    {
        for (int i=0, l=storyItems.Count; i< l; i++)
        {
            storyItems[i].WipeProgress();
        }
    }

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

    Story_Piece GetPiece(string discoverableType) {
        for (int keyI = 0, keyL = storyKeys.Count; keyI < keyL; keyI++)
        {
            if (storyKeys[keyI] != discoverableType)
            {
                continue;
            }

            Story_Piece piece = storyItems[keyI].NextPiece();
            if (piece)
            {
                piece.hasBeenShown = true;
                return piece;
            }
            
        }
        return null;
    }

    IEnumerator<WaitForSeconds> Investigate()
    {
        string discoverableType = walker.DiscoverableKey;
        Story_Piece activePiece = GetPiece(discoverableType);        

        yield return new WaitForSeconds(delayShowStory);

        if (activePiece == null)
        {
            MessageBar.instance.Prompt(RejectMsg.ToUpper());
            Discoverable.SetAllDiscovered(discoverableType);
            walker.SetSpacerMode(SpacerMode.Standing);
        }
        else {            
            storyUI.ShowStory(activePiece);
        }
    }
}
