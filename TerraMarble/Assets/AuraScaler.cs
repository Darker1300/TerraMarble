using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class AuraScaler : MonoBehaviour
{
    public Disc rad;
    //public SlowDownZone slowDownZone;
    public Color PulseColor;
    public float returnToNormalSpeed = 0.5f;
    public float IncreasAuraSpeed = 0.5f;
    public float increaseAuraAmount = 1;
    private float currentTarget;
    private bool pushOut;
    //public int TapsNeededForIncrease = 3;
    //private int CurrentTapAmount = 0;
    private float maxPushOutSize;
    private float currentTime;
    public bool test;
    public AnimationCurve pulseCurve;
    public AnimationCurve returnCorouteCurve;
    private float drainAuraSpeed;
    private float StartIncreaseSize;
    public float NormalSize;
    private bool controlAura = true;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void AuraToggle(bool on)
    {
        if (on)
        {
            controlAura = true;
        }
        else
            controlAura = false;


    }
    public void PushOut(float startRadiusSize)
    {
        pushOut = true;


    }
    public void ShrinkBackToNormal()
    {
        StopAllCoroutines();
        StartCoroutine(ShrinkBackToNormalSize());
    }
    //Update is called once per frame
    void Update()
    {

        if (test)
        {
            AuraToggle(true);
            test = false;
        }
        if (controlAura)
        {


            if (pushOut)
            {//if has not reached target
                if (rad.Radius < currentTarget)
                {
                    currentTime += IncreasAuraSpeed * Time.deltaTime;
                    rad.Radius = Mathf.Lerp(StartIncreaseSize, currentTarget, pulseCurve.Evaluate(currentTime));
                    //Aura.Radius += IncreasAuraSpeed * Time.deltaTime;
                }
                else
                {
                    pushOut = false;
                    currentTime = 0f;
                }

            }//drain until zero
            else if (rad.Radius > NormalSize)
            {
                rad.Radius -= drainAuraSpeed * Time.deltaTime;

            }
        }

    }

    private IEnumerator ShrinkBackToNormalSize()
    {
        AuraToggle(false);
        currentTime = 0;
        currentTarget = NormalSize;
        float StartSize = rad.Radius;

        if (rad.Radius > NormalSize)
        {

            while (rad.Radius > NormalSize)
            {

                currentTime += returnToNormalSpeed * Time.deltaTime;
                rad.Radius = Mathf.Lerp(StartSize, currentTarget, returnCorouteCurve.Evaluate(currentTime));
                yield return null;
            }


        }
        else
        {
            while (rad.Radius < NormalSize)
            {
                currentTime += returnToNormalSpeed * Time.deltaTime;
                rad.Radius = Mathf.Lerp(StartSize, currentTarget, returnCorouteCurve.Evaluate(currentTime));
                yield return null;

            }
        }
        rad.Radius = NormalSize;
        
    }

}
