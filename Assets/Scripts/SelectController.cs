using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectController : MonoBehaviour {

    Animator anim;

    [SerializeField]
    string walkTargetTrigger;

    [SerializeField]
    Image selectImage;

    Canvas worldCanvas;

    [SerializeField]
    float Yoffset = 0.01f;

    [SerializeField]
    string itemTrigger;

	void Start () {
        worldCanvas = selectImage.GetComponentInParent<Canvas>();
        anim = worldCanvas.GetComponent<Animator>();        
    }

    public void ClickLocation(Vector3 pos)
    {
        worldCanvas.transform.position = pos + Vector3.up * Yoffset;
        anim.SetTrigger(walkTargetTrigger);
    }

    public void ClickItem(GameObject item)
    {
        //TODO: Bug not grabbing item sprite

        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        if (rend)
        {
            selectImage.sprite = rend.sprite;
        } else
        {
            selectImage.sprite = null;
        }
        anim.SetTrigger(itemTrigger);
    }
    
}
