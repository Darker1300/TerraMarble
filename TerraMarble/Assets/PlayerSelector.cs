using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSelector : MonoBehaviour
{
    public List<GameObject> Balls;
    public GameObject currentObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //drag out angle 

    //check for closest ball drag angle and select it
    //this happens when the min is exceeded and 
    //if player delta exceeds lock
    // rotate offset to this

    //if goes below min it deselects

    ////////////////////////////////////////////
    ///get angle , get closest object, s
    //public float GetAngle()
    //{
    //    currentObj = Balls[0];
    //    for (int i = 0; i < Balls.Count; i++)
    //    {
    //        //compare sign angle
    //        //if less than current then overwrite current

    //    }
    
    //}

    public Vector2 RotateToAngle(Vector2 AimDirection, float angle)
    {
       // AimDirection = (Vector2)(Quaternion.Euler(0, 0, angle) * updateGravityScript.direction.normalized);
       
        AimDirection = (AimDirection * 20);
        //Debug.DrawLine(transform.position, transform.position + transform.rotation * (Vector3)AimDirection, Color.red);
        //Debug.DrawLine(transform.position, transform.position + transform.rotation * (Vector3)dirFake, Color.blue);
        return AimDirection;
    }
    public void swapOut()
    { 
    
    
    }
}
