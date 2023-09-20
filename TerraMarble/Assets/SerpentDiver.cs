using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SerpentDiver : MonoBehaviour
{
    public float shootForce = 10f; // Adjust this to control the force of the shot.
    public float pullStrength = 2f; // Adjust this to control the strength of the pull.
    public bool enablePull = false; // Toggle to enable/disable the gentle pull.
    public Transform target; // The target to shoot at.
    public Transform pullTarget; // The target to shoot at.
    public Transform world;
    private Rigidbody2D rb;
    public bool test;
    public float distanceBeforeTarget;
    public bool canshoot;
    public float maxVelocity;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Shoot();
    }

    private void Update()
    {
        //if (enablePull && target != null)
        //{
        //    // Calculate the direction from the current position to the target position.
        //    Vector2 pullDirection = (pullTarget.position - transform.position).normalized;

        //    // Apply the gentle pull force to the Rigidbody2D.
        //    rb.AddForce(pullDirection * pullStrength * Time.deltaTime, ForceMode2D.Force);
        //}

        if (test)
        {
            Shoot();
            test = false;
        }
        if (Vector2.Dot((pullTarget.position - transform.position).normalized, transform.up)<= 0)
        {
            // Calculate the direction from the current position to the target position.
            Vector2 pullDirection = (pullTarget.position - transform.position).normalized;

            // Apply the gentle pull force to the Rigidbody2D.
            rb.AddForce(pullDirection * pullStrength * Time.deltaTime, ForceMode2D.Force);
        }

        if (Vector2.Distance(transform.position, world.transform.position) > distanceBeforeTarget)
        {
            ShootTarget();
            
        }
        //if (Vector2.Distance(transform.position, world.transform.position)< 40f)
        //{
        //    canshoot = true
        //}
    }

    private void Shoot()
    {
        rb.velocity = Vector2.zero;
        if (target != null)
        {
            // Calculate the direction from the current position to the target position.
            Vector2 shootDirection = (target.position - transform.position).normalized;

            // Apply the impulse force to the Rigidbody2D.
            rb.AddForce(shootDirection * shootForce, ForceMode2D.Impulse);
            rb.velocity = Mathf.Clamp(rb.velocity.magnitude, 0, maxVelocity) * rb.velocity.normalized;
        }
        else
        {
            Debug.LogWarning("No target assigned for the projectile.");
        }
    }
    private void ShootTarget()
    {
        rb.velocity = Vector2.zero;
        if (target != null)
        {
            // Calculate the direction from the current position to the target position.
            Vector2 shootDirection = (pullTarget.position - transform.position).normalized;

            // Apply the impulse force to the Rigidbody2D.
            rb.AddForce(shootDirection * shootForce, ForceMode2D.Impulse);
            rb.velocity = Mathf.Clamp(rb.velocity.magnitude, 0, maxVelocity) * rb.velocity.normalized;
        }
        else
        {
            Debug.LogWarning("No target assigned for the projectile.");
        }
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        Shoot();
    }


}
