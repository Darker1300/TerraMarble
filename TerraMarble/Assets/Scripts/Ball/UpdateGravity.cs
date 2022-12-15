using UnityEngine;
using UnityEngine.Events;

public class UpdateGravity : MonoBehaviour
{
    public float maxGravityDist = 132.0f;
    public float maxGravity = 35.0f;

    private Transform wheelCenter;

    private Rigidbody2D rb;

    [SerializeField] private float stompPower = 38f;
    public Vector3 stompForceVector;
    public Vector3 wheelDir;

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
            // TODO make force weaker the further away from wheel, eventually pulling back to wheel
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        stompForceVector = Vector3.zero;
    }
}