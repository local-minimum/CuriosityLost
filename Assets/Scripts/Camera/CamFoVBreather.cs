using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFoVBreather : MonoBehaviour {

    [SerializeField, HideInInspector]
    float _cycleDuration = 1.6f;

    float refTime = 0;

    float refProgression = 0;

    [SerializeField]
    AnimationCurve cycle;

    float baseFoV;

    [SerializeField, Range(0, 20)]
    float effectMagnitude = 1f;

    Camera cam;

    public float cycleDuration
    {
        get
        {
            return _cycleDuration;
        }

        set
        {
            refProgression = cycleProgression;
            refTime = Time.timeSinceLevelLoad;
            _cycleDuration = value == 0 ? 1: value;
            Debug.Log(_cycleDuration);
        }
    }

    float cycleProgression
    {
        get
        {
            return ((Time.timeSinceLevelLoad - refTime) / _cycleDuration + refProgression) % 1.0f;
        }
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        baseFoV = cam.fieldOfView;
    }
	
	void Update () {
        cam.fieldOfView = baseFoV + cycle.Evaluate(cycleProgression) * effectMagnitude;
	}
}
