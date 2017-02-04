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

    Rect currentStationaryRelativeScreenRect;

    [SerializeField]
    AnimationCurve transition;

    [SerializeField]
    bool tracking;

    Camera cam;

    [SerializeField, Range(0, 2)]
    float animDuration = 1f;

    float camElevation;

    bool animating = false;

    MoodyMan moodyCtrl;

    void Awake()
    {
        moodyCtrl = player.GetComponent<MoodyMan>();
        currentStationaryRelativeScreenRect = stationaryRelativeScreenRect;
    }
    private void Start()
    {
        cam = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        player.OnDisembark += DisembarkHandler;
        moodyCtrl.OnMoodChange += CamSpacer_OnMoodChange;
    }

    private void OnDisable()
    {
        player.OnDisembark -= DisembarkHandler;
        moodyCtrl.OnMoodChange -= CamSpacer_OnMoodChange;
    }

    private void CamSpacer_OnMoodChange(float mood)
    {
        currentStationaryRelativeScreenRect.x = Mathf.Lerp(stationaryRelativeScreenRect.x, 0.49f, 1 - mood);
        currentStationaryRelativeScreenRect.y = Mathf.Lerp(stationaryRelativeScreenRect.y, 0.49f, 1 - mood);
        currentStationaryRelativeScreenRect.width = 1f - currentStationaryRelativeScreenRect.x * 2f;
        currentStationaryRelativeScreenRect.height = 1f - currentStationaryRelativeScreenRect.y * 2f;
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
            if (!currentStationaryRelativeScreenRect.Contains(pos))
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
