using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerSmoothSlowDown : MonoBehaviour
{
    public float TotalTime = 1.0f;
    public float CurrentTime = 0f;
    public float startTime = 0.3f;
    public AnimationCurve timeSpeedUpCurve;
    private float initialTimeScale;

    void Start()
    {
        initialTimeScale = Time.timeScale;
    }
    void Update()
    {
        if (CurrentTime < TotalTime)
        {
            CurrentTime += Time.deltaTime;
            Time.timeScale = Mathf.Lerp(startTime, 0.8f, timeSpeedUpCurve.Evaluate( CurrentTime / TotalTime));
        }
        else
        { this.enabled = false; }
    }
    private void OnEnable()
    {
        CurrentTime = 0f;
    }
}
