using MathUtility;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Events;


public class Wheel : MonoBehaviour
{
    [Header("Config")] public float speedFactor = 20f;
    public float minSpeed = 0.1f;
    public float maxSpeed = 100f;
    public float idleSpeed = -.2f;
    public float decelerationSpeed = 0f;
    public float minGrabDistance = 0.1f;
    public float reverseSpeedUpFactor = 10f;

    public bool invertGrabVelocity = true;

    [Foldout("Show Events")] public UnityEvent<bool> GrabEvent = new();
    //[Foldout("Show Events")] public UnityEvent<float> FixedRotationEvent = new UnityEvent<float>();

    [Header("Debug References")] [Foldout("Show Debug Fields")]
    public Transform grabber;

    [Foldout("Show Debug Fields")] public new Rigidbody2D rigidbody2D;
    [Foldout("Show Debug Fields")] public CircleCollider2D wheelCollider2D;
    [Foldout("Show Debug Fields")] public WheelRegionsManager regions;

    [Header("Debug Physics")] [Foldout("Show Debug Fields")] [SerializeField]
    private bool grabbing = false;

    [Foldout("Show Debug Fields")] [SerializeField]
    private float velocity = 0f;

    [Foldout("Show Debug Fields")] [SerializeField]
    private float grabCurrentAngle = 0f;

    [Foldout("Show Debug Fields")] [SerializeField]
    private float grabTargetAngle = 0f;

    private void Start()
    {
        rigidbody2D ??= GetComponent<Rigidbody2D>();
        wheelCollider2D ??= GetComponent<CircleCollider2D>();
        grabber ??= GameObject.Find("Grabber").transform;
        regions ??= GetComponent<WheelRegionsManager>();

        InputManager.LeftDragEvent += OnLeftDrag;
        InputManager.LeftDragVectorEvent += OnLeftDragUpdate;
        InputManager.TapLeftEvent += OnTapLeft;

        velocity = idleSpeed;
    }

    private void OnTapLeft(object sender, EventArgs args)
    {
        // ReverseSpin();
    }

    private void Update()
    {
        grabCurrentAngle = GetAngleFromPoint(grabber.position);

        // Calc Velocity
        if (grabbing)
            velocity = Mathf.DeltaAngle(grabCurrentAngle, grabTargetAngle);
        else
            // Deceleration
            velocity = Mathf.MoveTowards(
                velocity,
                idleSpeed,
                Mathf.Max(Mathf.Abs(velocity), minSpeed) * decelerationSpeed * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        var goalAngle = transform.eulerAngles.z + velocity;
        var goalRotation = Quaternion.Euler(new Vector3(0, 0, goalAngle));

        var speed = Mathf.Clamp(Mathf.Abs(velocity) * speedFactor, minSpeed, maxSpeed);
        var fixedSpeed = speed * Time.fixedDeltaTime;
        var newRotation = Quaternion.RotateTowards(transform.rotation, goalRotation, fixedSpeed);

        // Apply movement
        rigidbody2D.MoveRotation(newRotation);

        //FixedRotationEvent.Invoke(fixedSpeed);
    }

    private void OnLeftDrag(bool state)
    {
        grabbing = state;
        if (grabbing)
        {
            //Debug.Log(InputManager.DragLeftStartScreenPos);
            var worldPoint = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
            grabber.position = wheelCollider2D.ClosestPoint(worldPoint);
        }

        GrabEvent.Invoke(state);
    }

    private void OnLeftDragUpdate(Vector2 currentDragOffset, Vector2 mouseDelta)
    {
        //Debug.Log("DRAG: |" + currentDragOffset.ToString() + " | " + mouseDelta.ToString());
        Vector2 dragStartWorldPoint = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
        Vector2 dragCurrentWorldPoint;

        if (invertGrabVelocity)
        {
            var normal = ((Vector2) transform.position - dragStartWorldPoint).normalized.RotatedByDegree(90f);
            dragCurrentWorldPoint = Vector2.Reflect(currentDragOffset, normal);
        }
        else
        {
            dragCurrentWorldPoint = currentDragOffset;
        }

        if (Vector2.Distance(dragStartWorldPoint, dragCurrentWorldPoint) < minGrabDistance) return;

        var dragInvertedCurrentWorldPoint = dragStartWorldPoint + dragCurrentWorldPoint;
        grabTargetAngle = GetAngleFromPoint(dragInvertedCurrentWorldPoint);
    }

    /// <summary>
    /// World space Point
    /// </summary>
    /// <param name="_worldPoint"></param>
    /// <returns>Degrees</returns>
    public float GetAngleFromPoint(Vector2 _worldPoint)
    {
        return MathU.Vector2ToDegree((_worldPoint - (Vector2) transform.position).normalized);
    }


    [Button]
    public void ReverseSpin()
    {
        velocity = Mathf.Clamp(-velocity * reverseSpeedUpFactor, -maxSpeed, maxSpeed);
    }
}