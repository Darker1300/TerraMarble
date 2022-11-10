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

    [SerializeField] private bool showEvents = false;
    [ShowIf("showEvents")] public UnityEvent<bool> GrabEvent = new UnityEvent<bool>();

    [SerializeField] private bool showDebugFields = false;
    [Header("Debug References")]
    [ShowIf("showDebugFields")] public Transform grabber;
    [ShowIf("showDebugFields")] public Transform regionsParent;
    [ShowIf("showDebugFields")] public new Rigidbody2D rigidbody2D;
    [ShowIf("showDebugFields")] public CircleCollider2D wheelCollider2D;

    [Header("Debug Physics")]
    [ShowIf("showDebugFields")] [SerializeField] private bool grabbing = false;
    [ShowIf("showDebugFields")] [SerializeField] private float velocity = 0f;
    [ShowIf("showDebugFields")] [SerializeField] private float grabCurrentAngle = 0f;
    [ShowIf("showDebugFields")] [SerializeField] private float grabTargetAngle = 0f;

    [Header("Debug Data")]
    [ShowIf("showDebugFields")] public Region[] regions;

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
    }

    /// <summary>
    /// World space Point
    /// </summary>
    /// <param name="_point"></param>
    /// <returns>Degrees</returns>
    public float GetAngleFromPoint(Vector2 _point)
        => GetAngleFromDirection((_point - (Vector2)transform.position).normalized);

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
