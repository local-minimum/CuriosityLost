using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoodyMan : MonoBehaviour {

    int index = -1;

    [SerializeField]
    string[] messages;

    [SerializeField]
    float p = 0.01f;
	
	void Update () {
		if (!MessageBar.instance.showing && Random.value < p * Time.deltaTime)
        {
            index++;
            index %= messages.Length;
            MessageBar.instance.Prompt(messages[index]);
        }
	}
}
