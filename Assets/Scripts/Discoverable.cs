using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discoverable : MonoBehaviour {

    public string typeName;

    public bool discovered;
    
    public WorldEntity worldEntity;

	void Start () {
        worldEntity = GetComponent<WorldEntity>();
	}
	

}
