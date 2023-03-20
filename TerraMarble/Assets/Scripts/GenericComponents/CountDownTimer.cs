using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CountDownTimer : MonoBehaviour
{
    public float TotalTime;
    public float CurrentTime;
    public UnityEvent TimerFinished;
    private void OnEnable()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void SetTimer(float time)
    {
        TotalTime = time;
    }
    // Update is called once per frame
   
    void Update()
    {
        if (CurrentTime < TotalTime)
        {
            CurrentTime += Time.deltaTime;
            //Time.timeScale = Mathf.Lerp(startTime, 0.8f, CurrentTime / TotalTime);
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
