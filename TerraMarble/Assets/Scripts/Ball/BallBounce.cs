using UnityEngine;
using UnityEngine.Events;

public class BallBounce : MonoBehaviour
{
    [SerializeField] private bool canBounce = true;

    private UpdateGravity gravityDir;

    private Rigidbody2D rb;

    //how much influence of magnitude carries over
    [SerializeField] [Range(0, 1)] private float slideFactor = 1f;

    [SerializeField] private float slideMax = 50f;
    [SerializeField] private float slideMin = 0f;

    private void Start()
    {
        rb ??= GetComponent<Rigidbody2D>();
        gravityDir ??= GetComponent<UpdateGravity>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (canBounce)
        {
            Bounce(collision.contacts[0].normal);
        }
        else
        {
            Vector2 surfaceDirection = Vector3.Cross(collision.contacts[0].normal, Vector3.forward);
            var dot = Vector2.Dot(rb.velocity, surfaceDirection);

            //Debug.Log("dot" + dot);

            //if above zero facing relativly same direction
            var magnitude = Mathf.Clamp(rb.velocity.magnitude * slideFactor, slideMin, slideMax);
            var velocity = surfaceDirection * magnitude;
            if (dot > 0)
                rb.velocity = velocity;
            else if (dot < 0)
                rb.velocity = -velocity;
            else Bounce(collision.contacts[0].normal);
        }
    }

    public void Bounce(Vector2 normal)
    {
        var project = Vector2.ClampMagnitude(rb.velocity.normalized - 20 * (Vector2.Dot(rb.velocity, normal) * normal),
            gravityDir.maxGravity);
        rb.AddForce(project * Time.fixedDeltaTime);
    }
}