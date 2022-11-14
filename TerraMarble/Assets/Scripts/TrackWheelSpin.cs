using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TrackWheelSpin : MonoBehaviour
{
    private Wheel wheel = null;
    public float speed = 1f;
    public float PosFactor = 1f;
    public float NegFactor = 1f;

    void Awake()
    {
        wheel = GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        wheel.FixedRotationEvent.AddListener(OnWheelSpin);
    }

    private void OnWheelSpin(float fixedDeltaSpin)
    {
        float spin = fixedDeltaSpin;
        float absSpin = Mathf.Abs(spin) * speed;
        int spinSign = Math.Sign(spin);
        if (spinSign == -1) absSpin *= NegFactor;
        else if (spinSign == 1) absSpin *= PosFactor;

        transform.Rotate(transform.forward, absSpin);
    }
}
