using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtility;

public class TurretSpinnerController : MonoBehaviour
{
    private float minSpinVelocity;
    public Transform Wheel;
    public VelocitySpinner velSpinner;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void InitialSpeedCalculate(Rigidbody2D rb)
    {
        Vector3 up = rb.transform.position;
        Vector3 right = Vector3.Cross(transform.forward, up);
        bool goingRight = (Vector3.Dot(rb.velocity, right) > 0) ? true : false;

       // Debug.Log("going right" + goingRight);

        float projectedVelocityMagnitude;
        if (goingRight)
        {

            projectedVelocityMagnitude = Vector3.Dot(rb.velocity, right) * rb.velocity.magnitude;
            velSpinner.AddSpin(goingRight);
        }
        else
        { 
            projectedVelocityMagnitude =  Vector3.Dot(rb.velocity, -right) * rb.velocity.magnitude;
            velSpinner.AddSpin(goingRight);
        }

        //Debug.Log("velocity" + projectedVelocityMagnitude);


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
             //Debug.Log("col");
            InitialSpeedCalculate(collision.transform.GetComponentInParent<Rigidbody2D>());

        }
    }
}
