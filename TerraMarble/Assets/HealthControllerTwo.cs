using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthControllerTwo : MonoBehaviour
{
    [SerializeField] private Image healthBarFillImage;
    [SerializeField] private Image shieldBarFillImage;
    [SerializeField] private Image healthBarStatic;
    [SerializeField] private Image shieldBarStatic;
    [SerializeField] private float fillSpeed = 0.5f; // Adjust as needed
    [SerializeField]
    private float currentHealthPercentage = 100f;
    [SerializeField]
    private float currentShieldPercentage = 100f;
    [SerializeField] private bool test;
    public DamageVignette damageVignette;
    public Color ShieldVigColor;
    public Color HealthVigColor;
    public bool flashingHealthBackGround = false;
    public FlashSpriteColor FlashBar;
    
    private void Update()
    {
        // Simulate health and shield changes over time for demonstration purposes
        if (test)
        {
            // Example: Decrease health and shield percentages
           
            if (currentShieldPercentage<=0)
            {
                currentHealthPercentage -= 20f;
                currentHealthPercentage = Mathf.Clamp(currentHealthPercentage, 0f, 100f);
                float targetHealthFillAmount = currentHealthPercentage / 100f;
                healthBarStatic.fillAmount = targetHealthFillAmount;

            }
            else
            {
                currentShieldPercentage -= 15f;
                currentShieldPercentage = Mathf.Clamp(currentShieldPercentage, 0f, 100f);
                float targetHealthFillAmount2 = currentShieldPercentage / 100f;
                shieldBarStatic.fillAmount = targetHealthFillAmount2;
            }

            // Update health bar fill amount over time

           
           

            test = false;
        }

        UpdateBars();
    }

    private void UpdateBars()
    {
        
        
        // Update health bar fill amount over time
        float targetHealthFillAmount = currentHealthPercentage / 100f;
        healthBarFillImage.fillAmount = Mathf.MoveTowards(healthBarFillImage.fillAmount, targetHealthFillAmount, fillSpeed * Time.deltaTime);

        // Update shield bar fill amount over time
        float targetShieldFillAmount = currentShieldPercentage / 100f;
        shieldBarFillImage.fillAmount = Mathf.MoveTowards(shieldBarFillImage.fillAmount, targetShieldFillAmount, fillSpeed * Time.deltaTime);


    }

    // Example method to update health and shield percentages
    public void UpdateHealth(float newHealthPercentage, float damToMaxHealthPercent)
    {
        currentHealthPercentage = Mathf.Clamp(newHealthPercentage *100, 0f, 100f);
        //currentShieldPercentage = Mathf.Clamp(newShieldPercentage, 0f, 100f);
        float targetHealthFillAmount = currentHealthPercentage / 100f;
        healthBarStatic.fillAmount = targetHealthFillAmount;
        damageVignette.setVignetteTarget(damToMaxHealthPercent);
       damageVignette.SetColorToHealth();
        if (!flashingHealthBackGround)
        {
            //turn on flashing health
            ToggleFLashing(true);


        }

    }
    public void ToggleFLashing(bool on)
    {
        if (on)
        {
            FlashBar.enabled = true;
        }else
            FlashBar.enabled = false;
    }
    public void UpdateShield(float newShieldPercentage, float damToMaxShieldPercent)
    {
        currentShieldPercentage = Mathf.Clamp(newShieldPercentage * 100, 0f, 100f);
        float targetsheildFillAmount = currentShieldPercentage / 100f;
        shieldBarStatic.fillAmount = targetsheildFillAmount;
        damageVignette.setVignetteTarget(damToMaxShieldPercent);
        damageVignette.SetColorToShield();
    }
    public void ResetHealth()
    {
        healthBarStatic.fillAmount = 1;
        shieldBarStatic.fillAmount = 1;
        shieldBarFillImage.fillAmount = 1;
        healthBarFillImage.fillAmount = 1;
        currentHealthPercentage = 100;
        currentShieldPercentage = 100;
        ToggleFLashing(false);
    }
}