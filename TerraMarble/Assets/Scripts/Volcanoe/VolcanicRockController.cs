using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanicRockController : MonoBehaviour
{
    private WheelRegionsManager regionsMan;

    void Start()
    {
        regionsMan ??= FindObjectOfType<WheelRegionsManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Region.RegionHitInfo info = regionsMan.ProcessWheelHit(collision, null);

        if (info.region.regionID == Region.RegionID.Water)
        {
            // do create Land
        }
        else if (info.surfaceObj is not null)
        {
            // hit tree or other surface object
        }
        else
        {
            // hit a Land region, destroy land
        }
    }
}
