﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void StoryOptionChangeRequest(Story_UI_TextElement elem);

public class Story_UI_TextElement : MonoBehaviour {

    public static event StoryOptionChangeRequest OnChangeRequest;

    public bool spaceBefore = false;
    public bool spaceAfter = false;

    public string originalMessage;

    Button bgButton;
    
    public string Message
    {
        set
        {            
            textArea.text = value;                
        }
    }

    public bool buttonize
    {
        get
        {
            return bgButton != null && bgButton.enabled;
        }

        set
        {
            if (bgButton == null && value)
            {
                bgButton = bgRectTransf.gameObject.AddComponent<Button>();
                bgButton.image = bgImage;
                bgButton.onClick = new Button.ButtonClickedEvent();
                bgButton.onClick.AddListener(ClickedButton);

            } else if (bgButton != null)
            {
                bgButton.enabled = value;
            }
        }
    }
    bool _showBg = false;

    Text _textArea;
    public Text textArea
    {
        get
        {
            if (_textArea == null)
            {
                _textArea = GetComponent<Text>();
            }
            return _textArea;
        }
    }

    public bool showBG {
        set
        {
            _showBg = value;
            bgRectTransf.gameObject.SetActive(value);
        }

        get
        {
            return _showBg;
        }
    }

    RectTransform _bgRectTransf;

    public RectTransform bgRectTransf {
        get
        {
            if (_bgRectTransf == null)
            {
                _bgRectTransf = transform.GetChild(0).GetComponent<RectTransform>();
            }
            return _bgRectTransf;
        }
    }

    Image _bgImage;
    public Image bgImage
    {
        get
        {
            if (_bgImage == null && _bgRectTransf != null)
            {
                _bgImage = _bgRectTransf.GetComponent<Image>();
            }
            return _bgImage;
        }
    }

    public void ClickedButton()
    {
        if (OnChangeRequest != null)
        {
            OnChangeRequest(this);
        }
    }
}
