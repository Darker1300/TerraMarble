using Sirenix.OdinInspector;
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

    public bool test;
    [HideLabel]
    [SerializeField]
    [TextArea(1, 2)]
    private string InfoString = "";
    public UnityEvent TriggerEvent;
    // Start is called before the first frame update
    void Start()
    {

    }



    // Update is called once per frame
    void Update()
    {
        if (test)
        {
            TimerEventStart();
            test = false;
        }
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
    public void EndTimerEvent()
    {
        StopAllCoroutines();
    
    }

    public void setDuration(float duration)
    {
        TriggerDuration = duration;

    }
}
