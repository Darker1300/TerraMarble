using System.Collections;
using System.Collections.Generic;
using MathUtility;
using UnityEngine;

public class EntityBody : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private bool doGravitation = true;

    private Rigidbody2D rb;
    private Wheel wheel;
    private Vector2 initialVelocity;
    [SerializeField] private float debugLineSize = 1f;
    [SerializeField] private Color debugLineColor = Color.blue;

    void Awake()
    {
        wheel = FindObjectOfType<Wheel>();
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = initialVelocity;
    }

    void FixedUpdate()
    {
        if (!doGravitation) return;

        // get current magnitude
        var magnitude = rb.velocity.magnitude;

        // get vector center <- obj
        var gravityVector = (Vector2)wheel.transform.position - rb.position;

        // check whether left or right of target
        var left = Vector2.SignedAngle(rb.velocity, gravityVector) > 0;

        // get new vector which is 90° on gravityDirection 
        // and world Z (since 2D game)
        // normalize so it has magnitude = 1
        var newDirection = Vector3.Cross(gravityVector, Vector3.forward).normalized;

        // invert the newDirection in case user is touching right of movement direction
        if (!left) newDirection *= -1;

        // set new direction but keep speed(previously stored magnitude)
        rb.velocity = newDirection * magnitude;
    }

    private void OnDrawGizmosSelected()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        Vector2 pos = transform.position;
        Vector2 forceV = rb.velocity * debugLineSize;
        Gizmos.color = debugLineColor;
        Gizmos.DrawLine(pos, pos + forceV);
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    ContactPoint2D contact = collision.GetContact(0);
    //    Vector2 newDir = Vector2.Reflect(rb.velocity.normalized, contact.normal);

    //    Vector2 bumpVector = newDir * contact.normalImpulse;
    //    rb.velocity += bumpVector;
    //}
}
