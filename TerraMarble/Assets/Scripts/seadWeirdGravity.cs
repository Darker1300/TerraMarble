using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class seadWeirdGravity : MonoBehaviour
{
    public Vector3 wheelDir;
    public Vector3 stompForceVector;

    public float maxGravityDist = 132.0f;
    public float maxGravity = 35.0f;

    private Transform wheelCenter;

    private Rigidbody2D rb;

    [SerializeField] private float stompPower = 38f;

    private void Start()
    {
        rb ??= GetComponent<Rigidbody2D>();
        wheelCenter ??= FindObjectOfType<Wheel>().transform;
    }

    private void FixedUpdate()
    {
        UpdateDirectionToCentre();

        if (stompForceVector != Vector3.zero) // Apply Stomp
        {
            var force = stompForceVector * stompPower;
            rb.velocity = force * Time.fixedDeltaTime;
        }
        else // Apply Gravity
        {
            var dist = wheelDir.magnitude;
            if (dist <= maxGravityDist)
            {
                //velocity to planet direction , how much is orbiting 
                var dot = Vector2.Dot(rb.velocity, wheelDir);

                rb.AddForce(wheelDir.normalized * (1.0f - dist / maxGravityDist) * maxGravity);

                rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxGravity);
            }
        }
    }

    public void UpdateDirectionToCentre()
    {
        wheelDir = wheelCenter.transform.position - transform.position;
    }
}
