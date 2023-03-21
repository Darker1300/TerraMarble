using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CountDownTimer : MonoBehaviour
{
    public float TotalTime;
    public float CurrentTime;
    public UnityEvent TimerFinished;

    public void SetTimer(float time)
    {
        TotalTime = time;
    }
   
    void Update()
    {
        if (CurrentTime < TotalTime)
        {
            CurrentTime += Time.deltaTime;
        }
        else
        {
            TimerFinished.Invoke();
            this.enabled = false;
        }
    }

    private void OnDisable()
    {
        TotalTime = 0.0f;
        CurrentTime = 0.0f;
    }
}
