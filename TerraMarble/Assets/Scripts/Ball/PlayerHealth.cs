using MathUtility;
using Shapes;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject healthIconPrefab;
    [SerializeField] private Transform heartContainerUI;

    private const string immuneDiscName = "body, shadow";
    [SerializeField] private Disc immuneDisc;

    [Header("Health Config")]
    [SerializeField] private int currentHealth = 3;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private bool infiniteHealth = true;

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
    public int CurrentHealth
    {
        get => currentHealth;
        set => SetCurrentHealth(value);
    }

    public int MaxHealth
    {
        get => maxHealth;
        set => SetMaxHealth(value);
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

        // Health UI
        heartContainerUI = heartContainerUI != null ? heartContainerUI
            : GameObject.Find(heartContainerName)?.transform;

        boolDamageID = Animator.StringToHash(boolDamageName);
        CreateIcons();

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

    private void CreateIcons()
    {
        heartIcons = new List<Animator>(maxHealth);

        if (Application.isPlaying)
            heartContainerUI.DestroyChildren();
        else heartContainerUI.DestroyImmediateChildren();

        for (int i = 0; i < maxHealth; i++)
        {
            // Create Icon
            GameObject newIcon = GameObject.Instantiate(healthIconPrefab, heartContainerUI) as GameObject;
            // Set X
            RectTransform rectTransform = newIcon.GetComponent<RectTransform>();
            int x = iconWidth * (maxHealth - 1 - i) * -1;
            rectTransform.localPosition = rectTransform.localPosition.WithX(x);
            // Get Animator
            Animator iconAnimator = newIcon.GetComponentInChildren<Animator>();
            heartIcons.Add(iconAnimator);
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        if (heartIcons.Count == 0)
            Debug.LogError("Missing Health UI Reference");

        for (int i = maxHealth; i > 0; i--)
        {
            bool damaged = i > currentHealth;
            heartIcons[maxHealth - i].SetBool(boolDamageID, damaged);
        }
    }

    public void Damage() => Damage(1);

    public void Damage(int amount)
    {
        if (IsImmune) return;

        currentHealth = Math.Clamp(currentHealth - amount, 0, maxHealth);

        OnDamaged.Invoke();

        if (currentHealth == 0)
        {
            OnDeath.Invoke();

            if (infiniteHealth)
                SetCurrentHealth(maxHealth);
        }

        UpdateUI();
    }

    private void SetCurrentHealth(int newCurrentHealth)
    {
        currentHealth = Math.Clamp(newCurrentHealth, 0, maxHealth);
        UpdateUI();
    }

    private void SetMaxHealth(int newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Math.Clamp(currentHealth, 0, maxHealth);
        CreateIcons();
    }

    [Button] [ShowInInspector]
    private void TestDamage() 
        => Damage(1);

    [Button] [ShowInInspector]
    private void TestRestoreHealth() 
        => SetCurrentHealth(maxHealth);

    [Button] [ShowInInspector]
    private void TestIncreaseMaxHealth() 
        => SetMaxHealth(maxHealth + 1);

    [Button] [ShowInInspector]
    private void TestDecreaseMaxHealth() 
        => SetMaxHealth(maxHealth - 1);
    
    [Button] [ShowInInspector]
    private void TestRecreateIcons() 
        => CreateIcons();

}