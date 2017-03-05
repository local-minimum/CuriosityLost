using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class StorifyImporter : MonoBehaviour {

    [SerializeField]
    string importPath;

    private void Start()
    {
        if (!string.IsNullOrEmpty(importPath)) {
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
        foreach(string line in ta.text.Split('\n'))
        {
            ParseStoryBit(line.Trim());
        }
    }

    string optionsPattern = "\\[([^\\]]+)\\]";

    void ParseStoryBit(string line)
    {
        string processed = "";
        int from = 0;
        var matches = Regex.Matches(line, optionsPattern);
        for (int i=0; i<matches.Count; i++)
        {
            var match = matches[i];
            processed += line.Substring(from, match.Index - from);
            processed += string.Format("[{0}]", i);
            from = match.Index + match.Length;

            ParseOptions(matches[i].Groups[0].Value.Substring(1, match.Groups[0].Length - 2));
        }
        processed += line.Substring(from);
        Debug.Log(processed);
        
    }

    string optionPattern = "([^<]+)(<[^>]>)?";

    List<KeyValuePair<string, KeyValuePair<string, int>>> ParseOptions(string options)
    {
        var parsedOptions = new List<KeyValuePair<string, KeyValuePair<string, int>>>();

        foreach(string option in options.Split('|'))
        {
            var match = Regex.Match(option, optionPattern);
            if (match.Groups.Count == 1)
            {
                parsedOptions.Add(new KeyValuePair<string, KeyValuePair<string, int>>(match.Groups[0].Value, new KeyValuePair<string, int>()));
            } else if (match.Groups.Count == 2)
            {
                parsedOptions.Add(new KeyValuePair<string, KeyValuePair<string, int>>(
                    match.Groups[0].Value,
                    ParseEffect(match.Groups[1].Value.Substring(1, match.Groups[1].Value.Length - 2))));
            }
            Debug.Log(match.Groups[0].Value);
        }
        return parsedOptions;
    }

    KeyValuePair<string, int> ParseEffect(string optionEffect)
    {
        return new KeyValuePair<string, int>();
    }
}

