using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        [SerializeField]
        string importPath;

        private void Start()
        {
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
                        ParseStory(ta);
                        break;
                    case "data":
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

        void ParseStory(TextAsset ta)
        {
            string[] lines = ta.text.Split(new char[] { '\n', '\r' });
            for (int i=0,l=lines.Length; i< l; i++)
            {
                string line = lines[i].Trim();

                if (!string.IsNullOrEmpty(line))
                {
                    ParseStoryBit(line);
                }
            }
        }

        Regex sentenceRegEx = new Regex("\\[([^\\]]+)]");

        void ParseStoryBit(string line)
        {
            
            string processed = "";
            int from = 0;
            int index = 0;
            var match = sentenceRegEx.Match(line);
            var optionsList = new List<List<StorifyOption>>();
            while (match.Success)
            {                
                processed += line.Substring(from, match.Index - from);
                processed += string.Format("[{0}]", index);
                from = match.Index + match.Length;

                optionsList.Add(ParseOptions(match.Groups[1].Value));

                match = match.NextMatch();
                index++;
            }
            processed += line.Substring(from);
            Debug.Log("Proecessed line: " + processed);

        }

        Regex optionRegEx = new Regex("^([^\\{]+)(\\{[^\\}]*\\})?$");

        List<StorifyOption> ParseOptions(string options)
        {
            var parsedOptions = new List<StorifyOption>();

            foreach (string option in options.Split('|'))
            {

                if (string.IsNullOrEmpty(option.Trim()))
                {
                    continue;
                }

                var match = optionRegEx.Match(option);

                //Debug.Log(string.Format("Story option groups ({0})", string.Join(" :: ", match.Groups.Cast<Group>().Select(e => e.Value).ToArray())));

                string optionText = match.Groups[1].Value;
                string optionLogic = match.Groups[2].Value.Trim();

                if (string.IsNullOrEmpty(optionText))
                {
                    Debug.LogError(string.Format("Malformed option '{0}'", option));
                }
                if (string.IsNullOrEmpty(optionLogic))
                {
                    parsedOptions.Add(new StorifyOption(optionText));
                }
                else
                {                    
                    parsedOptions.Add(new StorifyOption(
                        optionText,
                        StorifyLogic.CreateFromString(optionLogic.Substring(1, optionLogic.Length - 2))));
                }

                //Debug.Log(parsedOptions[parsedOptions.Count - 1]);

            }
            return parsedOptions;
        }

    }

}