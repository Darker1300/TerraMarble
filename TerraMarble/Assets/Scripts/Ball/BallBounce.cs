using MathUtility;
using System;
using System.Linq;
using UnityEngine;

public class BallBounce : MonoBehaviour
{
    [Header("Deflect Config")]
    // [SerializeField] private float upTolerance = 0.36f;
    [SerializeField] private float sideDeflectForce = 100f;
    [SerializeField] private float downDeflectForce = 80f;
    [SerializeField] private float maxDragAllowed = 0.2f;
    [SerializeField] private float dragCooldown = 0.2f;
    [SerializeField] private float minVelocity = 0.1f;
    [Header("Tree Stiffness Config")]
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
    private ContactPoint2D contact;

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
            ApplyDeflect(contact);
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
        TreePaddleController treePC = contact.collider != null ?
            contact.collider.GetComponent<TreePaddleController>() : null;
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
        if (CheckHit(_collision, ref contact))
        {
            isContacting = true;
            contact = _collision.GetContact(0);
        }
    }

    // Check if the collision qualifies as a deflect, and set the relevant contact point.
    private bool CheckHit(Collision2D _collision, ref ContactPoint2D _outContact)
    {
        var tree = _collision.collider != null
            ? _collision.collider.GetComponent<TreePaddleController>() : null;

        // Check if the player is dragging and if the drag exceeds the allowed threshold.
        if (tree == null)
            return false;
        //if (playerInput.IsDragging && playerInput.RawDrag.sqrMagnitude > maxDragAllowed)
        //    return false;

        return true;

        //// Calculate the direction from the ball's position to the world center.
        //Vector2 downDirection = transform.position.To2DXY().Towards(wheel.transform.position.To2DXY()).normalized;

        //// Find the contact point that qualifies as a bounce based on the dot product.
        //_outContact = _collision.contacts.FirstOrDefault(point2D =>
        //{
        //    return Vector2.Dot(point2D.normal, -downDirection) > upTolerance;
        //});

        //// Return whether a valid contact point was found.
        //return _outContact.collider != null;
    }

    //private Vector2 CalcSpriteWorldSize(SpriteRenderer spriteRenderer)
    //{
    //    // Get the local bounds of the SpriteRenderer
    //    Bounds localBounds = spriteRenderer.localBounds;

    //    // Transform the local bounds corners to world space
    //    Vector3 worldMin = spriteRenderer.transform.TransformPoint(localBounds.min);
    //    Vector3 worldMax = spriteRenderer.transform.TransformPoint(localBounds.max);

    //    // Calculate the dimensions in world space
    //    float worldWidth = Mathf.Abs(worldMax.x - worldMin.x);
    //    float worldHeight = Mathf.Abs(worldMax.y - worldMin.y);

    //    return new Vector2(worldWidth, worldHeight);

    //    //// Transform the local bounds to world space.
    //    //Vector3 minWorld = spriteRenderer.transform.TransformPoint(localBounds.min);
    //    //Vector3 maxWorld = spriteRenderer.transform.TransformPoint(localBounds.max);

    //    //// Calculate the world size dimensions.
    //    //Vector2 worldSize = new(
    //    //    Mathf.Abs(maxWorld.x - minWorld.x),
    //    //    Mathf.Abs(maxWorld.y - minWorld.y));

    //    //return worldSize;
    //}



    // Apply a deflect force to the ball based on the contact point and direction.
    private void ApplyDeflect(ContactPoint2D _contact)
    {
        Vector2 downDir = transform.position.To2DXY().Towards(wheel.transform.position.To2DXY()).normalized;

        // Determine the direction to be pushed away from the surface (left or right).
        var rightDir = downDir.Perp();
        int sideDotSign = Math.Sign(Vector2.Dot(_contact.normal, rightDir));

        // Set the appropriate direction based on the surface normal.
        var sideDir = rightDir;
        if (sideDotSign < 0) sideDir = -rightDir;

        // Apply the deflect force to the rigid body.
        Vector2 sideForce = sideDir * sideDeflectForce;
        Vector2 downForce = downDir * downDeflectForce;


        rigidBody2D.velocity = (sideForce + downForce) * Time.fixedDeltaTime;
    }

}
