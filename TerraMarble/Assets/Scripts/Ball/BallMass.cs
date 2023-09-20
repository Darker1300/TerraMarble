using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class BallMass : MonoBehaviour
{
    [SerializeField] private int defaultMassIndex = 0;
    public List<string> massOptions = new() {"0.5", "0.75", "1", "1.25", "1.5", "1.75", "2"};

    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private CycleButton massButton;
    private const string massButtonName = "Mass Button";
    
    private void Start()
    {
        rigidBody = rigidBody != null ? rigidBody
            : GetComponent<Rigidbody2D>();

        massButton = massButton != null ? massButton
            : UnityU.FindObjectByName<CycleButton>(massButtonName, true);
        massButton?.Initialise(defaultMassIndex, massOptions, UpdateMass);
    }

    private void UpdateMass(string _massOption)
    {
        if (!rigidBody) return;
        rigidBody.mass = Convert.ToSingle(_massOption);
    }
}