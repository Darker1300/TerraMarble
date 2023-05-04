using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlowDownZone : MonoBehaviour
{

    public float slowdownFactor = 0.5f; // How much to slow down the object's speed each update
    public float StuckSlowdownFactor = 0.5f; // How much to slow down the object's speed each update
    public float maxSlowdown = 50f; // The maximum amount of slowdown that can be applied

    private float currentSlowdown = 0f; // The current amount of slowdown applied to objects in the zone
    public bool willgetStuck;
    public LayerMask targetLayer;
    private float velocityToGetStuck = 200f;

    [HideLabel]
    [SerializeField]
    [TextArea(1, 2)]
    private string InfoString = "";
    public UnityEvent OnEnter;
    [HideLabel]
    [SerializeField]
    [TextArea(1, 2)]
    private string InfoString2 = "";
    public UnityEvent OnExit;

    [HideLabel]
    [SerializeField]
    [TextArea(1, 2)]
    private string InfoString3 = "";
    public UnityEvent OnStuck;
    private bool OnstuckHasFiredEvent = false;
    public Transform Player;
    void OnTriggerExit2D(Collider2D other)
    {
        Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
        if (rb != null)
        {
            OnExit?.Invoke();

            currentSlowdown = 0f; // Reset the current slowdown when an object enters the trigger zone

        }
        // Check if the other object is on the target layer
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {

           
            other.GetComponentInParent<seadWeirdGravity>().enabled = true;
        }
        willgetStuck = false;
        OnstuckHasFiredEvent = false;
    }
    public bool WillGetStuck(Vector2 velocity)
    {

        if (velocity.magnitude < velocityToGetStuck)
        {
           
            //currentSlowdown = 500;
            return true;
        }
        else return false;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (targetLayer == (targetLayer | (1 << other.gameObject.layer)))
        {
            Player = other.gameObject.transform;
            Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
            if (rb != null)
            {
                other.GetComponentInParent<seadWeirdGravity>().enabled = false;
                WillGetStuck(rb.velocity);
                OnEnter?.Invoke();

            }

        }
        willgetStuck = true;
    }
    void OnTriggerStay2D(Collider2D other)
    {


        // Increase the current slowdown over time, up to the maximum value
        //if (willgetStuck)
        //{
        //    currentSlowdown = currentSlowdown + StuckSlowdownFactor * Time.deltaTime;

        //}else
        currentSlowdown = Mathf.Min(currentSlowdown + slowdownFactor * Time.deltaTime, maxSlowdown);
        Debug.Log("SlowDown " + currentSlowdown);
        // Check if the other object has a Rigidbody2D component
        Rigidbody2D otherRigidbody = other.GetComponentInParent<Rigidbody2D>();

        // If the other object has a Rigidbody2D component, reduce its velocity based on the current slowdown
        if (otherRigidbody != null)
        {
            if (!OnstuckHasFiredEvent && otherRigidbody.velocity.magnitude == 0 )
            {
                OnStuck?.Invoke();
                OnstuckHasFiredEvent = true;
            }
           
            otherRigidbody.velocity *= Mathf.Clamp01(1f - currentSlowdown);
        }
    }
}