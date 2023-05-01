using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowDownZone : MonoBehaviour
{

    public float slowdownFactor = 0.5f; // How much to slow down the object's speed each update
    public float StuckSlowdownFactor = 0.5f; // How much to slow down the object's speed each update
    public float maxSlowdown = 50f; // The maximum amount of slowdown that can be applied

    private float currentSlowdown = 0f; // The current amount of slowdown applied to objects in the zone
    public bool willgetStuck;
    public LayerMask targetLayer;
    private float velocityToGetStuck = 200f;
    void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
        if (rb != null)
        {


            currentSlowdown = 0f; // Reset the current slowdown when an object enters the trigger zone

        }
        // Check if the other object is on the target layer
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {

           
            other.GetComponentInParent<seadWeirdGravity>().enabled = true;
        }
        willgetStuck = false;
    }
    public bool WillGetStuck(Vector2 velocity)
    {

        if (velocity.magnitude < velocityToGetStuck)
        {
            willgetStuck = true;
            //currentSlowdown = 500;
            return true;
        }
        else return false;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {

            Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
            if (rb != null)
            {
                other.GetComponentInParent<seadWeirdGravity>().enabled = false;
                WillGetStuck(rb.velocity);
              

            }
        }
    }
    void OnTriggerStay2D(Collider2D other)
    {


        // Increase the current slowdown over time, up to the maximum value
        //if (willgetStuck)
        //{
        //    currentSlowdown = currentSlowdown + StuckSlowdownFactor * Time.deltaTime;

        //}else
        currentSlowdown = Mathf.Min(currentSlowdown + slowdownFactor * Time.deltaTime, maxSlowdown);

        // Check if the other object has a Rigidbody2D component
        Rigidbody2D otherRigidbody = other.GetComponent<Rigidbody2D>();

        // If the other object has a Rigidbody2D component, reduce its velocity based on the current slowdown
        if (otherRigidbody != null)
        {


            otherRigidbody.velocity *= 1f - currentSlowdown;

        }
    }
}