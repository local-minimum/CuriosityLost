using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamSpacer : MonoBehaviour {

    [SerializeField]
    Transform planet;

    [SerializeField]
    WalkController player;

    [SerializeField]
    Rect stationaryRelativeScreenRect;

    [SerializeField]
    AnimationCurve transition;

    [SerializeField]
    bool tracking;

    Camera cam;

    [SerializeField, Range(0, 2)]
    float animDuration = 1f;

    float camElevation;

    bool animating = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        player.OnDisembark += DisembarkHandler;
    }

    private void OnDisable()
    {
        player.OnDisembark -= DisembarkHandler;
    }

    private void DisembarkHandler()
    {
        camElevation = transform.position.y;
        tracking = true;
    }

    void Update () {
		if (tracking)
        {
            Vector2 pos = cam.WorldToViewportPoint(player.transform.position);
            if (!stationaryRelativeScreenRect.Contains(pos))
            {
                StartCoroutine(animateCamTransition());
            }
        }
	}
    
    IEnumerator<WaitForSeconds> animateCamTransition()
    {
        Vector3 entityPos = player.transform.position;
        Vector3 target = new Vector3(entityPos.x, camElevation, entityPos.z);
        Vector3 start = transform.position;
        target = (target - start) * 1.1f + start;
        animating = true;
        float t = Time.timeSinceLevelLoad;
        float progress = 0;
        while (progress < 1f)
        {
            progress = (Time.timeSinceLevelLoad - t) / animDuration;
            transform.position = Vector3.Lerp(start, target, transition.Evaluate(progress));
            yield return new WaitForSeconds(.016f);
        }
        transform.position = target;
        animating = false;
    }
}
