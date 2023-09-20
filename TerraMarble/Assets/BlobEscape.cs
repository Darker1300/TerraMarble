using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UnityEngine.Events;

public class BlobEscape : MonoBehaviour
{
    public UnityEvent AuraReady;
    public Disc Aura;
    public AuraManager AuraManager;
    public SlowDownZone slowDownZone;
    public Color PulseColor;
    public float drainAuraSpeed = 0.5f;
    public float IncreasAuraSpeed = 0.5f;
    public float increaseAuraAmount = 1;
    private float currentTarget;
    private bool Increase;
    public int TapsNeededForIncrease = 3;
    
    public int CurrentTapAmount = 0;
    private float StartIncreaseSize;
    private float currentTime;
    public bool test;
    public AnimationCurve pulseCurve;
    public bool HasFiredDash = false;
    public float maxSize = 5f;
    public RegularPolygon OuterRing;
    //public ChangeColorOverTime colorChange;

    //public Color PulseColor;
    public void ToggleOn()
    {
        
        InputManager.DoubleTapLeft += OnTap;
        InputManager.DoubleTapRight+= OnTap;
        Aura = slowDownZone.Player.transform.parent.GetComponentInChildren<AuraManager>().Aura;

        HasFiredDash = false;
        //colorChange.shapeRender = slowDownZone.Player.transform.parent.GetComponentInChildren<AuraManager>().pulseRing;
    }

    public void OnStuckBlob()
    {
        AuraManager.FadeIn();
        OuterRing.Thickness = 1.05f;
        //AuraManager.DisableAuraControl();
        //AuraManager.FadeIn();
        //AuraManager.Aura.Radius = 5;
    }
    public void OnUnStuck()
    {
        AuraManager.ShrinkAndFade();
        OuterRing.Thickness = 0.05f;

    }

    public void ToggleOff()
    {
        
        InputManager.DoubleTapLeft -= OnTap;
        InputManager.DoubleTapRight -= OnTap;
        //OnUnStuck();
        //Aura.Radius = 0f;
        //AuraManager.DisableAuraControl();
    }

    private void OnTap()
    {
       
        ++CurrentTapAmount;
        //as you tap a radius increases which fights with the radius trying to shrink to zero 
        // there will be big pulses which happen at different radius sizes 
        if (CurrentTapAmount >= TapsNeededForIncrease)
        {
            CurrentTapAmount = 0;
            Increase = true;
            currentTarget = Aura.Radius + increaseAuraAmount;
            StartIncreaseSize = Aura.Radius;
        }

    }


    // Start is called before the first frame update
    void Start()
    {
        slowDownZone = GetComponent<SlowDownZone>();
        AuraManager = GameObject.FindWithTag("Ball").transform.GetComponentInChildren<AuraManager>();
        this.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        


            if (test)
            {
                OnTap();
                test = false;
            }
            if (Aura.Radius >= maxSize)
            {
                //Aura.Radius = maxSize;
                
                slowDownZone.currentSlowdown = 0f;
            if (!HasFiredDash)
            {
                //Aura.Radius = .5f;
                currentTarget = 0;
                slowDownZone.Player.transform.parent.GetComponent<BallWindJump>().DoDash(50, 1,true);
                HasFiredDash = true;
                AuraReady?.Invoke();
            }

            }
            if (Increase)
            {//if has not reached target
                if (Aura.Radius < currentTarget)
                {
                    currentTime += IncreasAuraSpeed * Time.deltaTime;
                    Aura.Radius = Mathf.Lerp(StartIncreaseSize, currentTarget, pulseCurve.Evaluate(currentTime));
                    //Aura.Radius += IncreasAuraSpeed * Time.deltaTime;
                }
                else
                {
                    Increase = false;
                    currentTime = 0f;
                }

            }//drain until zero
            else if (Aura.Radius > 0)
            {
                Aura.Radius -= drainAuraSpeed * Time.deltaTime;

            }
        
    }
}
