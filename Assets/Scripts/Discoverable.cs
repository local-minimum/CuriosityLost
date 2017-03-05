using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Discoverable : MonoBehaviour {

    public static Dictionary<string, List<Discoverable>> _entities = new Dictionary<string, List<Discoverable>>();

    static void RegisterMe(Discoverable d)
    {
        if (!_entities.ContainsKey(d.typeName))
        {
            _entities[d.typeName] = new List<Discoverable>();
        }
        _entities[d.typeName].Add(d);
    }

    public static void SetAllDiscovered(string typeName)
    {
        if (_entities.ContainsKey(typeName))
        {
            for (int i=0,l=_entities[typeName].Count; i< l; i++)
            {
                _entities[typeName][i].discovered = true;
                //Debug.Log("Discovered " + _entities[typeName][i].name);
            }
        }
    }

    public string typeName;

    public bool discovered;
    
    public WorldEntity worldEntity;

    [Range(0, 1)]
    public float desaturationMax = 0.9f;


	void Start () {
        rend = GetComponent<Renderer>();  
        worldEntity = GetComponent<WorldEntity>();
        RegisterMe(this);
	}

    Renderer rend;

    void Update()
    {
        if (rend.isVisible)
        {
            rend.material.SetFloat("_Desaturation", discovered ? desaturationMax :
                Mathf.Lerp(desaturationMax, 0, CamSpacer.MoodyMan.EvaluateInterestRange(Vector3.Distance(CamSpacer.TrackingPoint, transform.position))));
        }
    }

    public void Investigate() {
        discovered = true;        
    }

    void OnDestroy()
    {
        _entities[typeName].Remove(this);
    }
}
