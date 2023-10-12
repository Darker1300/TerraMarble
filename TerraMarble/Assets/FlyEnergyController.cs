using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnergyController : MonoBehaviour
{
    public float dDelayTime;
    private float currentTime;
    public PlayerHealth playerHealth;
    public bool NeedsReplenishing;
    public float lastTimeUsed;
    public float replenishDelay;

    // Start is called before the first frame update
    void Start()
    {
       
    }
    /// <summary>
    /// if sheild is not full 
    /// minus delay 
    /// if delay 
    /// 
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        //if (NeedsReplenishing && Time.time - lastTimeUsed >= replenishDelay)
        //{
        //    playerHealth.
        //}
        //else
            //if shield is bellow
            //last damage received cached 
            //UpdateBars();
    }
}
