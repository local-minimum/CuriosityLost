using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

namespace LM_Storify {

    public enum EffectLogic {Absolute, Relative}

    public struct StorifyEffect
    {
        public string effectName;
        public int amount;
        public EffectLogic logic;

        public override string ToString()
        {
            return string.Format("{2}{0} {1}", Mathf.Abs(amount), effectName, logic == EffectLogic.Absolute ? "" : (amount < 0 ? "-" : "+"));
        }
    }

    public enum RequirementLogic { LessThan, GreaterThan, EqualTo};

    public struct StorifyRequirement
    {
        public string requirementName;
        public int amount;
        public RequirementLogic logic;

        public override string ToString()
        {
            return string.Format("{0} {2} {1}", requirementName, amount, "<>="[(int)logic]);
        }
    }

    [System.Serializable]
    public class StorifyLogic {

        public List<StorifyEffect> effects = new List<StorifyEffect>();
        public List<StorifyRequirement> requirements = new List<StorifyRequirement>();

        public override string ToString()
        {
            string s1 = string.Join(",", effects.Select(e => e.ToString()).ToArray());
            string s2 = string.Join(",", requirements.Select(e => e.ToString()).ToArray());
            if (string.IsNullOrEmpty(s1))
            {
                return s2;
            } else if (string.IsNullOrEmpty(s2))
            {
                return s1;
            } else
            {
                return s1 + "," + s2;
            }            
        }

        public bool Validate(string[] characterStatNames)
        {
            HashSet<string> invalids = new HashSet<string>();

            for (int i = 0, l = requirements.Count; i < l; i++)
            {
                if (!characterStatNames.Contains(requirements[i].requirementName))
                {
                    invalids.Add(requirements[i].requirementName);
                }
            }

            for (int i = 0, l = effects.Count; i < l; i++)
            {
                if (!characterStatNames.Contains(effects[i].effectName))
                {
                    invalids.Add(effects[i].effectName);
                }
            }

            if (invalids.Count != 0)
            {
                Debug.LogError(string.Format("StorifyLogic '{0}' requires unsuppored character stats {1}", this, invalids.ToArray()));
                return false;
            } else
            {
                return true;
            }
            
        }

        public bool IsAllowed(System.Func<string, int> characterStatsGetter)
        {
            for (int i=0, l=requirements.Count; i< l; i++)
            {
                if (!IsAllowed(characterStatsGetter(requirements[i].requirementName), requirements[i]))
                {
                    return false;
                }
            }
            return true;
        }


        public static bool IsAllowed(int value, StorifyRequirement requirement)
        {
            switch (requirement.logic)
            {
                case RequirementLogic.EqualTo:
                    return value == requirement.amount;
                case RequirementLogic.LessThan:
                    return value < requirement.amount;
                case RequirementLogic.GreaterThan:
                    return value > requirement.amount;
                default:
                    return false;
            }
        }

        public void InvokeEffects(System.Action<string, int> characterStatsSetterRelative, System.Action<string, int> characterStatsSetterAbsolute)
        {
            for (int i = 0, l = effects.Count; i < l; i++)
            {
                switch (effects[i].logic)
                {
                    case EffectLogic.Absolute:
                        characterStatsSetterAbsolute(effects[i].effectName, effects[i].amount);
                        break;
                    case EffectLogic.Relative:
                        characterStatsSetterRelative(effects[i].effectName, effects[i].amount);
                        break;
                    default:
                        Debug.LogWarning(string.Format("Can't invoke effect '{0}' because missing logic", effects[i]));
                        break;
                }
            }
        }

        public static StorifyLogic CreateFromString(string s)
        {
            StorifyLogic item = new StorifyLogic();
            item.parseSet(s);
            return item;
        }

        void parseSet(string text)
        {
            string[] instructions = text.Split(',');

            for (int i = 0, l = instructions.Length; i < l; i++)
            {
                string instruction = instructions[i].Trim();
                StorifyEffect effect;
                StorifyRequirement requirement;

                if (string.IsNullOrEmpty(instruction))
                {
                    continue;
                } else if (IsEffect(instruction, out effect))
                {
                    effects.Add(effect);
                } else if (IsRequirement(instruction, out requirement))
                {
                    requirements.Add(requirement);
                } else
                {
                    Debug.LogError(string.Format("StorifyLogic could not parse '{0}' in '{1}'", instruction, text));
                }  
            }
        }
        
        Regex effectRegEx = new Regex("^([+-]?\\d+) ?([\\w _-]+)$");

        bool IsEffect(string text, out StorifyEffect effect)
        {
            effect = new StorifyEffect();

            var match = effectRegEx.Match(text);
            string amount = match.Groups[1].Value.Trim();
            string characterStat = match.Groups[2].Value.Trim();

            if (string.IsNullOrEmpty(characterStat) || string.IsNullOrEmpty(amount))
            {
                return false;
            } else 
            {

                //Debug.Log(amount + " :: " + characterStat);
                effect.effectName = characterStat;
                effect.logic = amount.IndexOfAny(new char[] { '+', '-' }) == 0 ? EffectLogic.Relative : EffectLogic.Absolute;
                effect.amount = int.Parse(amount.StartsWith("+") ? amount.Substring(1) : amount);

                return true;
            }
        }


        Regex requirementRegEx = new Regex("^(\\w+) ?([><=]) ?([+-]?\\d)$");

        bool IsRequirement(string text, out StorifyRequirement requirement)
        {
            requirement = new StorifyRequirement();

            var match = requirementRegEx.Match(text);
            string characterStat = match.Groups[1].Value.Trim();
            string logic = match.Groups[2].Value.Trim();
            string amount = match.Groups[3].Value.Trim();

            if (string.IsNullOrEmpty(characterStat) || string.IsNullOrEmpty(logic) || string.IsNullOrEmpty(amount))
            {
                return false;
            } else
            {

                requirement.requirementName = characterStat;
                requirement.logic = (RequirementLogic)"<>=".IndexOf(logic);
                requirement.amount = int.Parse(amount.StartsWith("+") ? amount.Substring(1) : amount);
                
                return true;
            }

        }

    }

}