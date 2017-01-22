using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VigorBar : MonoBehaviour {

    [SerializeField, Range(0, 1)]
    float vigorEffect = 1;

    float fullSize = 0;

    [SerializeField]
    float minSize = 0;

    Vector2 sizeDelta;

	void Start () {
        sizeDelta = (transform as RectTransform).sizeDelta;
        fullSize = sizeDelta.x;

	}
	
	
	void Update () {

        sizeDelta.x = Mathf.Round(minSize + vigorEffect * (fullSize - minSize));
        (transform as RectTransform).sizeDelta = sizeDelta;
	}
}
