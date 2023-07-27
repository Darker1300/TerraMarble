using System;
using MathUtility;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtility;

public class TreeBend : MonoBehaviour
{
    // Positioning
    private CircleCollider2D circleCollider2D;
    private WheelRegionsManager wheelRegions;
    private BallStateTracker ball;
    private Rigidbody2D ballRb;
    private AimTreeLockUI aimUi;

    [Header("Drag Input Config")] [Tooltip("offset of starting position, in degrees")]
    public float dragStartOffset = 15f;

    [Tooltip("range extent of bend area movement, in degrees")]
    public float dragMoveRange = 20f;

    [Tooltip("percentage of screen used for touch drag input, originating from touch position")]
    public Vector2 dragScreenSize = new(0.1f, 0.2f);

    [Tooltip("how much drag dir boosts bend area, in degrees")]
    public float dragDirOffsetAmount = 20f;

    [Tooltip("how much of the screen that the drag needs before setting drag dir")] [SerializeField]
    private Vector2 dragDirTolerance = new(0.1f, 0.05f);

    [SerializeField] private bool invertXInput = true;
    [SerializeField] private bool invertDragDir = false;


    [Header("Bend Config")] public float bendTime = 0.05f;
    public float bendMaxSpeed = 1000f;
    [SerializeField] private bool doDebug = false;

    [SerializeField] private AnimationCurve treeBendCurve
        = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private AnimationCurve inputCurve
        = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [FormerlySerializedAs("PopOutHeightCurve")]
    public AnimationCurve launchCurve = new(
        new Keyframe(0, 0),
        new Keyframe(0.5f, 1f),
        new Keyframe(1, 0));

    public float launchTime = 0.05f;

    public float jumpHeight = 0.4f;
    public float stretchHeight = 0.75f;

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
    [Foldout("Data")] public Vector2 dragInput = new(0, 0);
    [Foldout("Data")] public bool isDragDirSet = false;
    [Foldout("Data")] public int dragDir = 0;
    [Foldout("Data")] public List<TreePaddleController> nearbyTrees = new();

    public bool CanBallSlide
        => isDragDirSet && dragInput.y > 0 && dragDir == Math.Sign(dragInput.x);
    public bool IsDragging
        => isDragDirSet && dragInput.y > 0;

    private void Start()
    {
        aimUi = FindObjectOfType<AimTreeLockUI>(true);
        wheelRegions = FindObjectOfType<WheelRegionsManager>();
        ball = FindObjectOfType<BallStateTracker>();
        ballRb = ball?.GetComponent<Rigidbody2D>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        wheelRadius = wheelRegions.WheelRadius;

        InputManager.LeftDragEvent += OnDragLeftToggle;
        InputManager.RightDragEvent += OnDragRightToggle;
        InputManager.LeftDragVectorEvent += OnLeftDragUpdate;
        InputManager.RightDragVectorEvent += OnRightDragUpdate;
    }


    private void Update()
    {
        Vector2 dir = ((Vector2) wheelRegions.transform.Towards(ball.transform)).normalized;

        dir = dir.RotatedByDegree(
            dragInput.x * dragMoveRange
            + dragDirOffsetAmount * dragDir
            - dragStartOffset * dragDir);
        transform.position = wheelRegions.transform.position
                             + (Vector3) dir * wheelRadius;

        UpdateTrees();
    }

    private void OnDisable()
    {
        foreach (var target in nearbyTrees)
            target.Release();
    }

    private void UpdateTrees()
    {
        if (nearbyTrees.Count > 0)
            foreach (TreePaddleController target in nearbyTrees)
            {
                if (target == null)
                {
                    // has been destroyed
                    nearbyTrees.Remove(target);
                    continue;
                }

                float upPercent;
                int direction;

                Region region = target.GetComponentInParent<Region>();
                Vector3 treeSurfacePoint = region.RegionPosition(0.5f, 1f);
                Vector3 distVector = transform.position.Towards(treeSurfacePoint);
                float distPercent = distVector.sqrMagnitude / (circleCollider2D.radius * circleCollider2D.radius);
                float curve = treeBendCurve.Evaluate(Mathf.Clamp01(distPercent));
                float fallOffPercent = 1f - curve;
                upPercent = 1f - dragInput.y * fallOffPercent;
                direction = target.DirectionFromPoint(transform.position);

                target.SetTreeState(upPercent, direction);
            }
    }

    private void OnDragLeftToggle(bool state)
    {
        dragDir = InputManager.Instance.Mobile ? invertDragDir ? 1 : -1 : 0;
        UpdateDragToggle(state);
    }

    private void OnDragRightToggle(bool state)
    {
        dragDir = InputManager.Instance.Mobile ? invertDragDir ? -1 : 1 : 0;
        UpdateDragToggle(state);
    }

    private void UpdateDragToggle(bool state)
    {
        isDragDirSet = false;
        dragInput = Vector2.zero;
    }

    private void OnLeftDragUpdate(Vector2 dragVector, Vector2 dragDelta, Vector2 screenDragVector)
    {
        UpdateDragInput(screenDragVector);

        SetDragDir(true);
    }

    private void OnRightDragUpdate(Vector2 dragVector, Vector2 dragDelta, Vector2 screenDragVector)
    {
        UpdateDragInput(screenDragVector);

        SetDragDir(false);
    }

    private void UpdateDragInput(Vector2 screenDragVector)
    {
        // Update Position
        dragInput.x = -Mathf.Clamp(screenDragVector.x / dragScreenSize.x, -1f, 1f) * (invertXInput ? -1f : 1f);
        dragInput.y = Mathf.Abs(Mathf.Clamp(screenDragVector.y / dragScreenSize.y, -1f, 0f));

        dragInput.x = math.remap(-1, 1, 0, 1, dragInput.x);
        dragInput.x = inputCurve.Evaluate(dragInput.x);
        dragInput.x = math.remap(0, 1, -1, 1, dragInput.x);

        dragInput.y = math.remap(1, 0, 0, 1, dragInput.y);
        dragInput.y = inputCurve.Evaluate(dragInput.y);
        dragInput.y = math.remap(0, 1, 1, 0, dragInput.y);
    }

    private void SetDragDir(bool isLeft)
    {
        if (!isDragDirSet)
        {
            if (InputManager.Instance.Mobile)
            {
                if (isLeft)
                    dragDir = invertDragDir ? 1 : -1;
                else
                    dragDir = invertDragDir ? -1 : 1;
            }
            else if (!isDragDirSet)
            {
                if (Mathf.Abs(dragInput.x) > dragDirTolerance.x)
                {
                    dragDir = Math.Sign(dragInput.x);
                    if (invertDragDir) dragDir = -dragDir;
                }
                else if (dragInput.y > dragDirTolerance.y)
                {
                    dragDir = 0;
                }
            }

            isDragDirSet = true;
        }
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
        //if (wheelRegions == null) wheelRegions = FindObjectOfType<WheelRegionsManager>();
        //GizmosExtensions.DrawWireCircle(wheelRegions.transform.position, minSlideHeight,
        //    72, Quaternion.LookRotation(Vector3.up, Vector3.forward));

        ball ??= FindObjectOfType<BallStateTracker>();
        ballRb ??= ball?.GetComponent<Rigidbody2D>();

        Gizmos.DrawLine(ballRb.transform.position, ballRb.transform.position + (Vector3) ballRb.velocity);


        //Vector3 center = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
        //Vector2 worldSize = InputManager.ScreenWorldSize * dragSize;

        //float camAngle = Camera.main.transform.rotation.eulerAngles.z;

        //bool xSmaller = (dragSize.x < dragSize.y);
        //if (xSmaller)
        //    GizmosExtensions.DrawWireCapsule(center, worldSize.x, worldSize.y * 2f,
        //        Quaternion.AngleAxis(90f + 0f + camAngle, Vector3.forward));
        //else
        //    GizmosExtensions.DrawWireCapsule(center, worldSize.y, worldSize.x * 2f,
        //        Quaternion.AngleAxis(90f + 90f + camAngle, Vector3.forward));

        //Gizmos.DrawWireMesh();
    }
}