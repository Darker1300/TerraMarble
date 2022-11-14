using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TImeSlowDown : MonoBehaviour
{
    [Header("TimeControllerSettings")]
    public float TimeScale;
    private float startTimeScale;
    private float startFixedDeltaTime;
    // Start is called before the first frame update
    void Start()
    {
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startSlowMotion()
    {
        Time.timeScale = TimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * TimeScale;
    
    }
    public void StopSlowMotion()
    {
        Time.timeScale = startTimeScale;
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
