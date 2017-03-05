using System.Collections.Generic;
using UnityEngine;

namespace LM_Storify
{

    public struct StorifyOption
    {
        public string text;
        public StorifyLogic logic;

        public override string ToString()
        {
            return text + "{" + logic + "}";
        }

        public StorifyOption(string text, StorifyLogic logic)
        {
            this.text = text;
            this.logic = logic;
        }

        public StorifyOption(string text)
        {
            this.text = text;
            logic = null;
        }
    }

    public class StorifyImporter : MonoBehaviour
    {

        static Dictionary<string, StorifyImporter> stories = new Dictionary<string, StorifyImporter>();

        [SerializeField]
        string importPath;

        [SerializeField]
        string importAs;

        [SerializeField]
        StorifyStory story = new StorifyStory();

        public static StorifyStory GetStory(string key)
        {
            return stories[key].story;
        }

        private void Start()
        {

            DontDestroyOnLoad(gameObject);
            string key = string.IsNullOrEmpty(importAs) ? importPath : importAs;
            stories[key] = this;

            if (!string.IsNullOrEmpty(importPath))
            {
                LoadFromPath(importPath);
            }
        }

        public void LoadFromPath(string path)
        {
            foreach (TextAsset ta in Resources.LoadAll<TextAsset>(path))
            {
                var classifier = ParseTextAssetName(ta);

                switch (classifier.Value)
                {
                    case "story":
                        var chapter = new StorifyChapter(classifier.Key, ta.text);
                        story.Extend(chapter);
                        break;
                    case "data":
                        Debug.LogWarning("Not implemented to take care of data");
                        break;
                    default:
                        Debug.LogError(string.Format("Resource '{0}' in '{1}' not understood type '{2}'.",
                            ta.name, path, classifier.Value));
                        break;

                }
            }
        }

        KeyValuePair<string, string> ParseTextAssetName(TextAsset ta)
        {

            int divider = ta.name.LastIndexOf('.');
            return new KeyValuePair<string, string>(
                ta.name.Substring(0, divider),
                ta.name.Substring(divider + 1));
        }

    }

}