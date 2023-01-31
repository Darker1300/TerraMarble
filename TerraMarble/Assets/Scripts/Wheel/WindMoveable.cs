using MathUtility;
using UnityEngine;

public class WindMoveable : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private WindSpin wind;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        wind = FindObjectOfType<WindSpin>();
    }

    void FixedUpdate()
    {
        if (rb2D != null)
        {
            float rotation = wind.WindVelocity * Time.fixedDeltaTime;
            Vector2 pivotPoint = wind.transform.position;

            Vector2 deltaV = rb2D.position.RotatedAround(pivotPoint, rotation) - rb2D.position;

            rb2D.MoveRotation(Mathf.Repeat(rb2D.rotation + rotation, 360f));

            rb2D.velocity += deltaV * wind.ForceFactor;
        }
        else
            transform.RotateAround(wind.transform.position, Vector3.forward,
                wind.WindVelocity * Time.fixedDeltaTime);
    }
}
