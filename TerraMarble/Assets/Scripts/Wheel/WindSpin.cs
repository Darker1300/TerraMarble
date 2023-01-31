using MathUtility;
using UnityEngine;

public class WindSpin : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float sensitivityFactor = 2.5f;
    [SerializeField] private float minSpeed = 1.5f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private float forceFactor = 2f;

    [SerializeField] private float minGrabDelta = 0.1f;

    [Header("Data")]
    [SerializeField] private Transform grabber;
    [SerializeField] private bool isGrabbing = false;
    [SerializeField] private float grabStartAngle = 0f;
    [SerializeField] private float grabTargetAngle = 0f;
    [SerializeField] private float windVelocity = 0f;

    public float WindVelocity => windVelocity;
    public float ForceFactor => forceFactor;

    void Awake()
    {
        InputManager.LeftDragEvent += OnLeftDragToggle;
        InputManager.LeftDragVectorEvent += OnLeftDragUpdate;

        grabber = new GameObject("Grabber").transform;
        grabber.SetParent(transform, false);
    }

    void OnDestroy()
    {
        InputManager.LeftDragEvent -= OnLeftDragToggle;
        InputManager.LeftDragVectorEvent -= OnLeftDragUpdate;
    }

    void OnLeftDragToggle(bool state)
    {
        isGrabbing = state;
        if (isGrabbing)
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
            grabber.position = worldPoint;

            // Set Start grab angle
            grabStartAngle = grabTargetAngle = grabTargetAngle
                = GetAngleFromPoint(grabber.position);
        }
    }

    void OnLeftDragUpdate(Vector2 currentDragPoint, Vector2 mouseDelta)
    {
        float dragCurrentAngle = GetAngleFromPoint(currentDragPoint);

        // Min Drag
        if (Mathf.Abs(
            Mathf.DeltaAngle(grabStartAngle, dragCurrentAngle))
            < minGrabDelta) return;

        // Set Target grab angle
        grabTargetAngle = dragCurrentAngle;
    }

    void Update()
    {
        windVelocity = Mathf.DeltaAngle(grabStartAngle, grabTargetAngle)
                       * sensitivityFactor;
        // Max speed
        windVelocity = Mathf.Clamp(windVelocity, -maxSpeed, maxSpeed);

        // Min speed
        //if (Mathf.Abs(windVelocity) < minSpeed)
        //    windVelocity = minSpeed * Mathf.Sign(windVelocity);

        //windVelocity = Mathf.Clamp(Mathf.Abs(windVelocity) * SensitivityFactor, minSpeed, maxSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        if (!grabber) return;

        Vector2 EndGrabPoint = transform.position.Towards(grabber.position);
        EndGrabPoint.RotatedByDegree(
            Mathf.DeltaAngle(grabStartAngle, grabTargetAngle));
        EndGrabPoint += (Vector2) transform.position;

        Gizmos.DrawLine(grabber.position, EndGrabPoint);
    }

    /// <param name="_worldPoint">World space</param>
    /// <returns>Degrees</returns>
    public float GetAngleFromPoint(Vector2 _worldPoint)
    {
        return MathU.Vector2ToDegree((_worldPoint - (Vector2)transform.position).normalized);
    }



    //   void Awake()
    //   {
    //       grabber ??= GameObject.Find("Grabber").transform;
    //  
    //       velocity = idleSpeed;
    //  
    //       InputManager.LeftDragEvent += OnLeftDrag;
    //       InputManager.LeftDragVectorEvent += OnLeftDragUpdate;
    //       InputManager.TapLeftEvent += OnTapLeft;
    //   }
    //  
    //   void FixedUpdate()
    //   {
    //       if (!windActive) return;
    //  
    //  
    //       var goalAngle = windPlane.eulerAngles.z + velocity;
    //       var goalRotation = Quaternion.Euler(new Vector3(0, 0, goalAngle));
    //  
    //       var speed = Mathf.Clamp(Mathf.Abs(velocity) * speedFactor, minSpeed, maxSpeed);
    //       var fixedSpeed = speed * Time.fixedDeltaTime;
    //       // float a = MathU.MoveTowardsRange();
    //       var newRotation = Quaternion.RotateTowards(windPlane.rotation, goalRotation, fixedSpeed);
    //  
    //       // Apply movement
    //       // windSpin.windSpeed = newRotation.;
    //  
    //  
    //       Vector3 windCenter = transform.position;
    //  
    //       foreach (Transform target in transform)
    //       {
    //           Rigidbody2D rb2D = target.GetComponent<Rigidbody2D>();
    //           if (rb2D != null)
    //               rb2D.MoveRotateAround(windCenter, windSpeed);
    //           else
    //               target.RotateAround(windCenter, Vector3.forward, windSpeed);
    //       }
    //   }
    //  
    //  
    //   private void OnTapLeft(object sender, EventArgs args)
    //   {
    //       // ReverseSpin();
    //   }
    //  
    //   private void OnLeftDragStart(bool state)
    //   {
    //       isGrabbing = state;
    //       if (isGrabbing)
    //       {
    //           //Debug.Log(InputManager.DragLeftStartScreenPos);
    //           var worldPoint = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
    //           grabber.position = wheelCollider2D.ClosestPoint(worldPoint);
    //       }
    //  
    //       //GrabEvent.Invoke(state);
    //   }
    //  
    //  
    //   private void OnLeftDragUpdate(Vector2 currentDragOffset, Vector2 mouseDelta)
    //   {
    //       //Debug.Log("DRAG: |" + currentDragOffset.ToString() + " | " + mouseDelta.ToString());
    //       Vector2 dragStartWorldPoint = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
    //       Vector2 dragCurrentWorldPoint;
    //  
    //       if (invertGrabVelocity)
    //       {
    //           var normal = ((Vector2)windPlane.position - dragStartWorldPoint).normalized.RotatedByDegree(90f);
    //           dragCurrentWorldPoint = Vector2.Reflect(currentDragOffset, normal);
    //       }
    //       else
    //       {
    //           dragCurrentWorldPoint = currentDragOffset;
    //       }
    //  
    //       if (Vector2.Distance(dragStartWorldPoint, dragCurrentWorldPoint) < minDragDistance) return;
    //  
    //       var dragInvertedCurrentWorldPoint = dragStartWorldPoint + dragCurrentWorldPoint;
    //       grabTargetAngle = GetAngleFromPoint(dragInvertedCurrentWorldPoint);
    //   }
    //  
    //   private void UpdateVelocity()
    //   {
    //       grabCurrentAngle = GetAngleFromPoint(grabber.position);
    //  
    //       // Calc Velocity
    //       if (isGrabbing)
    //           velocity = Mathf.DeltaAngle(grabCurrentAngle, grabTargetAngle);
    //       else
    //           // Deceleration
    //           velocity = Mathf.MoveTowards(
    //               velocity,
    //               idleSpeed,
    //               Mathf.Max(Mathf.Abs(velocity), minSpeed) * decelerationSpeed * Time.deltaTime);
    //   }





}
