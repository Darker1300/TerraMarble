using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UnityEngine.Events;

public class ScaleRadius : MonoBehaviour
{
    public RegularPolygon rad;
    //public SlowDownZone slowDownZone;
    public Color PulseColor;
    public float returnToNormalSpeed = 0.5f;
    public float outRadSpeed = 0.5f;
    public float inSpeed;
   // public float increaseRadAmount = 1;
    public float outTargetSize;
    public float NormalSize;
    private bool pushOut;
    public bool HasDelay;
    public bool delayState;
    public float currentDelayDuration;
    public float indelayDuration;
    public float outdelayDuration;
    //public int TapsNeededForIncrease = 3;
    //private int CurrentTapAmount = 0;
    private float maxPushOutSize;
    private float currentTime;
    public bool test;
    public AnimationCurve InCurve;
    public AnimationCurve returnCorouteCurve;
    private float StartIncreaseSize;
    private bool controlRadius = true;
    private bool isDelayIn = false;
    public UnityEvent IsIn;
    public UnityEvent IsOut;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    public void ScaleToggle(bool on)
    {
        if (on)
        {
            controlRadius = true;
        }
        else
            controlRadius = false;


    }
    public void PushOut()
    {
        pushOut = true;


    }
    public void ShrinkBackToNormal()
    {
        StopAllCoroutines();
        StartCoroutine(ShrinkBackToNormalSize());
    }
    public void DelayIn()
    {
        if (currentTime < indelayDuration)
        {
            currentTime += Time.deltaTime;

        }
        else
        {
            currentTime = 0;
            delayState = false;
            pushOut = true;
            IsIn?.Invoke();
        }
    }

    public void DelayOut()
    {
        if (currentTime < outdelayDuration)
        {
            currentTime += Time.deltaTime;

        }
        else
        {
            currentTime = 0;
            delayState = false;
            IsOut?.Invoke();
        }
    }
    //Update is called once per frame
    void Update()
    {

        if (test)
        {
            ScaleToggle(true);
            test = false;
        }
        if (controlRadius)
        {


            if (pushOut)
            {//if has not reached target
                if (rad.Radius < outTargetSize)
                {
                    currentTime += outRadSpeed * Time.deltaTime;
                    rad.Radius = Mathf.Lerp(NormalSize, outTargetSize, InCurve.Evaluate(currentTime));
                    //Aura.Radius += IncreasAuraSpeed * Time.deltaTime;
                }
                else
                {
                    pushOut = false;
                    currentTime = 0f;
                    delayState = true;
                    isDelayIn = false;
                }

            }//drain until zero
            else if (delayState)
            {

                if (isDelayIn)
                {
                    DelayIn();
                }
                else
                    DelayOut();

            }
            else
                if (rad.Radius > NormalSize)
            {
                currentTime += inSpeed * Time.deltaTime;
                rad.Radius = Mathf.Lerp(outTargetSize, NormalSize, InCurve.Evaluate(currentTime));


            }
            else
            {
               
                currentTime = 0;
                delayState = true;
                isDelayIn = true;
            }
        }

    }


    //private IEnumerator DelayTimerIn()
    //{ 
    
    //}
    //private IEnumerator ShrinkBackToNormalSize()
    //{

    //}


        private IEnumerator ShrinkBackToNormalSize()
    {
        ScaleToggle(false);
        currentTime = 0;
        outTargetSize = NormalSize;
        float StartSize = rad.Radius;

        if (rad.Radius > NormalSize)
        {

            while (rad.Radius > NormalSize)
            {

                currentTime += returnToNormalSpeed * Time.deltaTime;
                rad.Radius = Mathf.Lerp(StartSize, outTargetSize, returnCorouteCurve.Evaluate(currentTime));
                yield return null;
            }


        }
        else
        {
            while (rad.Radius < NormalSize)
            {
                currentTime += returnToNormalSpeed * Time.deltaTime;
                rad.Radius = Mathf.Lerp(StartSize, outTargetSize, returnCorouteCurve.Evaluate(currentTime));
                yield return null;

            }
        }
        rad.Radius = NormalSize;

    }


}
