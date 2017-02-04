using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void MoodChange(float mood);

public class MoodyMan : MonoBehaviour {

    public event MoodChange OnMoodChange;

    int index = -1;

    [SerializeField]
    string[] messages;

    [SerializeField, Range(0, 1)]
    float messageThreshold = 0.2f;

    [SerializeField]
    float p = 0.01f;

    [SerializeField, Range(0, 1)]
    float mood = 1;

    [SerializeField]
    AnimationCurve decayCurve;

    [SerializeField]
    float investigationBonus = 0.4f;

    [SerializeField]
    float decaySpeedMoving = -0.01f;

    [SerializeField]
    float decaySpeedStaning = -0.001f;

    [SerializeField]
    VigorEffects vigorEffects;

    bool isMoving = false;

    WalkController walker;

    void Awake()
    {
        walker = GetComponent<WalkController>();

    }

    void OnEnable()
    {
        walker.OnModeChange += Walker_OnModeChange;
    }

    void OnDisable()
    {
        walker.OnModeChange -= Walker_OnModeChange;
    }

    private void Walker_OnModeChange(SpacerMode mode)
    {
        if (mode == SpacerMode.Investigating)
        {
            StartCoroutine(Rejuvinate(investigationBonus * decayCurve.Evaluate(mood)));
        } else if (mode == SpacerMode.Jumping || mode == SpacerMode.Walking)
        {
            isMoving = true;
        } else
        {
            isMoving = false;
        }
    }

    [SerializeField]
    float rejuvinationSpeed = 0.1f;

    [SerializeField]
    float rejuvinationSteps = 40;

    IEnumerator<WaitForSeconds> Rejuvinate(float amount)
    {
        float stepRejuve = amount / rejuvinationSteps;
        for (int i=0; i<rejuvinationSteps; i++)
        {
            UpdateMood(stepRejuve);
            yield return new WaitForSeconds(rejuvinationSpeed);
        }
    }

    void UpdateMood(float delta)
    {
        mood = Mathf.Clamp01(mood + delta);
        vigorEffects.vigor = mood;
        if (OnMoodChange != null)
        {
            OnMoodChange(mood);
        }
    }

    void Update () {

        UpdateMood((isMoving ? decaySpeedMoving : decaySpeedStaning) * decayCurve.Evaluate(mood) * Time.deltaTime);

		if (mood < messageThreshold && !MessageBar.instance.showing && Random.value < p * Time.deltaTime)
        {
            index++;
            index %= messages.Length;
            MessageBar.instance.Prompt(messages[index]);
        }
	}
}
