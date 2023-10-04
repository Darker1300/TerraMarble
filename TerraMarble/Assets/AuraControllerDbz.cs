using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
public class AuraControllerDbz : MonoBehaviour
{
    [SerializeField]
    public Disc disc;
    [SerializeField]
    private TrailRenderer auraTrail;
    //[SerializeField]
    //private float rangeMin = 0.5f;
    //[SerializeField]
    //private float rangeMax = 0.5f;

    [Range(0, 1)]
    public float intensity;

    public Material vignette;
    //public AnimationCurve ;
    [SerializeField]
    public float maxIntensity = 0.3f;
    public AnimationCurve damagePercentCurve;
    [SerializeField]
    public float CurrentIntensity;
    [SerializeField]
    private bool Increase;
    [SerializeField]
    private bool Test;

    [SerializeField]
    private float currentTime;
    [SerializeField]
    private float ExpandBackSpeed = 0.3f;
    //[SerializeField]
    //private float maxSize = 0.8f;

    private float currentTarget;

    private float StartIncreaseSize;
    [SerializeField]
    private AnimationCurve pulseCurve;
    [SerializeField]
    private float drainAuraSpeed;
    public Color shieldColor;
    public Color healthColor;
    public float testAmount;
    /// <summary>
    /// onhit 
    /// </summary>
    /// Aura uniform settings
    /// 
    
    [SerializeField]
    private float minAuraDiscSize;
    [SerializeField]
    private float maxAuraDiscSize;
    [Range(0, 1)]
    private float currentAuraSize;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //float newIntensity = Mathf.Sin(Time.time); // Example value
        //vignette.SetFloat("_FullScreenIntensity", newIntensity);

        if (Test)
        {
            setRadiusTarget(testAmount);
            Test = false;
            GetComponent<AuraDIscColorChangeDbz>().LerpToColor();
        }
        UpdateVignette();
    }
    public void TestDam()
    {

        setRadiusTarget(testAmount);
        GetComponent<AuraDIscColorChangeDbz>().LerpToColor();
    }
    public void SetColorToShield()
    {

        vignette.SetColor("_Color", vignette.GetColor("_ColorTwo"));
    }
    public void SetColorToHealth()
    {

        vignette.SetColor("_Color", vignette.GetColor("_ColorOne"));
    }
    private void UpdateVignette()
    {
        //if (CurrentIntensity >= maxSize)
        //{



        //    slowDownZone.currentSlowdown = 0f;
        //    if (!HasFiredDash)
        //    {

        //        currentTarget = 0;
        //        slowDownZone.Player.transform.parent.GetComponent<BallWindJump>().DoDash(50, 1, true);
        //        HasFiredDash = true;
        //        AuraReady?.Invoke();
        //    }

        //}
        if (Increase)
        {//if has not reached target
            if (CurrentIntensity > currentTarget)
            {
                currentTime += ExpandBackSpeed * Time.deltaTime;
                CurrentIntensity = Mathf.Lerp(StartIncreaseSize, currentTarget, pulseCurve.Evaluate(1-currentTime));
                disc.Radius = CurrentIntensity ;
               
                //vignette.SetFloat("_FullScreenIntensity", CurrentIntensity);
            }
            else
            {
                Increase = false;
                currentTime = 0f;
            }

        }//drain until zero
        else if (CurrentIntensity < maxIntensity)
        {
            CurrentIntensity += drainAuraSpeed * Time.deltaTime;
            disc.Radius = CurrentIntensity;
        }

    }

    //value of intensity based on damage amount
    public void setRadiusTarget(float intensity)
    {
        //currentTarget = damagePercentCurve.Evaluate(intensity * maxIntensity);
        currentTarget = intensity;
        Increase = true;
    }
}
