using MathUtility;
using System;
using System.Linq;
using UnityEngine;

public class BallBounce : MonoBehaviour
{
    [Header("Deflect Config")]
    [SerializeField] private float sideDeflectForce = 100f;
    [SerializeField] private float downDeflectForce = 80f;
    [SerializeField] private float maxDragAllowed = 0.2f;
    [SerializeField] private float dragCooldown = 0.2f;
    [SerializeField] private float minVelocity = 0.1f;
    [Header("Slip Config")]
    [SerializeField] private float sideSlipForce = 50f;
    [SerializeField] private float slipUpTolerance = 0.1f;
    //[SerializeField]
    //private AnimationCurve treeStiffnessCurve
    //    = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private Rigidbody2D rigidBody2D = null;
    private PlayerInput playerInput = null;
    private Wheel wheel = null;

    [Header("Data")]
    [SerializeField] private bool isContacting = false;
    [SerializeField] private bool isDragReleased = false;
    [SerializeField] private bool isActive = false;
    [SerializeField] private float dragTimer = 0f;
    private ContactPoint2D deflectContact;

    [SerializeField] VelocityBoost velBooster;
    //[SerializeField] private float treeLength = 2f; // Maximum distance to start reducing velocity.
    //private Vector2 initialVelocity;

    void Start()
    {
        rigidBody2D = rigidBody2D != null ? rigidBody2D
            : GetComponent<Rigidbody2D>();
        playerInput = FindObjectOfType<PlayerInput>();
        wheel = FindObjectOfType<Wheel>();

        InputManager.LeftDragEvent
            += (a) => OnDragToggle(a, -1);
        InputManager.RightDragEvent
            += (a) => OnDragToggle(a, 1);
    }

    private void FixedUpdate()
    {
        UpdateState();

        //initialVelocity = rigidBody2D.velocity;
        //if (isActive && contact.collider)
        //    ApplySuction(contact);

        if (isActive)
            ApplyDeflect(deflectContact);
    }

    //private void ApplySuction(ContactPoint2D _contact)
    //{
    //    // raycast side dir if fast
    //    // down dir if slow
    //    // clamp min velocity in that dir

    

    //    // Calculate the distance between A and B (Tree).
    //    float distance = Vector2.Distance(_contact.point, _contact.collider.transform.position);

    //    TreePaddleController treePC = _contact.collider.GetComponent<TreePaddleController>();
    //    treeLength = treePC.TreeHeight;

    //    //Debug.Log(treeLength.ToString("0.00")); // string.Format(@"{treeLength:0.00}"

    //    // Calculate the reduction factor based on the AnimationCurve.
    //    float reductionFactor = treeStiffnessCurve.Evaluate(distance / treeLength);

    //    // Reduce the velocity of A.
    //    rigidBody2D.velocity = initialVelocity * reductionFactor;
    //}

    private void OnDrawGizmos()
    {
        TreePaddleController treePC = deflectContact.collider != null ?
            deflectContact.collider.GetComponent<TreePaddleController>() : null;
        if (treePC != null)
        {
            var startPos = treePC.transform.position;
            Vector3 endPos = treePC.TreeTipPosition;
            Gizmos.DrawLine(startPos, endPos);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
        => HandleCollision(collision);

    private void OnCollisionStay2D(Collision2D collision)
        => HandleCollision(collision);

    #region State Checking

    private void OnDragToggle(bool state, int side)
    {
        if (state == false)
            dragTimer = dragCooldown;
    }

    private void UpdateState()
    {
        // Determine if the drag has been released, and is within the cooldown period.
        isDragReleased = dragTimer > 0f;

        // Determine if the ball's deflecting behavior is active.
        // isActive = isContacting && !isDragReleased;
        UpdateIsActive();

        // Reset flags and timers after handling contact.
        if (isContacting ||
            (playerInput.IsDragging
             && playerInput.RawDrag.sqrMagnitude > maxDragAllowed * maxDragAllowed))
        {
            dragTimer = dragCooldown;
        }
        else if (dragTimer > float.Epsilon)
            dragTimer = Mathf.Max(0f, dragTimer - Time.fixedDeltaTime);

        isContacting = false;
    }

    private void UpdateIsActive()
        => isActive = isContacting
                      && !isDragReleased
                      && rigidBody2D.velocity.sqrMagnitude > (minVelocity * minVelocity);

    #endregion

    private void HandleCollision(Collision2D _collision)
    {
        if (GetTree(_collision) != null)
        {
            isContacting = true;
            deflectContact = _collision.GetContact(0);

            // Find the contact point that qualifies as a bounce based on the dot product.
            ContactPoint2D[] contacts = new ContactPoint2D[_collision.contactCount];
            int points = _collision.GetContacts(contacts);

            for (int i = 0; i < points; i++)
            {
                ContactPoint2D contact = contacts[i];
                if (IsContactAtSurfaceTop(contact, slipUpTolerance))
                {
                    // Apply Slip
                    ApplySlip(contact);
                    break;
                }
            }
        }
        //else if (_collision.gameObject.CompareTag("Mountain"))
        //{
        //    velBooster.boo
        //}
    }

    private TreePaddleController GetTree(Collision2D _collision)
    {
        return _collision.collider != null
            ? _collision.collider.GetComponent<TreePaddleController>()
            : null;
    }

    private bool IsContactAtSurfaceTop(ContactPoint2D _contact, float _tolerance = 0.1f)
    {
        if (_contact.collider == null) return false;

        float dotUp = _contact.point
            .Towards(wheel.transform.position.To2DXY())
            .normalized
            .PerpDot(_contact.normal);
        bool result = MathF.Abs(dotUp) < _tolerance;

        return result;
    }
    

    // Apply a deflect force to the ball based on the contact point and direction.
    private void ApplyDeflect(ContactPoint2D _contact)
    {
        Vector2 downDir = transform.position.To2DXY()
            .Towards(wheel.transform.position.To2DXY())
            .normalized;

        // Determine the direction to be pushed away from the surface (left or right).
        var leftDir = downDir.Perp();
        int sideDotSign = Math.Sign(Vector2.Dot(_contact.normal, leftDir));

        // Set the appropriate direction based on the surface normal.
        var sideDir = leftDir;
        if (sideDotSign < 0) sideDir = -leftDir;

        // Apply the deflect force to the rigid body.
        Vector2 sideForce = sideDir * sideDeflectForce;
        Vector2 downForce = downDir * downDeflectForce;
        
        rigidBody2D.velocity = (sideForce + downForce) * Time.fixedDeltaTime;
    }

    private void ApplySlip(ContactPoint2D _contact)
    {
        Vector2 wheelPos = wheel.transform.position.To2DXY();
        Vector2 ballDownDir = wheelPos
            .Towards(transform.position.To2DXY())
            .normalized;
        Vector2 sideDir = ballDownDir.Perp();
        //Vector2 treeDownDir = wheelPos
        //    .Towards(_contact.collider.transform.position.To2DXY())
        //    .normalized;

        //float dirDot = treeDownDir.PerpDot(ballDownDir);
        //if (Math.Sign(dirDot) == -1)
        //    sideDir *= -1f;

        rigidBody2D.velocity += sideDir * sideSlipForce * Time.fixedDeltaTime;

        //Debug.Log((sideDir * sideSlipForce).ToString("0.00"));
    }
}
