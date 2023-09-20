using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionWetController : MonoBehaviour
{

    public event Action<float> WetUpdate;
    public event Action SpawnFruitEvent;

    
    [SerializeField] private float percentagePerMeter = 0.07f;
    [SerializeField] private float maxWetFromFruit = 20f;

    [SerializeField] private float maxWetness = 100f;
    [SerializeField] private float wetnessDecreaseRate = 1f;
    private Vector3 Base_position;
    private ForestController forestController;

    //[SerializeField] private UnityEvent<float> wetnessChangedEvent;

    [SerializeField] private float currentWetness = 0;

    private void Awake()
    {
        Base_position = transform.GetChild(0).transform.position;
        currentWetness = 0;
        
    }

    private void Update()
    {
        if (currentWetness <= 0f) return;

        currentWetness = Mathf.Max(currentWetness - (wetnessDecreaseRate * Time.deltaTime), 0f);
        WetUpdate?.Invoke(currentWetness);
    }

    //this is updated from our fruit manager
    public void AddWetness(Vector2 surfacePos, float radiusSize)
    {
        //0-1 factor
        float distanceFactor = 1 - Vector2.Distance(surfacePos, Base_position) / radiusSize;
        //Debug.Log("factor "+ distanceFactor + "original " + Vector2.Distance(surfacePos, transform.position));
        //float percentageToAdd = distanceFactor * percentagePerMeter;
        currentWetness = Mathf.Min(currentWetness + ((distanceFactor) * maxWetFromFruit), maxWetness);
        WetUpdate?.Invoke(currentWetness);
        if (currentWetness > 30)
        {
             SpawnFruitEvent?.Invoke();
            currentWetness = 0f;
        }
        

    }
   
    public void AddWetnessTest()
    {
        currentWetness = 100f;
        WetUpdate?.Invoke(currentWetness);
    }
}
