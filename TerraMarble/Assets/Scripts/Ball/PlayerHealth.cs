using Shapes;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("healthbar or hearts")]
    public bool healthBar = true;
    [SerializeField]
    private HealthControllerTwo healthShieldController;
    [Header("References")]
    [SerializeField] private GameObject healthIconPrefab;
    [SerializeField] private Transform heartContainerUI;

    private const string immuneDiscName = "body, shadow";
    [SerializeField] private Disc immuneDisc;

    [Header("Health Config")]
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool infiniteHealth = true;
    [Header("Shield Config")]
    [SerializeField] private float maxShield = 100f;
    [SerializeField] private float currentShield = 100f;
    [Header("Immune Config")]
    [SerializeField] private float immuneDuration = 2;
    [SerializeField] private float immuneTimeRemaining = 0;
    [SerializeField] private Color immuneColor = Color.white;
    private Color normalDiscColor;
    public bool IsImmune = false;

    [Header("UI Config")]
    [SerializeField] private int iconWidth = 70;

    // Private Fields
    private const string boolDamageName = "Damaged";
    private const string heartContainerName = "HealthUI";

    private List<Animator> heartIcons;
    private int boolDamageID;

    // Properties
    public float CurrentHealth
    {
        get => currentHealth;
        set => SetCurrentHealth(value);
    }

    public float MaxHealth
    {
        get => maxHealth;
        set => SetMaxHealth(value);
    }

    public float CurrentShield
    {
        get => currentShield;
        set => currentShield = Math.Clamp(value, 0f, maxShield);
    }

    public float MaxShield
    {
        get => maxShield;
        set => maxShield = value;
    }

    // Events
    [HideInInspector] public UnityEvent OnDamaged = new();
    [HideInInspector] public UnityEvent OnDeath = new();

    private void Start()
    {
        // Immunity indicator
        immuneDisc = immuneDisc != null ? immuneDisc
            : FindObjectOfType<BallAnimController>()?
                .transform.Find(immuneDiscName)?
                .GetComponent<Disc>();

        if (immuneDisc is not null)
            normalDiscColor = immuneDisc.Color;
        //if (!healthBar)
        //{
        //    //disable healthbar
        //    healthShieldController.gameObject.SetActive(false);

        //    // Health UI
        //    heartContainerUI = heartContainerUI != null ? heartContainerUI
        //    : GameObject.Find(heartContainerName)?.transform;

        //boolDamageID = Animator.StringToHash(boolDamageName);
        ////CreateIcons();
        //}
        //else //using healthbar
        //{
        //heartContainerUI.gameObject.SetActive(false);
        healthShieldController.gameObject.SetActive(true);
        //}
        // Damage Immunity callback
        OnDamaged.AddListener(StartImmune);
    }

    private void Update()
    {
        UpdateImmunity();
    }

    private void UpdateImmunity()
    {
        if (immuneTimeRemaining > 0f)
        {
            immuneTimeRemaining -= Time.deltaTime;
            if (immuneTimeRemaining < 0f)
                EndImmune();
        }
    }

    private void SetCurrentHealth(float newCurrentHealth)
    {
        currentHealth = Math.Clamp(newCurrentHealth, 0, maxHealth);
        //UpdateUI();
    }

    private void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Math.Clamp(currentHealth, 0, maxHealth);
        //CreateIcons();
    }

    private void SetCurrentShield(float newCurrentShield)
    {
        currentShield = Math.Clamp(newCurrentShield, 0f, maxShield);
        UpdateShieldUI();
    }

    private void SetMaxShield(float newMaxShield)
    {
        maxShield = newMaxShield;
        currentShield = Math.Clamp(newMaxShield, 0f, maxShield);
        UpdateShieldUI();
    }

    private void UpdateShieldUI()
    {
        float percent = currentShield * (1f / maxShield);
        healthShieldController.UpdateShield(percent);
    }

    public void ConsumeShield(float amount)
    {
        SetCurrentShield(currentShield - amount);
    }

    public void DamageShield(float amount)
    {
        currentShield = Math.Clamp(currentShield - amount, 0f, maxShield);
        float currentPercent = currentShield * (1f / maxShield);
        float dmgPercent = amount * (1f / maxShield);
        healthShieldController.UpdateShield(currentPercent, dmgPercent);
    }

    private void StartImmune()
    {
        IsImmune = true;
        immuneDisc.Color = immuneColor;
        immuneTimeRemaining = immuneDuration;
    }

    private void EndImmune()
    {
        IsImmune = false;
        immuneDisc.Color = normalDiscColor;
        immuneTimeRemaining = 0f;
    }

    //private void CreateIcons()
    //{
    //    heartIcons = new List<Animator>(maxHealth);

    //    if (Application.isPlaying)
    //        heartContainerUI.DestroyChildren();
    //    else heartContainerUI.DestroyImmediateChildren();

    //    for (int i = 0; i < maxHealth; i++)
    //    {
    //        // Create Icon
    //        GameObject newIcon = GameObject.Instantiate(healthIconPrefab, heartContainerUI) as GameObject;
    //        // Set X
    //        RectTransform rectTransform = newIcon.GetComponent<RectTransform>();
    //        int x = iconWidth * (maxHealth - 1 - i) * -1;
    //        rectTransform.localPosition = rectTransform.localPosition.WithX(x);
    //        // Get Animator
    //        Animator iconAnimator = newIcon.GetComponentInChildren<Animator>();
    //        heartIcons.Add(iconAnimator);
    //    }

    //    UpdateUI();
    //}

    //public void UpdateUI()
    //{
    //    if (!healthBar)
    //    {
    //        if (heartIcons.Count == 0)
    //            Debug.LogError("Missing Health UI Reference");

    //        for (int i = maxHealth; i > 0; i--)
    //        {
    //            bool damaged = i > currentHealth;
    //            heartIcons[maxHealth - i].SetBool(boolDamageID, damaged);
    //        }
    //    }
    //    else
    //    {
    //    }

    //}

    //public void Damage() => Damage(1);

    public void Damage(float amount)
    {
        if (IsImmune) return;

        //if (!healthBar)
        //{
        //    currentHealth = Math.Clamp(currentHealth - amount, 0, maxHealth);

        //    OnDamaged.Invoke();

        //    if (currentHealth == 0)
        //    {
        //        OnDeath.Invoke();

        //        if (infiniteHealth)
        //            SetCurrentHealth(maxHealth);
        //    }
        //}
        //else
        //{
        if (currentShield > 0)
        {
            currentShield = Math.Clamp(currentShield - amount, 0, maxShield);
            healthShieldController.UpdateShield(currentShield * (1 / maxShield), amount * (1 / maxShield));
            //healthShieldController.damageVignette.SetColorToShield();
        }
        else
        if (currentHealth > 0)
        {
            currentHealth = Math.Clamp(currentHealth - amount, 0, maxHealth);
            healthShieldController.UpdateHealth(currentHealth * (1 / maxHealth), amount * (1 / maxHealth));
            // healthShieldController.damageVignette.SetColorToHealth();
        }
        else
        {
            healthShieldController.ResetHealth();
            currentHealth = maxHealth;
            currentShield = maxShield;
            OnDeath.Invoke();
        }
        //}

        //UpdateUI();
    }

    [Button]
    [ShowInInspector]
    private void TestDamage()
        => Damage(1);

    [Button]
    [ShowInInspector]
    private void TestRestoreHealth()
        => SetCurrentHealth(maxHealth);

    [Button]
    [ShowInInspector]
    private void TestIncreaseMaxHealth()
        => SetMaxHealth(maxHealth + 1);

    [Button]
    [ShowInInspector]
    private void TestDecreaseMaxHealth()
        => SetMaxHealth(maxHealth - 1);

    //[Button]
    //[ShowInInspector]
    //private void TestRecreateIcons()
    //    => CreateIcons();
}