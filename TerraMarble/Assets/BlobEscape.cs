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
    public SlowDownZone slowDownZone;
    public Color PulseColor;
    public float drainAuraSpeed = 0.5f;
    public float IncreasAuraSpeed = 0.5f;
    public float increaseAuraAmount = 1;
    private float currentTarget;
    private bool Increase;
    public int TapsNeededForIncrease = 3;
    private int CurrentTapAmount = 0;
    private float StartIncreaseSize;
    private float currentTime;
    public bool test;
    public AnimationCurve pulseCurve;
    public bool OveralToggle;
    public float maxSize = 5f;
    //public ChangeColorOverTime colorChange;

    //public Color PulseColor;
    public void ToggleOn()
    {
        OveralToggle = true;
        InputManager.DoubleTapLeft += OnTap;
        InputManager.DoubleTapRight+= OnTap;
        Aura = slowDownZone.Player.transform.parent.GetComponentInChildren<AuraManager>().Aura;
        //colorChange.shapeRender = slowDownZone.Player.transform.parent.GetComponentInChildren<AuraManager>().pulseRing;
    }

    public void ToggleOff()
    {
        OveralToggle = false;
        InputManager.DoubleTapLeft -= OnTap;
        InputManager.DoubleTapRight -= OnTap;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (OveralToggle)
        {


            if (test)
            {
                OnTap();
                test = false;
            }
            if (Aura.Radius >= maxSize)
            {
                Aura.Radius = maxSize;
                OveralToggle = false;
                AuraReady?.Invoke();

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
}
