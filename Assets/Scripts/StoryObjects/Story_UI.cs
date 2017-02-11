using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public enum WordDecoration { None, Options, All};

public class Story_UI : MonoBehaviour {


    struct MessagePart {
        public string msg;
        public bool spaceAfter;

        public MessagePart(string msg, bool spaceAfter)
        {
            this.msg = msg;
            this.spaceAfter = spaceAfter;
        }
    }

    [SerializeField]
    RectTransform fillSpace;

    List<Story_UI_TextElement> textFields = new List<Story_UI_TextElement>();

    [SerializeField]
    FontData fontData;

    [SerializeField]
    TextAnchor textAnchor;

    [SerializeField]
    string message = "This message, is a test!";

    [SerializeField]
    Color textColor;

    [SerializeField]
    WordDecoration addBgImage = WordDecoration.Options;

    [SerializeField]
    Sprite bgImage;

    [SerializeField]
    Color bgImageColor;

    [SerializeField]
    Vector2 paddingRightTop;

    [SerializeField]
    Vector2 paddingLeftBottom;

    string delimiters = " .,!:;?/";

    string optionPattern = @"^\{\{(.+)\}\}$";

    int cachedTextsIndex = 0;

    [SerializeField]
    float wordSpacing = 10;

    Vector2 screenSize;

    void Start()
    {
        DisplayMessage();
    }

    string _displayedMsg;

    public void DisplayMessage()
    {
     
        cachedTextsIndex = 0;
        bool spaceBefore = true;

        foreach (MessagePart mPart in GetMessageInParts())
        {
            Story_UI_TextElement t = Get_StoryTextElem(mPart.msg);
            Story_UI_TextElement tElem = t.GetComponent<Story_UI_TextElement>();
            tElem.spaceAfter = mPart.spaceAfter;
            tElem.spaceBefore = spaceBefore;
            string modString;
            bool modified = IsOption(mPart.msg, out modString);
            tElem.buttonize = modified;
            if (modified)
            {
                if (tElem.originalMessage != mPart.msg)
                {                    
                    tElem.originalMessage = mPart.msg;
                    tElem.Message = modString;
                }          
            } else
            {
                tElem.originalMessage = mPart.msg;
                tElem.Message = mPart.msg;
            }

            spaceBefore = tElem.spaceAfter;

        }

        while (cachedTextsIndex < textFields.Count)
        {
            textFields[cachedTextsIndex].gameObject.SetActive(false);
            cachedTextsIndex++;
        }

    }

    Vector2 wordSpacingV2;
    Vector2 wordOff;
    float wordHeight;
    float lineWidth = 40f;
    float lineHeightFactor = 1.5f;

    void Realign()
    {
        wordSpacingV2 = Vector2.right * this.wordSpacing;
        bool inheritPrevActiveBgSetting = false;
        screenSize = new Vector2(Screen.width, Screen.height);
        wordOff = new Vector2(-Screen.width, Screen.height) / 2f + new Vector2(30, -100);
        lineWidth = Screen.width * 0.9f;
        wordHeight = 0;
        float xOffSource = wordOff.x;

        foreach (Story_UI_TextElement tElem in textFields)
        {
            if (!tElem.gameObject.activeSelf)
            {
                break;
            }

            tElem.showBG = addBgImage == WordDecoration.All || addBgImage == WordDecoration.Options && tElem.buttonize || inheritPrevActiveBgSetting;

            inheritPrevActiveBgSetting = tElem.buttonize && tElem.spaceAfter == false;

            AlignElement(tElem);
            if (Mathf.Abs(wordOff.x - xOffSource)  > lineWidth){
                wordOff.x = xOffSource;
                wordOff.y -= lineHeightFactor * wordHeight;
                wordHeight = 0;
                AlignElement(tElem);
            }
            tElem.Aligned();
        }
    }

    void AlignElement(Story_UI_TextElement tElem)
    {
        RectTransform rt = tElem.transform as RectTransform;

        if (tElem.showBG)
        {
            wordOff -= Vector2.right * (tElem.spaceBefore ? paddingLeftBottom.x : 0);
            rt.localPosition = wordOff;
            tElem.bgRectTransf.offsetMin = new Vector2(tElem.spaceBefore ? paddingLeftBottom.x : 0, paddingLeftBottom.y);
            tElem.bgRectTransf.offsetMax = new Vector2(tElem.spaceAfter ? paddingRightTop.x : 0, paddingRightTop.y);
            wordOff += Vector2.right * (rt.sizeDelta.x + (tElem.spaceAfter ? paddingRightTop.x : 0)) + (tElem.spaceAfter ? wordSpacingV2 : Vector2.zero);
            wordHeight = Mathf.Max(wordHeight, Mathf.Abs(tElem.bgRectTransf.sizeDelta.y));
        }
        else
        {
            rt.localPosition = wordOff;
            wordOff += Vector2.right * rt.sizeDelta.x + (tElem.spaceAfter ? wordSpacingV2 : Vector2.zero);
            wordHeight = Mathf.Max(wordHeight, Mathf.Abs(rt.sizeDelta.y));
        }
    }

    public bool IsOption(string word, out string match)
    {
        match = null;
        Match m = Regex.Match(word, optionPattern);
        if (m.Success)
        {
            match = m.Groups[1].Value;          
        }
        return m.Success;
    }

    IEnumerable<MessagePart> GetMessageInParts()
    {
        _displayedMsg = message;

        string[] words = message.Split(delimiters.ToCharArray());
        int wordId = 0;
        bool spaceAfter;
        for (int i=0; i<message.Length; i++)
        {
            if (wordId < words.Length && message.Substring(i, words[wordId].Length) == words[wordId])
            {
                i += words[wordId].Length - 1;
                spaceAfter = i + 1 >= message.Length || message[i + 1] == ' ';
                yield return new MessagePart(words[wordId], spaceAfter);
                wordId++;
                while (wordId < words.Length && words[wordId].Length == 0)
                {
                    wordId++;
                }
            } else if (message[i] != ' ')
            {
                spaceAfter = i + 1 >= message.Length || message[i + 1] == ' ';
                yield return new MessagePart(message[i].ToString(), spaceAfter);
            }
        }

    }

    void AddBackground(RectTransform textTF)
    {

        GameObject go = new GameObject(textTF.name + " Background", typeof(RectTransform), typeof(Image));        
        go.transform.SetParent(textTF);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = paddingLeftBottom;
        rt.offsetMax = paddingRightTop;
        Image i = go.GetComponent<Image>();
        i.sprite = bgImage;
        i.color = bgImageColor;
       
    }
    
    Story_UI_TextElement Get_StoryTextElem(string txt)
    {
        Story_UI_TextElement storyElem;
        if (cachedTextsIndex < textFields.Count)
        {
            storyElem = textFields[cachedTextsIndex];
            cachedTextsIndex++;
            storyElem.gameObject.SetActive(true);

        } else
        {
            storyElem = NewTextElement();
            cachedTextsIndex++;
            textFields.Add(storyElem);

            Text t = storyElem.textArea;
            t.alignment = textAnchor;
            t.font = fontData.font;
            t.fontSize = fontData.fontSize;
            t.fontStyle = fontData.fontStyle;

            ContentSizeFitter cfit = storyElem.GetComponent<ContentSizeFitter>();
            cfit.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            cfit.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            RectTransform rt = storyElem.transform as RectTransform;
            rt.anchorMin = Vector2.up;
            rt.anchorMax = Vector2.up;
            rt.pivot = Vector2.zero;

            AddBackground(rt);
        }
        
        return storyElem;
    }

    Story_UI_TextElement NewTextElement()
    {
        GameObject go = new GameObject("Text Bit " + cachedTextsIndex, typeof(Text), typeof(ContentSizeFitter), typeof(Story_UI_TextElement));        
        go.transform.SetParent(fillSpace);
        
        return go.GetComponent<Story_UI_TextElement>();
    }

    bool NeedToRepartition
    {
        get
        {
            return _displayedMsg != message;
        }
    }

    bool _delayUpdate = false;

    bool NeedRealignment
    {
        get
        {
            if (_delayUpdate)
            {
                _delayUpdate = false;
                return true;
            }

            if (screenSize.x != Screen.width || screenSize.y != Screen.height)
            {
                _delayUpdate = true;
                return false;
            }

            for (int i = 0, l = textFields.Count; i < l; i++) {
                if (!textFields[i].gameObject.activeSelf)
                {
                    break;
                }
                if (textFields[i].NeedRealignment)
                {
                    _delayUpdate = true;
                    return false;
                }
            }

            _delayUpdate = false;
            return false;
        }
    }

    void Update()
    {
        if (NeedToRepartition)
        {            
            DisplayMessage();
        } else if (NeedRealignment)
        {            
            Realign();
        }
    }
}
