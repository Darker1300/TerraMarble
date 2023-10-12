using System.Collections;
using System.Collections.Generic;
using MathUtility;
using UnityEngine;

public class VelocityBoost : MonoBehaviour
{
    public float impulseForce = 5.0f; // Adjust this value as needed
    public string collisionTag = "YourTag"; // Set the desired tag in the Inspector
    private Rigidbody2D rb;
    [SerializeField] private float minVelocity = 0.1f; 
    private Vector2 lastVelocity;
    [SerializeField] private bool test;
    public Vector2 debugDirection;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Store the current velocity at the beginning of each frame
        lastVelocity = rb.velocity;
        Debug.DrawLine(transform.position, (Vector2)transform.position + debugDirection * 10f,Color.red);
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

            // Calculate the direction of movement
            Vector2 movementDirection = rb.velocity.normalized;

            // Get the collision normal from the first contact point
            Vector2 collisionNormal = collision.contacts[0].normal;

            // Calculate the direction of the surface relative to the movement direction
            Vector2 surfaceDirection = Vector2.Reflect(movementDirection, collisionNormal);


            debugDirection = surfaceDirection.normalized;
            Vector2 impulseDirection = lastVelocity.normalized;
            //Debug.Log("working slide rock");
            // Apply an impulse in the direction it was going
            rb.AddForce(surfaceDirection * impulseForce, ForceMode2D.Impulse);
        }
        else if (collision.gameObject.CompareTag("Wheel"))
        {
            Debug.Log("working slide wheel");
            // Calculate the impulse direction based on the previous velocity

            // Calculate the direction of movement
            //Vector2 movementDirection = lastVelocity;

            // Get the collision normal from the first contact point
            Vector2 collisionNormal = collision.contacts[0].normal;

            // Calculate the direction of the surface relative to the movement direction
            Vector2 reflectedVelocity = Vector2.Reflect(lastVelocity, collisionNormal);//(Vector2.Reflect(movementDirection, collisionNormal) + transform.up.To2DXY()) * 0.5f;


            debugDirection = reflectedVelocity.normalized;
            //Vector2 impulseDirection = lastVelocity.normalized;

            rb.velocity = reflectedVelocity;
            //rb.AddForce(surfaceDirection, ForceMode2D.Impulse);
        }
    }

}
