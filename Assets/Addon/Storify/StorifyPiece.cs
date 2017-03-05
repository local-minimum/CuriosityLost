
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;

namespace LM_Storify {

    [System.Serializable]
    public class StorifyPiece {

        [SerializeField, HideInInspector]
        string text;

        [SerializeField, HideInInspector]
        List<List<StorifyOption>> optionsList = new List<List<StorifyOption>>();

        [SerializeField, HideInInspector]
        List<int> currentOption = new List<int>();

        public string Text
        {
            get
            {
                return text;
            }
        }

        public StorifyPiece(string text, List<List<StorifyOption>> optionsList)
        {
            this.text = text;
            this.optionsList = optionsList;
            SetDefaultOptions();
        }

        void SetDefaultOptions()
        {
            currentOption.Clear();
            for (int i=0, l=optionsList.Count; i< l; i++)
            {
                currentOption.Add(-1);
            }
        }

        static Regex optionSlot = new Regex("\\[(\\d+)\\]");

        public override string ToString()
        {

            Match m = optionSlot.Match(text);
            string s = "";
            int pos = 0;

            while (m.Success)
            {
                s += text.Substring(pos, m.Index - pos);
                int index = int.Parse(m.Groups[1].Value);
                s += "[" + string.Join("|", optionsList[index].Select(e => e.ToString()).ToArray()) + "]";
                pos = m.Index + m.Length;
                m = m.NextMatch();
            }

            s += text.Substring(pos);
            return s;
        }

        static Regex sentenceRegEx = new Regex("\\[([^\\]]+)]");

        public static StorifyPiece CreateFromText(string line)
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

            return new StorifyPiece(processed, optionsList);
        }

        static Regex optionRegEx = new Regex("^([^\\{]+)(\\{[^\\}]*\\})?$");

        static List<StorifyOption> ParseOptions(string options)
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

        public StorifyOption GetNextOption(string index)
        {
            int i = int.Parse(index);
            currentOption[i]++;
            currentOption[i] %= optionsList[i].Count;
            return optionsList[i][currentOption[i]];
        }

        public StorifyOption GetNextAllowedOption(string index, System.Func<string, int> characterStatsGetter)
        {
            int i = int.Parse(index);            
            int curOption = currentOption[i] + 1;
            int n = optionsList[i].Count;
            curOption %= n;
            int steps = 0;
            
            while (steps < n)
            {
                StorifyOption option = optionsList[i][curOption];
                if (option.logic.IsAllowed(characterStatsGetter))
                {
                    currentOption[i] = curOption;
                    break;
                }
                curOption++;
                curOption %= n;
                steps++;
            }

            return optionsList[i][currentOption[i]];
        }
    }

}
