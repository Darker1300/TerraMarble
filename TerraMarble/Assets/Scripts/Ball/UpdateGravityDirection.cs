using UnityEngine;
using UnityEngine.Events;

public class UpdateGravityDirection : MonoBehaviour
{
    [SerializeField] private bool canBounce = true;

    public Vector3 direction;
    public Vector3 ExtraForceVector;
    [HideInInspector] public UnityEvent<Collision2D> HitSurface;

    public float maxGravDist = 4.0f;
    public float maxGravity = 35.0f;

    // Start is called before the first frame update
    [SerializeField]
    [Range(0,1)]
    private float MaxOrbitLeftInfluence;

    private Transform planetCenter;

    private Rigidbody2D rb;

    //how much influence of magnitude carries over
    [SerializeField] [Range(0, 1)] private float SlideFactor = 1;

    [SerializeField] private float slideMax = 50f;
    [SerializeField] private float slideMin = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        planetCenter = FindObjectOfType<Wheel>().transform;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateDirectionToCentre();


        if (ExtraForceVector != Vector3.zero)
        {
            //rb.velocity = Vector2.zero;
            Vector3 force = ExtraForceVector.normalized * 10000;
            rb.AddRelativeForce(force * Time.fixedDeltaTime);
        }
        else
        {
            float dist = direction.magnitude;
            if (dist <= maxGravDist)
            {
                //velocity to planet direction , how much is orbiting 
                float dot = Vector2.Dot(rb.velocity, direction);


                rb.AddForce(direction.normalized * (1.0f - dist / maxGravDist) * maxGravity);

                





                rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxGravity);
            }
        }
    }

    public void UpdateDirectionToCentre()
    {
        direction = planetCenter.transform.position - transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ExtraForceVector = Vector2.zero;
        //Vector2 project = -2 * (Vector2.Dot(rb.velocity, collision.contacts[0].normal) * collision.contacts[0].normal + rb.velocity);
        //rb.velocity = Vector2.zero;
        //rb.velocity = project;
        HitSurface.Invoke(collision);

        if (canBounce)
        {
            Bounce(collision.contacts[0].normal);
        }
        else
        {
            Vector2 surfaceDirection = Vector3.Cross(collision.contacts[0].normal, Vector3.forward);
            float dot = Vector2.Dot(rb.velocity, surfaceDirection);

            Debug.Log("dot" + dot);

            //if above zero facing relativly same direction
            float magnitude = Mathf.Clamp(rb.velocity.magnitude * SlideFactor, slideMin, slideMax);
            Vector2 velocity = surfaceDirection * magnitude;
            if (dot > 0)
                rb.velocity = velocity;
            else if (dot < 0)
                rb.velocity = -velocity;
            else Bounce(collision.contacts[0].normal);


            // we turn bounce and 
            //Bounce(collision.contacts[0].normal);
        }


        // Vector2 project = Vector2.ClampMagnitude( rb.velocity.normalized - 20 * (Vector2.Dot(rb.velocity , collision.contacts[0].normal) * collision.contacts[0].normal), maxGravity);
        //project =  project;


        //rb.velocity.Set(0,0);
        //Debug.Log("coll" + dot);
        //rb.AddForce(project);
        //rb.AddRelativeForce(project*10);
    }

    public void Bounce(Vector2 normal)
    {
        // ExtraForceVector = Vector2.zero;
        var project = Vector2.ClampMagnitude(rb.velocity.normalized - 20 * (Vector2.Dot(rb.velocity, normal) * normal),
            maxGravity);
        rb.AddForce(project * Time.fixedDeltaTime);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
    }
}