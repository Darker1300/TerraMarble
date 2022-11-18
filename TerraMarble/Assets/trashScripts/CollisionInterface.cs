using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtility;

public class CollisionInterface : MonoBehaviour
{
    public float sectionValue;
    public WheelGenerator Wheel;
    public float rotationOffset;
    // Start is called before the first frame update
    void Start()
    {
        sectionValue = 360 / Wheel.regionCount;
       
    }
    //GET WHEEL SECTION
    //FIRE AN EVENT FROM THAT SECTION

    //FOR HITTING SECTION

    public void FindWheelSection(Vector2 HitDirection)
    {
                //wheels rotation
        rotationOffset = transform.rotation.z;
        float currentAngle = Mathf.Atan2(HitDirection.y, HitDirection.x) * Mathf.Rad2Deg;
        //angle to hit direction taking account of 
        currentAngle = (((currentAngle + 360) + rotationOffset) % 360) - 1;
        int selection = (int)currentAngle / (int)sectionValue;
        Debug.Log("section + " + selection);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        FindWheelSection( collision.contacts[0].point);
       // collision.contacts[0].point

    }




}