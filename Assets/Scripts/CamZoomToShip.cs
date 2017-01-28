using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamZoomToShip : MonoBehaviour {

    [SerializeField]
    Transform spaceShip;

    [SerializeField]
    AnimationCurve zoomCurve;

    [SerializeField, Range(0, 2)]
    float duration = 1f;

    [SerializeField]
    Vector3 targetOffset = Vector3.up * 7f + Vector3.left * 4f;

    [SerializeField]
    float preZoomPause = 0.25f;

    static CamZoomToShip _instance;

    public static void Zoom()
    {
        _instance.ZoomIn();
    }

	void Awake () {
        _instance = this;
	}

    void OnDestroy()
    {
        _instance = null;
    }

    public void ZoomIn()
    {
        StartCoroutine(zoom());
    }	

    IEnumerator<WaitForSeconds> zoom()
    {
        yield return new WaitForSeconds(preZoomPause);

        float start = Time.timeSinceLevelLoad;
        float delta = 0;

        Vector3 origin = transform.position;

        while (delta < 1)
        {
            transform.position = Vector3.LerpUnclamped(origin, spaceShip.position + targetOffset, zoomCurve.Evaluate(delta));
            yield return new WaitForSeconds(0.016f);
            delta = Mathf.Min(1, (Time.timeSinceLevelLoad - start) / duration);
        }

        transform.position = spaceShip.position + targetOffset;
    }
}
