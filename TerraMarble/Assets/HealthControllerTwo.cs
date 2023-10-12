using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthControllerTwo : MonoBehaviour
{
    [SerializeField] private Image healthBarFillImage;
    [SerializeField] private Image flyBarFillImage;
    
    [SerializeField] private Image shieldBarFillImage;
    [SerializeField] private Image healthBarStatic;
    [SerializeField] private Image shieldBarStatic;
    [SerializeField] private Image flybarBarStatic;
     private float currentFillSpeed = 0.5f; // Adjust as needed
    
    [SerializeField] private float currentHealthPercentage = 100f;
    [SerializeField] private float currentShieldPercentage = 100f;
    [SerializeField] private bool test;
    public DamageVignette damageVignette;
    public Color ShieldVigColor;
    public Color HealthVigColor;
    public bool flashingHealthBackGround = false;
    public FlashSpriteColor FlashBar;
    [SerializeField] private AuraControllerDbz AuraController;
    private float lastTimeDamaged;
    private bool needsHealing;
    [SerializeField] private float healingDelay = 3f;
    [SerializeField] private float replenishSpeed = 0.5f; // Adjust as needed
    [SerializeField] private float damageSpeed = 0.5f; // Adjust as needed
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private TrailScaler trailScaler;
    /// <summary>
    /// fly energy variable

    /// </summary>
    [SerializeField] private float RechargeFlyDelay = 3f;
    [SerializeField] private float replenishFlySpeed = 0.5f; // Adjust as needed
    [SerializeField] private float FlyDepletionSpeed = 0.5f; // Adjust as needed
    private float lastTimeFlyUsed;
    private bool needsFlyRecharge;
    private float currentFlyBarPercentage = 100f;
    public float flyBarCurrentFillSpeed=0.5f;
    private void Start()
    {
        trailScaler = trailScaler != null ? trailScaler
            : GetComponentInChildren<TrailScaler>();
    }

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

        //healing will stop 
        if (needsHealing && Time.time - lastTimeDamaged >= healingDelay)
        {
            ReplenishBars();
        } else 
            //if shield is bellow
            //last damage received cached 
            UpdateBars();



        //FLY STUFF
        FlyHandler();

    }
    private void FlyHandler()
    {
        if (playerHealth.UseFlyEnergy && needsFlyRecharge && Time.time - lastTimeFlyUsed >= RechargeFlyDelay)
        {
            ReplenishFlyValue();
        }
        else
            //if shield is bellow
            //last damage received cached 
            UpdateFlyBar();

    }
    private void ReplenishFlyValue()
    {

        float targetFlyFillAmount = 1f;
        flybarBarStatic.fillAmount = Mathf.MoveTowards(flybarBarStatic.fillAmount, targetFlyFillAmount, flyBarCurrentFillSpeed * Time.deltaTime);
        
        currentFlyBarPercentage = flybarBarStatic.fillAmount;
        playerHealth.FlyEnergyCurrent = playerHealth.flyEnergyMax;
      
    }
    private void UpdateFlyBar()
    {
        // Update health bar fill amount over time
        float targetFlyFillAmount = currentFlyBarPercentage / 100f;
        flyBarFillImage.fillAmount = Mathf.MoveTowards(flyBarFillImage.fillAmount, targetFlyFillAmount, currentFillSpeed * Time.deltaTime);
        needsFlyRecharge = true;
    }
    //private void ReplenishFlyBar()
    //{

    //}
    private void UpdateBars()
    {
        // Update health bar fill amount over time
        float targetHealthFillAmount = currentHealthPercentage / 100f;
        healthBarFillImage.fillAmount = Mathf.MoveTowards(healthBarFillImage.fillAmount, targetHealthFillAmount, currentFillSpeed * Time.deltaTime);

        // Update shield bar fill amount over time
        float targetShieldFillAmount = currentShieldPercentage / 100f;
        shieldBarFillImage.fillAmount = Mathf.MoveTowards(shieldBarFillImage.fillAmount, targetShieldFillAmount, currentFillSpeed * Time.deltaTime);

        // Trail
        trailScaler.UpdateTrail(playerHealth.CurrentShield * (1f / playerHealth.MaxShield));
    }

    private void ReplenishBars()
    {
        // Update health bar fill amount over time
        float targetHealthFillAmount = 1f;
        healthBarStatic.fillAmount = Mathf.MoveTowards(healthBarStatic.fillAmount, targetHealthFillAmount, currentFillSpeed * Time.deltaTime);
        shieldBarFillImage.fillAmount = Mathf.MoveTowards(shieldBarFillImage.fillAmount, targetHealthFillAmount, currentFillSpeed * Time.deltaTime);
        currentHealthPercentage = healthBarStatic.fillAmount;
        playerHealth.CurrentHealth = playerHealth.MaxHealth;
        playerHealth.CurrentShield = playerHealth.MaxShield;

        // Update shield bar fill amount over time
        float targetShieldFillAmount =  1f;
        shieldBarStatic.fillAmount = Mathf.MoveTowards(shieldBarStatic.fillAmount, targetShieldFillAmount, currentFillSpeed * Time.deltaTime);
        shieldBarFillImage.fillAmount = Mathf.MoveTowards(shieldBarFillImage.fillAmount, targetShieldFillAmount, currentFillSpeed * Time.deltaTime);
        currentShieldPercentage = shieldBarStatic.fillAmount;
        if (Mathf.Approximately(playerHealth.CurrentHealth, playerHealth.MaxHealth))
        {
            //turn on flashing health
            ToggleFlashing(false);
            flashingHealthBackGround = false;
        }
        // trail
        trailScaler.UpdateTrail(playerHealth.CurrentShield * (1f / playerHealth.MaxShield));
        currentFillSpeed = replenishSpeed;
    }

    // Example method to update health and shield percentages
    public void UpdateHealth(float newHealthPercentage, float damToMaxHealthPercent)
    {
        lastTimeDamaged = Time.time;
        needsHealing = true;
        currentFillSpeed = damageSpeed;
        currentHealthPercentage = Mathf.Clamp(newHealthPercentage *100, 0f, 100f);
        //currentShieldPercentage = Mathf.Clamp(newShieldPercentage, 0f, 100f);
        float targetHealthFillAmount = currentHealthPercentage / 100f;
        healthBarStatic.fillAmount = targetHealthFillAmount;
        damageVignette.setVignetteTarget(damToMaxHealthPercent);
       damageVignette.SetColorToHealth();
        AuraController.TestDam();
        if (!flashingHealthBackGround)
        {
            //turn on flashing health
            ToggleFlashing(true);
        }
        
    }
    public void UpdateFlyEnergy(float newCurrentFlyBarPercent,float damToMaxFlyPercent)
    {
        lastTimeFlyUsed = Time.time;
        needsFlyRecharge = true;
        currentFillSpeed = FlyDepletionSpeed;
        currentFlyBarPercentage = Mathf.Clamp(newCurrentFlyBarPercent * 100, 0f, 100f);
        //currentShieldPercentage = Mathf.Clamp(newShieldPercentage, 0f, 100f);
        float flyEnergyFillAmount = currentFlyBarPercentage / 100f;
        flybarBarStatic.fillAmount = flyEnergyFillAmount;

    }
    public void UpdateShield(float newCurrentShieldPercent)
    {
        lastTimeDamaged = Time.time;
        needsHealing = true;

        currentShieldPercentage = Mathf.Clamp(newCurrentShieldPercent * 100, 0f, 100f);
        float targetShieldFillAmount = currentShieldPercentage / 100f;
        shieldBarStatic.fillAmount = targetShieldFillAmount;
        currentFillSpeed = damageSpeed;
    }

    public void UpdateDamageVignette(float shieldDamagePercent)
    {
        damageVignette.setVignetteTarget(shieldDamagePercent);
        damageVignette.SetColorToShield();
        AuraController.TestDam();
    }

    public void UpdateShield(float newCurrentShieldPercent, float shieldDamagePercent)
    {
        currentFillSpeed = damageSpeed;
        UpdateShield(newCurrentShieldPercent);
        UpdateDamageVignette(shieldDamagePercent);
    }

    public void ToggleFlashing(bool on)
    {
        if (on)
        {
            FlashBar.enabled = true;
        }else
            FlashBar.enabled = false;
    }
  
    public void ResetHealth()
    {
        healthBarStatic.fillAmount = 1;
        shieldBarStatic.fillAmount = 1;
        shieldBarFillImage.fillAmount = 1;
        healthBarFillImage.fillAmount = 1;
        currentHealthPercentage = 100;
        currentShieldPercentage = 100;
        ToggleFlashing(false);
    }
}