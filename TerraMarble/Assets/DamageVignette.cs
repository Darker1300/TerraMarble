using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
public class DamageVignette : MonoBehaviour
{

    [Range(0,1)]
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
    private float IncreasVignetteSpeed = 0.3f;
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
    /// <summary>
    /// onhit 
    /// </summary>

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
            setVignetteTarget(0.15f);
            Test = false;
        }
        UpdateVignette();
    }
    public void SetColorToShield()
    {
        
        vignette.SetColor( "_Color", vignette.GetColor("_ColorTwo"));
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
            if (CurrentIntensity < currentTarget)
            {
                currentTime += IncreasVignetteSpeed * Time.deltaTime;
                CurrentIntensity = Mathf.Lerp(StartIncreaseSize, currentTarget, pulseCurve.Evaluate(currentTime));
                vignette.SetFloat("_FullScreenIntensity", CurrentIntensity);
            }
            else
            {
                Increase = false;
                currentTime = 0f;
            }

        }//drain until zero
        else if (CurrentIntensity > 0.0f)
        {
            CurrentIntensity -= drainAuraSpeed * Time.deltaTime;
            vignette.SetFloat("_FullScreenIntensity", CurrentIntensity);

        }

    }

    //value of intensity based on damage amount
    public void setVignetteTarget(float intensity)
    {
        //currentTarget = damagePercentCurve.Evaluate( intensity * maxIntensity);
        currentTarget = maxIntensity;
        //currentTarget = intensity;
        Increase = true;
    }

}

   


