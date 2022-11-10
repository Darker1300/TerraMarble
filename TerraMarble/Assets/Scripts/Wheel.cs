using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;


public class Wheel : MonoBehaviour
{
    [Header("Config")]
    public float speedFactor = 20f;
    public float minSpeed = 0.1f;
    public float maxSpeed = 100f;
    public float idleSpeed = -.2f;
    public float decelerationSpeed = 4f;

    public UnityEvent<bool> GrabEvent = new UnityEvent<bool>();

    [Header("Debug References")]
    public Transform grabber;
    public Transform regionsParent;
    public new Rigidbody2D rigidbody2D;
    public CircleCollider2D wheelCollider2D;

    [Header("Debug Physics")]
    public bool grabbing = false;
    public float velocity = 0f;
    public float grabCurrentAngle = 0f;
    public float grabTargetAngle = 0f;
    //private float decelSmoothCounter = 0f;
    //public float decelerationTime = 0.3f;

    [Header("Debug Data")]
    public Region[] regions;

    private void Start()
    {
        rigidbody2D ??= GetComponent<Rigidbody2D>();
        wheelCollider2D ??= GetComponent<CircleCollider2D>();
        grabber ??= GameObject.Find("Grabber").transform;

        InputManager.LeftDragEvent += OnLeftDrag;
        InputManager.LeftDragVectorEvent += OnLeftDragUpdate;

        velocity = idleSpeed;
    }

    private void Update()
    {
        grabCurrentAngle = GetAngleFromPoint(grabber.position);

        // Calc Velocity
        if (grabbing)
        {
            velocity = Mathf.DeltaAngle(grabCurrentAngle, grabTargetAngle);
        }
        else
        {
            // Deceleration

            //velocity = Mathf.SmoothDampAngle(
            //    velocity,
            //    idleSpeed,
            //    ref decelSmoothCounter,
            //    decelerationTime
            //);

            velocity = Mathf.MoveTowards(
                velocity,
                idleSpeed,
                Mathf.Max(Mathf.Abs(velocity), minSpeed) * decelerationSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        float goalAngle = transform.eulerAngles.z + velocity;
        Quaternion goalRotation = Quaternion.Euler(new Vector3(0, 0, goalAngle));

        float speed = Mathf.Clamp(Mathf.Abs(velocity) * speedFactor, minSpeed, maxSpeed);
        Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, goalRotation, speed * Time.fixedDeltaTime);

        // Apply movement
        rigidbody2D.MoveRotation(newRotation);
    }

    private void OnLeftDrag(bool state)
    {
        grabbing = state;
        if (grabbing)
        {
            //Debug.Log(InputManager.DragLeftStartScreenPos);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
            grabber.position = wheelCollider2D.ClosestPoint(worldPoint);
        }

        GrabEvent.Invoke(state);
    }

    private void OnLeftDragUpdate(Vector2 currentLocalPosition, Vector2 mouseDelta)
    {
        Vector3 dragStartWorldPoint = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
        Vector3 dragCurrentWorldPoint = (Vector3)currentLocalPosition + dragStartWorldPoint;
        grabTargetAngle = GetAngleFromPoint(dragCurrentWorldPoint);



        //dragTarget = transform.InverseTransformPoint(dragCurrentWorldPoint);

        // Debug.Log(mouseDelta);
        //Camera gameCamera = Camera.main;
        //velocity = Mathf.Clamp(currentLocalPosition.magnitude * dragFactor, idleVelocity, maxVelocity);

        //Vector3 grabberToMouseVector = dragCurrentWorldPoint - grabber.position;
    }

    private void UpdateVelocity()
    {

    }

    /// <summary>
    /// World space Point
    /// </summary>
    /// <param name="_point"></param>
    /// <returns>Degrees</returns>
    public float GetAngleFromPoint(Vector2 _point)
        => GetAngleFromDirection((_point - (Vector2)transform.position).normalized);

    //{
    //    return Mathf.Atan2(
    //        _point.y - transform.position.y,
    //        _point.x - transform.position.x)
    //            * Mathf.Rad2Deg;
    //}

    /// <summary>
    /// </summary>
    /// <param name="_direction"></param>
    /// <returns>Degrees</returns>
    public float GetAngleFromDirection(Vector2 _direction)
    {
        return Mathf.Atan2(
                   _direction.y,
                   _direction.x)
               * Mathf.Rad2Deg;
    }
    public Vector2 GetDirectionFromAngle(float angle)
    {
        return new Vector2(
            Mathf.Sin(Mathf.Deg2Rad * angle),
            Mathf.Cos(Mathf.Deg2Rad * angle));
    }
}
