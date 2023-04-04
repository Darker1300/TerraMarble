using UnityEngine;
using UnityEngine.Events;
using UnityUtility;
using System;

public class BallClimb : MonoBehaviour
{
    private seadWeirdGravity gravityDir;
    private Rigidbody2D rb;

    [SerializeField] private float slideMax = 50f;
    [SerializeField] private float slideMin = 3f;
    [SerializeField] private float dotMin = 0.0f;
    private Vector3 startPos;
    [SerializeField] private float maxBounceForce = 15;
    [SerializeField] private float minBounceForce = 0;

    [SerializeField] private bool isHit = false;


    private void Start()
    {
       // InputManager.LeftAlternateEvent += RespawnBall;
        //InputManager.RightAlternateEvent += RespawnBall;

        startPos = transform.position;
        rb ??= GetComponent<Rigidbody2D>();
        gravityDir ??= GetComponent<seadWeirdGravity>();
    }

    private void Update()
    {
        isHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //if (collision.collider.gameObject.layer != LayerMask.NameToLayer("Wheel")
        //    && collision.collider.gameObject.layer != LayerMask.NameToLayer("Surface")) return;
        //SlideTrue(collision.contacts[0].normal);
        if (!isHit)
        {
            BounceTwo(collision.contacts[0].normal);
            isHit = true;
        }
        //UphillBoulderRoll(collision.contacts[0].normal);
        //else
        //    if (collision.collider.gameObject.CompareTag("Tree"))
        //    {
        //    }
        //    else Bounce(collision.contacts[0].normal);




    }
    public void UphillBoulderRoll(Vector2 NormalDirection)
    {
        //get right of of tree surface normal
        Vector2 surfaceDirection = Vector3.Cross(NormalDirection, Vector3.forward);
        //dot product of planet normal and the tree surface right
        var dot = Vector2.Dot(gravityDir.wheelDir.normalized, surfaceDirection);
        //info
        //what direction it is moving , is it moving with the slope or not 







        if (dot > dotMin)
            surfaceDirection *= -1f;
        Debug.Log("dott " + dot);
        var clampedVelocity = Mathf.Clamp(rb.velocity.magnitude, slideMin, slideMax);
        rb.velocity = surfaceDirection * clampedVelocity;

    }
    public void SlideTrue(Vector2 NormalDirection)
    {
        //get velocity direction 

        Vector2 rbUp = Vector3.Cross(rb.velocity, Vector3.forward);
        var dot = Vector2.Dot(NormalDirection, rbUp);







        Debug.Log("dott " + dot);


        //ball up is close to surface up meaning surface meaning ball is moving is same direction the floor is going
        if (dot > dotMin)
        {

            var clampedVelocity = Mathf.Clamp(rb.velocity.magnitude, slideMin, slideMax);

            //which direction parallel to surface should we apply force to
            //right
            var Right = Vector3.Cross(NormalDirection, Vector3.forward);
            //is ball moving right or left (if above 0 moving right)
            if (Vector2.Dot(Right, rb.velocity) > 0)
            {
                rb.velocity = Right * clampedVelocity;
            }
            else
                rb.velocity = -Right * clampedVelocity;



        }


    }
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

        Vector2 bounceClamped = surfaceReflect * Mathf.Clamp(initialMag, minBounceForce, maxBounceForce);

        rb.velocity = bounceClamped;

        // // Old Bounce code
        //var project = Vector2.ClampMagnitude(rb.velocity.normalized -20 * (Vector2.Dot(rb.velocity, normal) * normal),
        //     gravityDir.maxGravity);
        // rb.AddForce(project * Time.fixedDeltaTime);
    }
    public void BounceTwo(Vector2 surfaceNormal)
    {
        Vector2 initialVel = rb.velocity;
        Vector2 initialDir = initialVel.normalized;

        Vector2 surfaceReflect = Vector2.Reflect(initialDir, surfaceNormal);

        // apply min magnitude
        float initialMag = initialVel.magnitude;
        initialMag = MathF.Max(initialMag, minBounceForce);
        surfaceReflect = surfaceReflect * initialMag;

        // // attempts to keep momentum
        //Vector2 wheelReflect = Vector2.Reflect(initialDir, -updateGravity.wheelDir.normalized);
        //Vector2 followThru = (surfaceReflect * bounceFactor
        //                      + wheelReflect * (1f - bounceFactor)).normalized;
        

        rb.velocity = surfaceReflect;

        // // Old Bounce code
        //var project = Vector2.ClampMagnitude(rb.velocity.normalized -20 * (Vector2.Dot(rb.velocity, normal) * normal),
        //     gravityDir.maxGravity);
        // rb.AddForce(project * Time.fixedDeltaTime);
    }


    public void RespawnBall(object sender, EventArgs o)
    {

        transform.position = startPos;


    }
}