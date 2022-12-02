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
            regionsMan
                .GetClosestRegion(collision.GetContact(0).point)
                .TerraformRegion(Region.RegionID.Dirt);
    }
}