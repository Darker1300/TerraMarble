using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityBoost : MonoBehaviour
{
    public float impulseForce = 5.0f; // Adjust this value as needed
    public string collisionTag = "YourTag"; // Set the desired tag in the Inspector
    private Rigidbody2D rb;
    [SerializeField] private float minVelocity = 0.1f; 
    private Vector2 lastVelocity;
    [SerializeField] private bool test;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Store the current velocity at the beginning of each frame
        lastVelocity = rb.velocity;
    }
    private void FixedUpdate()
    {
        if (test)
        {
            // Calculate the impulse direction based on the previous velocity
            Vector2 impulseDirection = lastVelocity.normalized;
            //Debug.Log("working slide rock");
            // Apply an impulse in the direction it was going
            rb.AddForce(impulseDirection * impulseForce, ForceMode2D.Impulse);
            test = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.relativeVelocity.magnitude > 0.1f && collision.collider.gameObject.CompareTag(collisionTag))
        {
            Debug.Log("working slide rock");
            // Calculate the impulse direction based on the previous velocity
            Vector2 impulseDirection = lastVelocity.normalized;
            //Debug.Log("working slide rock");
            // Apply an impulse in the direction it was going
            rb.AddForce(impulseDirection * impulseForce, ForceMode2D.Impulse);
        }
    }
}
