using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VigorEffects : MonoBehaviour {

    VigorFieldOfView vigorFieldOfView;

    VigorBar[] vigorBars;

	void Start () {
        vigorFieldOfView = GetComponentInChildren<VigorFieldOfView>();
        vigorBars = GetComponentsInChildren<VigorBar>();
	}

    public float vigor {

        set
        {
            vigorFieldOfView.vigor = value;
            for (int i=0; i<vigorBars.Length; i++)
            {
                vigorBars[i].vigor = value;
            }
        }
    }

}
