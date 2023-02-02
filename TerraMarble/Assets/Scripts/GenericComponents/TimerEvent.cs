using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TimerEvent : MonoBehaviour
{
    [SerializeField]
    private float TriggerDuration = 2f;

    [SerializeField]
    private int repeatAmount;

    [SerializeField]
    private bool loop;


    public UnityEvent TriggerEvent;
    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {

    }
    public void TimerEventStart()
    {
        StartCoroutine(trigger());

    }
    IEnumerator trigger()
    {
        float timer = 0;
        while (timer <= TriggerDuration)
        {
            timer = timer + Time.deltaTime;


            yield return null;
        }
        TriggerEvent?.Invoke();
        if (repeatAmount > 0 || loop)
        {
            --repeatAmount;
            TimerEventStart();
        }

    }
    public void setDuration(float duration)
    {
        TriggerDuration = duration;

    }
}
