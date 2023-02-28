using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class SlowTime : MonoBehaviour
{
    
    [Header("TimeControllerSettings")]
    [SerializeField] private float slowModeTimeScale = 0.4f;
    private float startFixedDeltaTime;
    [OnValueChanged("SetDefaultTimeScale")] [SerializeField] 
    private float defaultTimeScale = 1f;
    [SerializeField] private bool isInput;
    // Start is called before the first frame update
    void Start()
    {
        SetDefaultTimeScale();
        //startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
        if (isInput)
        {
            InputManager.LeftDragEvent += StartStop;
            InputManager.RightDragEvent += StartStop;

        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetDefaultTimeScale()
    {
        Time.timeScale = defaultTimeScale;
    }

    public void StartStop(bool start)
    {
        if (start)
        {
            startSlowMotion();
        }
        else
        {
            StopSlowMotion();
        }
    
    }
    public void startSlowMotion()
    {
        Time.timeScale = slowModeTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowModeTimeScale;

    }
    public void StopSlowMotion()
    {
        Time.timeScale = defaultTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime;

    }

    public void SlowDownSubscriber(bool shouldSlow)
    {
        if (shouldSlow)
        {
            startSlowMotion();
        }
        else StopSlowMotion();

    }

}

