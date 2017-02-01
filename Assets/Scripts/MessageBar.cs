using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageBar : MonoBehaviour {

    [SerializeField]
    Text textField;

    [SerializeField]
    Animator anim;

    [SerializeField]
    string foldOutTrigger = "FoldOut";

    [SerializeField]
    string foldInTrigger = "FoldIn";

    [SerializeField, Range(0, 5)]
    float defaultDuration = 2.4f;

    static MessageBar _instance;

    bool _showing;

    public bool showing
    {
        get
        {
            return _showing;
        }
    }

    public static MessageBar instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MessageBar>();
            }
            return _instance;
        }
    }

    void Start()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this;
        } else
        {
            Destroy(gameObject);
        }
    }

    public void Prompt(string message)
    {
        StartCoroutine(_Prompt(message, defaultDuration));
    }

    public void Prompt(string message, float duration)
    {
        StartCoroutine(_Prompt(message, duration));
    }

    IEnumerator<WaitForSeconds> _Prompt(string message, float duration)
    {
        _showing = true;
        textField.text = message;
        textField.enabled = false;  
        anim.SetTrigger(foldOutTrigger);
        yield return new WaitForSeconds(duration);
        anim.SetTrigger(foldInTrigger);
        _showing = false;    
    }
}
