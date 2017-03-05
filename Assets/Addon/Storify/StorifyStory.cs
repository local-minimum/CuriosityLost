using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LM_Storify
{
    public class StorifyStory
    {
        [SerializeField, HideInInspector]
        Dictionary<string, StorifyChapter> chapters = new Dictionary<string, StorifyChapter>();

        public StorifyStory() {}

        public StorifyStory(List<StorifyChapter> chapters)
        {
            for (int i = 0, l = chapters.Count; i < l; i++)
            {
                if (this.chapters.ContainsKey(chapters[i].Name))
                {
                    Debug.LogError(string.Format("Duplicated chapter '{0}' omitted.", chapters[i].Name));
                }
                else {
                    this.chapters[chapters[i].Name] = chapters[i];
                }
            }
        }

        public void Extend(StorifyChapter chapter)
        {
            if (chapters.ContainsKey(chapter.Name))
            {
                Debug.LogWarning("Replacing chapter: " + chapter.Name);
            }
            chapters[chapter.Name] = chapter;
        }
    
        public StorifyChapter this[string key]
        {
            get
            {
                return chapters[key];
            }
        }        
    }
}
