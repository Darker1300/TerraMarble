using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityUtility;

public class RandomGrowth : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private bool doGrow = true;
    [SerializeField] private int attempts = 16;
    [SerializeField] private bool doDebug = false;
    [Header("Rate Per Minute")]
    [SerializeField] private float terraformRate = 30f;
    [SerializeField] private float growRate = 30f;

    private Wheel wheel = null;

    [Header("Data")]
    [SerializeField] private float terraformTimer = 0f;
    [SerializeField] private float growTimer = 0f;

    void Awake()
    {
        wheel = FindObjectOfType<Wheel>();
    }

    void Update()
    {
        if (!doGrow) return;

        terraformTimer += Time.deltaTime * (1f / (terraformRate / 60f));
        growTimer += Time.deltaTime * (1f / (growRate / 60f));

        while (terraformTimer > 1f)
        {
            terraformTimer -= 1f;
            DoTerraform();
        }

        while (growTimer > 1f)
        {
            growTimer -= 1f;
            DoGrow();
        }
    }

    void DoTerraform()
    {
        for (int i = 0; i < attempts; i++)
        {
            int target = Random.Range(0, wheel.regions.RegionCount - 1);
            Region region = wheel.regions[target];

            if (region.animTerraform.Animating)
                continue;

            if (region.regionID == Region.RegionID.Forest)
                continue;

            if (region.regionID == Region.RegionID.Rock)
                if (region.transform.FindChildWithTag("Mountain") != null)
                    continue;

            Region.RegionID newid = (region.regionID + 1);

            if (newid == Region.RegionID.SIZE)
                continue;

            if (doDebug) Debug.Log(string.Format("Terra: {0} | {1} ► {2}", 
                region.gameObject.name, region.regionID, newid));

            region.TerraformRegion(newid);
            break;
        }
    }

    void DoGrow()
    {
        for (int i = 0; i < attempts; i++)
        {
            int target = Random.Range(0, wheel.regions.RegionCount - 1);
            Region region = wheel.regions[target];

            if (region.surfaceObjects.Count == 0)
                continue;

            bool isGrowable = false;
            for (var index = 0; index < region.surfaceObjects.Count; index++)
            {
                Growable surfaceObject = region.surfaceObjects[index];
                if (surfaceObject.animGoalIndex < surfaceObject.animMaxIndex)
                {
                    isGrowable = true;
                    break;
                }
            }
            if (isGrowable == false) continue;
            
            if (doDebug) Debug.Log(string.Format("Grow : {0} | {1}",
                region.gameObject.name, region.regionID));

            region.Grow();

            break;
        }
    }
}
