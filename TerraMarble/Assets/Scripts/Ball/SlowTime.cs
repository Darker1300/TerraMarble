using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowTime : MonoBehaviour
{
    
    [Header("TimeControllerSettings")]
    public float TimeScale;
    private float startTimeScale;
    private float startFixedDeltaTime;
    [SerializeField]
    private float defaultTimeScale;
    [SerializeField]
    private bool isInput;
    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = defaultTimeScale;
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
        if (isInput)
        {
            InputManager.LeftDragEvent += StartStop;

        }
    }

    // Update is called once per frame
    void Update()
    {

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
        Time.timeScale = TimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * TimeScale;

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

