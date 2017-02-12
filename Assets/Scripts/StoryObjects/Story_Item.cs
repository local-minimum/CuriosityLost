using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Story_Item
{

    public List<Story_Piece> pieces = new List<Story_Piece>();

    public Story_Piece NextPiece()
    {
        for (int i = 0, l = pieces.Count; i < l; i++)
        {
            if (!pieces[i].hasBeenShown)
            {
                return pieces[i];
            }
        }
        return null;
    }

    public void WipeProgress()
    {
        for (int i = 0, l = pieces.Count; i < l; i++)
        {
            pieces[i].hasBeenShown = false;

        }
    }
}