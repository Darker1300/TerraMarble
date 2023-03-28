using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionWetController : MonoBehaviour
{

    public event Action<float> WetUpdate;
    [SerializeField] private float percentagePerMeter = 0.07f;

    [SerializeField] private float maxWetness = 100f;
    [SerializeField] private float wetnessDecreaseRate = 1f;
    //[SerializeField] private UnityEvent<float> wetnessChangedEvent;

    [SerializeField] private float currentWetness = 0;

    private void Awake()
    {
        currentWetness = 0;
    }

    private void Update()
    {
        if (currentWetness <= 0f) return;

        currentWetness = Mathf.Max(currentWetness - (wetnessDecreaseRate * Time.deltaTime), 0f);
        WetUpdate?.Invoke(currentWetness);
    }

    //this is updated from our fruit manager
    public void AddWetness(float distanceFactor)
    {


        float percentageToAdd = distanceFactor * percentagePerMeter;
        currentWetness = Mathf.Min(currentWetness + percentageToAdd, maxWetness);
        WetUpdate?.Invoke(currentWetness);

    }
}
