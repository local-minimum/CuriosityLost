using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LM_Storify
{
    [System.Serializable]
    public class StorifyChapter
    {

        [SerializeField, HideInInspector]
        string name;

        public string Name
        {
            get
            {
                return name;
            }
        }

        [SerializeField, HideInInspector]
        List<StorifyPiece> pieces = new List<StorifyPiece>();

        [SerializeField]
        int index = -1;
        
        public StorifyChapter(string name, List<StorifyPiece> pieces)
        {
            this.name = name;
            this.pieces = pieces;
        }

        public StorifyChapter(string name, string rawText)
        {
            this.name = name;
            this.pieces = ParseStory(rawText);
        }

        List<StorifyPiece> ParseStory(string text)
        {
            var pieces = new List<StorifyPiece>();

            string[] lines = text.Split(new char[] { '\n', '\r' });
            for (int i = 0, l = lines.Length; i < l; i++)
            {
                string line = lines[i].Trim();

                if (!string.IsNullOrEmpty(line))
                {
                    pieces.Add(StorifyPiece.CreateFromText(line));
                }
            }
            return pieces;
        }

        public StorifyPiece Current
        {
            get
            {
                if (index < 0)
                {
                    index++;
                }
                if (index < pieces.Count)
                {
                    return pieces[index];
                } else
                {
                    return null;
                }
            }
        }

        public StorifyPiece NextPiece()
        {
            index++;
            if (index < pieces.Count)
            {
                return pieces[index];
            }
            else
            {
                return null;
            }
        }

        public void Reset()
        {
            index = -1;
        }
    }

}
