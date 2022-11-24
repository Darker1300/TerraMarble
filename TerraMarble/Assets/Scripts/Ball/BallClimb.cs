using UnityEngine;
using UnityEngine.Events;

public class BallClimb : MonoBehaviour
{
    private UpdateGravity gravityDir;
    private Rigidbody2D rb;

    [HideInInspector] public UnityEvent<Collision2D> HitSurface;

    [SerializeField] private float slideMax = 50f;
    [SerializeField] private float slideMin = 3f;


    private void Start()
    {
        rb ??= GetComponent<Rigidbody2D>();
        gravityDir ??= GetComponent<UpdateGravity>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        gravityDir.stompForceVector = Vector2.zero;

        HitSurface.Invoke(collision);

        Vector2 surfaceDirection = Vector3.Cross(collision.contacts[0].normal, Vector3.forward);

        var dot = Vector2.Dot(gravityDir.wheelDir.normalized, surfaceDirection);

        if (dot > 0.01)
            surfaceDirection *= -1f;

        var clampedVelocity = Mathf.Clamp(rb.velocity.magnitude, slideMin, slideMax);
        rb.velocity = surfaceDirection * clampedVelocity;
    }

    public void Bounce(Vector2 normal)
    {
        var project = Vector2.ClampMagnitude(
            rb.velocity.normalized - 20 * (Vector2.Dot(rb.velocity, normal) * normal),
            gravityDir.maxGravity);
        rb.AddForce(project * Time.fixedDeltaTime);
    }
}