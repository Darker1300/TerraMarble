using System;
using UnityEngine;
using UnityEngine.Events;

public class BallBounce : MonoBehaviour
{
    private UpdateGravity updateGravity;
    private Rigidbody2D rb;
    private Wheel wheel;
    [Header("Bounce")] [SerializeField] private bool canBounce = true;

    [SerializeField] private float minBounceForce = 32f;
    [SerializeField] [Range(0f, 1f)] private float bounceFactor = 0.95f;

    //how much influence of magnitude carries over
    [Header("Slide")]
    [SerializeField]
    [Range(0, 1)]
    private float slideFactor = 1f;

    [SerializeField] private float slideMax = 50f;
    [SerializeField] private float slideMin = 0f;

    private bool isHit = false;
    [SerializeField]
    [Range(-1, 1)]
    private float bounceRange = 0.0f;

    private void Start()
    {
        rb ??= GetComponent<Rigidbody2D>();
        wheel = FindObjectOfType<Wheel>();
        
        updateGravity ??= GetComponent<UpdateGravity>();
    }

    private void FixedUpdate()
    {
        if (isHit) isHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit) return;
        Slide(collision.contacts[0].normal);
        //CheckDotProduct(collision.contacts[0].normal, rb.velocity.normalized, bounceRange)
        //Debug.Log("slideDot" + (Vector2.Dot(collision.contacts[0].normal, rb.velocity.normalized)));
        //                  Downwards against wind 
        //if ((canBounce && CheckDirectionProduct(updateGravity.wheelDir, rb.velocity.normalized, 0.0f) && !MovingWithWind(transform.position, rb.velocity)) ||
        //   (canBounce && !CheckDirectionProduct(updateGravity.wheelDir, rb.velocity.normalized, 0.0f) && !MovingWithWind(transform.position, rb.velocity)))
        //{ 
        //    Bounce(collision.contacts[0].normal);
        //}
        //else if ((canBounce && !CheckDirectionProduct(updateGravity.wheelDir, rb.velocity.normalized, 0.0f) && MovingWithWind(transform.position, rb.velocity)) ||
        //   (canBounce && CheckDirectionProduct(updateGravity.wheelDir, rb.velocity.normalized, 0.0f) && MovingWithWind(transform.position, rb.velocity)))
        //{
        //    Slide(collision.contacts[0].normal);
        //}
        isHit = true;
    }
    public bool CheckDirectionProduct(Vector2 lft,Vector2 rght,float range)
    {
        //if 
        if (Vector2.Dot(lft, rght) > range )
        {
            Debug.Log("Bounce");
            return true;
        }
        else
            return false;

    }

    //private bool MovingWithWind(Vector3 pos,Vector2 vel)
    //{
    //    return wheel.IsMovingWithWheel(pos, vel);
    //}

    public void Bounce(Vector2 surfaceNormal)
    {
        Vector2 initialVel = rb.velocity;
        Vector2 initialDir = initialVel.normalized;
        float initialMag = initialVel.magnitude;

        Vector2 surfaceReflect = Vector2.Reflect(initialDir, surfaceNormal);

        // // attempts to keep momentum
        //Vector2 wheelReflect = Vector2.Reflect(initialDir, -updateGravity.wheelDir.normalized);
        //Vector2 followThru = (surfaceReflect * bounceFactor
        //                      + wheelReflect * (1f - bounceFactor)).normalized;

        Vector2 bounceClamped = surfaceReflect * Mathf.Max(minBounceForce, initialMag);

        rb.velocity = bounceClamped;

        // // Old Bounce code
        //var project = Vector2.ClampMagnitude(rb.velocity.normalized -20 * (Vector2.Dot(rb.velocity, normal) * normal),
        //     gravityDir.maxGravity);
        // rb.AddForce(project * Time.fixedDeltaTime);
    }

    public void Slide(Vector2 surfaceNormal)
    {
        Vector2 surfaceDirection = Vector3.Cross(surfaceNormal, Vector3.forward);
        float dotA = Vector2.Dot(rb.velocity, surfaceDirection);
        float dotB = Vector2.Dot(rb.velocity, -surfaceDirection);
        surfaceDirection = dotA > dotB ? surfaceDirection : -surfaceDirection;
        //simply which direction is closest to wind
        //dot product of vel and (-/surface direction )
        //whichever is closer apply for that way
        //slide in that direction
        //
        //Debug.Log("dot" + dot);

        //if above zero facing relativly same direction
        float magnitude = Mathf.Clamp(rb.velocity.magnitude * slideFactor, slideMin, slideMax);
        Vector2 velocity = surfaceDirection * magnitude;
        if (dotA > 0)
            rb.AddForce(velocity);
        else if (dotA < 0)
            rb.AddForce( -velocity);
        //else Bounce(surfaceNormal);
    }
}