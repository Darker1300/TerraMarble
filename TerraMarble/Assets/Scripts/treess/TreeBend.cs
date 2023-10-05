using MathUtility;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtility;

public class TreeBend : MonoBehaviour
{
    // Positioning
    private CircleCollider2D bendCircleCollector;
    private WheelRegionsManager wheelRegions;
    private BallStateTracker ball;
    private Rigidbody2D ballRb;
    private PlayerInput playerInput;

    [Header("Drag Input Config")]
    [Tooltip("offset of starting position, in degrees")]
    public float dragStartOffset = 15f;

    [Tooltip("range extent of bend area movement, in degrees")]
    public float dragMoveRange = 20f;

    [Tooltip("how much drag dir boosts bend area, in degrees")]
    public float dragDirOffsetAmount = 20f;

    [Header("Bend Config")] 
    public float bendTime = 0.05f;
    public float bendMaxSpeed = 1000f;
    [SerializeField] private bool doDebug = false;
    public float bendMaxAngle = 89f;

    [SerializeField]
    private AnimationCurve treeBendCurve
        = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private bool allowBendIdle = false;
    [SerializeField] private Vector2 bendIdle = new Vector2(-0.05f, 1f);
    [SerializeField] private float bendIdleTime = 0.2f;

    [Header("Spring Config")]
    public float collisionSensitivity = 1f;
    public float collisionForceMin = 1f;
    public float collisionForceMax = 100f;

    public float collisionAngleMax = 40f;
    public float collisionSpeedIncrease = 10f;
    public float collisionSpeedDecrease = 2f;

    [Header("Launch Config")]
    public float launchTime = 0.05f;
    public AnimationCurve launchCurve = new(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 1f),
        new Keyframe(1, 0));

    public float jumpHeight = 0.4f;
    public float stretchHeight = 0.75f;

    [Header("Wobble Config")]
    public float wobbleMaxAngle = 30f;
    public float wobbleSlowFactor = 0.75f;
    public float wobbleMinThreshold = 5.0f;


    [Header("Slide Config")] public float minSlideVelocityNeeded = 22f;
    public float slideDirectionThreshold = .98f;
    public float slideLiftAngle = 0f;
    public float slideAdjustmentSpeed = 160f;
    public float slideBoostForce = 30f;
    public float slideMinForce = 10f;

    [Foldout("Data")] [SerializeField] private float wheelRadius = 10;
    [Foldout("Data")] public List<TreePaddleController> nearbyTrees = new();
    [Foldout("Data")] [SerializeField] private Vector2 previousTreeDrag = Vector2.zero;
    [Foldout("Data")] [SerializeField] private Vector2 bendIdleVelocity;

    private void Start()
    {
        wheelRegions = FindObjectOfType<WheelRegionsManager>();
        ball = FindObjectOfType<BallStateTracker>();
        ballRb = ball?.GetComponent<Rigidbody2D>();
        bendCircleCollector = GetComponent<CircleCollider2D>();
        wheelRadius = wheelRegions.WheelRadius;

        playerInput = FindObjectOfType<PlayerInput>();
    }


    private void Update()
    {
        // Get the normalized direction vector from the wheel center to the ball.
        Vector2 dir = ((Vector2)wheelRegions.transform.Towards(ball.transform)).normalized;

        // Adjust the direction based on user input and various drag factors.
        // dragInput.x * dragMoveRange is the basic input scaled by a range factor.
        // dragDirOffsetAmount * inputSide accounts for dynamic offset in the drag direction.
        // -dragStartOffset * inputSide subtracts the starting drag offset to ensure the direction vector starts from the correct point.

        int inputSide = playerInput.TreeDragSide;
        Vector2 dragInput = playerInput.TreeDrag;
        dragInput.y = Mathf.Clamp01(-dragInput.y);

        dir = dir.RotatedByDegree(
            dragInput.x * dragMoveRange
            + dragDirOffsetAmount * inputSide
            - dragStartOffset * inputSide);

        // Update the position of the 'tree circle collider' object this script is attached to.
        // This makes it follow along the edge of the wheel at a distance specified by wheelRadius.
        transform.position = wheelRegions.transform.position
                             + (Vector3)dir * wheelRadius;

        // Update the state of the trees in the scene
        UpdateTrees();
    }

    private void OnDisable()
    {
        foreach (var target in nearbyTrees)
            target.Release();
    }

    private void UpdateTrees()
    {
        nearbyTrees.RemoveAllNull();

        float clampedTreeDragY = Mathf.Clamp01(-playerInput.TreeDrag.y);

        Vector2 treeDrag = playerInput.TreeDrag.WithY(clampedTreeDragY);

        // Allow Bend Idle
        if (allowBendIdle && IsBendIdle(treeDrag, previousTreeDrag))
        {
            int xSign = Math.Sign(treeDrag.x);
            Vector2 dragTarget = new Vector2(
                bendIdle.x * (xSign != 0 ? xSign : 1),
                bendIdle.y);
            treeDrag = dragTarget;
            //treeDrag = Vector2.SmoothDamp(treeDrag, dragTarget, ref bendIdleVelocity, bendIdleTime);
        } else bendIdleVelocity = Vector2.zero;

        foreach (TreePaddleController tree in nearbyTrees)
        {
            // Get the parent region of the tree and the point on the tree surface
            Region region = tree.GetComponentInParent<Region>();
            Vector3 treeGroundSurfacePoint = region.RegionPosition(0.5f, 1f);

            // Calculate the direction vector from the object this script is attached to the tree surface point
            Vector3 directionVector = transform.position.Towards(treeGroundSurfacePoint);

            // Calculate the squared distance between this object and the tree, normalized by the square of the circle collider's radius
            float distPercent = directionVector.sqrMagnitude / (bendCircleCollector.radius * bendCircleCollector.radius);

            // Apply the tree bending curve across the extent of the circle collider
            float bendCurve = treeBendCurve.Evaluate(Mathf.Clamp01(distPercent));

            float fallOffPercent = 1f - bendCurve;

            float bendPercent = treeDrag.y * fallOffPercent;
            int direction = tree.DirectionFromPoint(transform.position);

            // Update the tree's state
            tree.SetTreeState(bendPercent, direction);
        }

        previousTreeDrag = playerInput.TreeDrag.WithY(clampedTreeDragY);
    }

    private bool IsBendIdle(Vector2 _currentTreeDrag, Vector2 _previousTreeDrag, float _tolerance = 0.01f)
    {
        return (_currentTreeDrag - _previousTreeDrag).sqrMagnitude < _tolerance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var treeTarget = collision.gameObject.GetComponentInParent<TreePaddleController>();
        if (treeTarget && !nearbyTrees.Contains(treeTarget)) nearbyTrees.Add(treeTarget);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var treeTarget = collision.gameObject.GetComponentInParent<TreePaddleController>();
        if (!treeTarget) return;

        nearbyTrees.Remove(treeTarget);
        treeTarget.Release();
    }

    private void OnDrawGizmosSelected()
    {
        if (!doDebug) return;

        Gizmos.color = Color.white;

        ball ??= FindObjectOfType<BallStateTracker>();
        ballRb ??= ball?.GetComponent<Rigidbody2D>();

        Gizmos.DrawLine(ballRb.transform.position, ballRb.transform.position + (Vector3)ballRb.velocity);
    }
}