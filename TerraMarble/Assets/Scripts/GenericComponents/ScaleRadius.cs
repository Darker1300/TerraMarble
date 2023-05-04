using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class ScaleRadius : MonoBehaviour
{
    public Disc rad;
    //public SlowDownZone slowDownZone;
    public Color PulseColor;
    public float fadeAuraSpeed = 0.5f;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void ToggleOn(float startRadiusSize)
    {
        pushOut = true;

        
    }
    // Update is called once per frame
    //void Update()
    //{
    //    if (test)
    //    {
    //        OnTap();
    //        test = false;
    //    }

    //    if (pushOut)
    //    {//if has not reached target
    //        if (Aura.Radius < currentTarget)
    //        {
    //            currentTime += IncreasAuraSpeed * Time.deltaTime;
    //            Aura.Radius = Mathf.Lerp(StartIncreaseSize, currentTarget, pulseCurve.Evaluate(currentTime));
    //            //Aura.Radius += IncreasAuraSpeed * Time.deltaTime;
    //        }
    //        else
    //        {
    //            pushOut = false;
    //            currentTime = 0f;
    //        }

    //    }//drain until zero
    //    else if (Aura.Radius > 0)
    //    {
    //        Aura.Radius -= drainAuraSpeed * Time.deltaTime;

    //    }
    //}

    
}
