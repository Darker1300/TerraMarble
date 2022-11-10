using UnityEngine;


public class Wheel : MonoBehaviour
{
    [Header("Config")]
    public float minSpeed = 1f;
    public float maxSpeed = 100f;
    public float idleSpeed = 5f; 
    public float decelerationSpeed = 5f;
    public Vector3 dragTarget = Vector3.zero;

    [Header("Debug")]
    public Transform grabber;
    public Transform regionsParent;
    public new Rigidbody2D rigidbody2D;
    public bool grabbing = false;
    public float velocity = 0f;
    public float grabCurrentAngle = 0f;
    public float grabTargetAngle = 0f;
    public Region[] regions;



    // public float direction;
    //public float velocity = 1f;
    //public float targetAngle = 0f;

    //public Vector3 dragTargetPrevious = Vector3.zero;

    //public float frictionFactor = 0.99f;
    //public float dragFactor = 0.1f;

    private void Start()
    {
        rigidbody2D ??= GetComponent<Rigidbody2D>();
        grabber ??= GameObject.Find("Grabber").transform;

        InputManager.LeftDragEvent += OnLeftDrag;
        InputManager.LeftDragVectorEvent += OnLeftDragUpdate;

        velocity = minSpeed;
    }

    private void Update()
    {
        grabCurrentAngle = GetAngleFromPoint(grabber.position);

        // Calc Velocity
        if (grabbing)
        {
            velocity = Mathf.DeltaAngle(grabCurrentAngle, grabTargetAngle);
        }




        // float currentAngle = transform.eulerAngles.z;
        //  // Direction

            //  if (grabbing)
            //  {
            //      targetAngle = Mathf.Atan2(
            //                        dragTarget.y - transform.position.y,
            //                        dragTarget.x - transform.position.x)
            //                    * Mathf.Rad2Deg;
            //  }
            //  else
            //  {
            //      targetAngle = currentAngle + Mathf.DeltaAngle(currentAngle, currentAngle - 10f);
            //  }

            //  float deltaAngle = Mathf.DeltaAngle(currentAngle, targetAngle);

            //  velocity = Mathf.Clamp(deltaAngle, -maxVelocity, maxVelocity);
    }

    private void FixedUpdate()
    {
        //  // Direction
        //  float angle = 0f;

        //  if (grabbing)
        //      angle = Mathf.Atan2(
        //                  dragTarget.y - transform.position.y,
        //                  dragTarget.x - transform.position.x)
        //              * Mathf.Rad2Deg;
        //  else angle = transform.eulerAngles.z - 10f;

        //  Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        //  var newRot = Quaternion.RotateTowards(
        //      transform.rotation, 
        //      targetRotation, 
        //      velocity * Time.fixedDeltaTime);

        //  // Apply direction
        //  rigidbody2D.MoveRotation(newRot);

        //  // Friction
        //  if (velocity > idleVelocity)
        //      velocity = Mathf.MoveTowards(
        //          velocity,
        //          idleVelocity,
        //          frictionFactor * Time.fixedDeltaTime);

        float newTargetAngle = transform.eulerAngles.z + velocity;

        Quaternion newTargetRotation = Quaternion.Euler(new Vector3(0, 0, newTargetAngle));
        var newRot = Quaternion.RotateTowards(
            transform.rotation,
            newTargetRotation,
            100f * Time.fixedDeltaTime);

        // Apply direction
        rigidbody2D.MoveRotation(newRot);
    }

    private void OnLeftDrag(bool state)
    {
        grabbing = state;
        if (grabbing)
        {
            //Debug.Log(InputManager.DragLeftStartScreenPos);
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
            grabber.position = rigidbody2D.ClosestPoint(worldPoint);
        }

        //if (grabbing)
        //{
        //    Vector3 dragStartWorldPoint = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
        //    grabStartAngle
        //}
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
