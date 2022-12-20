using UnityEngine;
using UnityEngine.Events;

public class BallBounce : MonoBehaviour
{
    private UpdateGravity updateGravity;
    private Rigidbody2D rb;

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

    private void Start()
    {
        rb ??= GetComponent<Rigidbody2D>();
        updateGravity ??= GetComponent<UpdateGravity>();
    }

    private void FixedUpdate()
    {
        if (isHit) isHit = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit) return;

        if (canBounce)
            Bounce(collision.contacts[0].normal);
        else
            Slide(collision.contacts[0].normal);

        isHit = true;
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

        Vector2 bounceClamped = surfaceReflect * Mathf.Max(minBounceForce, initialMag);

        rb.velocity =  bounceClamped;

        // // Old Bounce code
        //var project = Vector2.ClampMagnitude(rb.velocity.normalized -20 * (Vector2.Dot(rb.velocity, normal) * normal),
        //     gravityDir.maxGravity);
        // rb.AddForce(project * Time.fixedDeltaTime);
    }

    public void Slide(Vector2 surfaceNormal)
    {
        Vector2 surfaceDirection = Vector3.Cross(surfaceNormal, Vector3.forward);
        float dot = Vector2.Dot(rb.velocity, surfaceDirection);

        //Debug.Log("dot" + dot);

        //if above zero facing relativly same direction
        float magnitude = Mathf.Clamp(rb.velocity.magnitude * slideFactor, slideMin, slideMax);
        Vector2 velocity = surfaceDirection * magnitude;
        if (dot > 0)
            rb.velocity = velocity;
        else if (dot < 0)
            rb.velocity = -velocity;
        else Bounce(surfaceNormal);
    }
}