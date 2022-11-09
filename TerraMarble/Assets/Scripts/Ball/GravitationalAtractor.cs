using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationalAtractor : MonoBehaviour
{

   
    public bool applyGravity;
    //public Vector2 planetGravity;
    Rigidbody2D rb;

    public float gravityForce;
    public float gravityDistance;
    float lookAngle;


    // Start is called before the first frame update
    void Start()
    {
        
    }


   public Vector2 GetObjectToPlanet(Vector3 objToCompareGravity)
    {

        float dist = Vector3.Distance(objToCompareGravity, transform.position);
        Vector3 v = transform.position - objToCompareGravity;

        Vector2 objectToPlanetGravity = v.normalized * (1.0f - dist / gravityDistance) * gravityForce;

        //lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
        
        return objectToPlanetGravity;

    }
    public Vector2 GetObjectToPlanet(Vector3 objToCompareGravity,ref Quaternion rot)
    {

        float dist = Vector3.Distance(objToCompareGravity, transform.position);
        Vector3 v = transform.position - objToCompareGravity;
        Vector2 objectToPlanetGravity = v.normalized * (1.0f - dist / gravityDistance) ;

        lookAngle = 90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        rot = Quaternion.Euler(0f, 0f, lookAngle);


        return objectToPlanetGravity;

    }

    // Update is called once per frame
    void Update()
    {
        

    }
    
}
