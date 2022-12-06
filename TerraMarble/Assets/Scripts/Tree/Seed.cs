using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    private WheelRegionsManager regionsMan;

    private void Awake()
    {
        regionsMan = FindObjectOfType<WheelRegionsManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!regionsMan) return;

        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Wheel"))
        {
            Region reg = regionsMan.GetClosestRegion(collision.GetContact(0).point);
            if (reg.regionID == Region.RegionID.Sand ||
                reg.regionID == Region.RegionID.Dirt ||
                reg.regionID == Region.RegionID.Grass)
                reg.TerraformRegion(Region.RegionID.Forest);
        }
    }
}