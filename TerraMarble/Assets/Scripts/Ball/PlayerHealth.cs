using MathUtility;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private GameObject healthIconPrefab;

    [SerializeField] private Transform heartContainerUI;

    [Header("Config")] [SerializeField] private int currentHealth = 3;
    [SerializeField] private int maxHealth = 3;

    [Header("UI Config")] [SerializeField] private int iconWidth = 70;

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
        heartContainerUI = heartContainerUI != null
            ? heartContainerUI
            : GameObject.Find(heartContainerName)?.transform;
        boolDamageID = Animator.StringToHash(boolDamageName);
        CreateIcons();
    }

    private void CreateIcons()
    {
        heartIcons = new List<Animator>(maxHealth);
        heartContainerUI.DestroyChildren();
        for (int i = 0; i < maxHealth; i++)
        {
            // Create Icon
            GameObject newIcon = PrefabUtility.InstantiatePrefab(healthIconPrefab, heartContainerUI) as GameObject;
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

    public void Damage(int amount = 1)
    {
        currentHealth = Math.Clamp(currentHealth - amount, 0, maxHealth);

        OnDamaged.Invoke();

        if (currentHealth == 0)
            OnDeath.Invoke();

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
    {
        Damage(1);
    }

    [Button] [ShowInInspector]
    private void TestRestoreHealth()
    {
        SetCurrentHealth(maxHealth);
    }

    [Button] [ShowInInspector]
    private void TestIncreaseMaxHealth()
    {
        SetMaxHealth(maxHealth + 1);
    }

    [Button] [ShowInInspector]
    private void TestDecreaseMaxHealth()
    {
        SetMaxHealth(maxHealth - 1);
    }
}