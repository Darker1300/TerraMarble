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
    private AimTreeLockUI aimUi;

    [Header("Drag Input Config")]
    [Tooltip("offset of starting position, in degrees")]
    public float dragStartOffset = 15f;

    [Tooltip("range extent of bend area movement, in degrees")]
    public float dragMoveRange = 20f;

    [Tooltip("percentage of screen used for touch drag input, originating from touch position")]
    public Vector2 dragScreenSize = new Vector2(0.1f, 0.2f);

    [Tooltip("how much drag dir boosts bend area, in degrees")]
    public float dragDirOffsetAmount = 20f;

    [Tooltip("how much of the screen that the drag needs before setting drag dir")]
    [SerializeField] private Vector2 dragDirTolerance = new Vector2(0.1f, 0.05f);

    [SerializeField] private bool invertXInput = true;
    [SerializeField] private bool invertDragDir = false;


    [Header("Bend Config")]

    public float bendTime = 0.05f;
    public float bendMaxSpeed = 1000f;
    public float flipTime = 0.1f;

    [SerializeField]
    private AnimationCurve treeBendCurve
        = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [FormerlySerializedAs("PopOutHeightCurve")]
    public AnimationCurve launchCurve = new AnimationCurve(
            new(0, 0),
            new(0.5f, 1f),
            new(1, 0));

    public float launchTime = 0.05f;

    public float jumpHeight = 0.4f;
    public float stretchHeight = 0.75f;

    public float wobbleMaxAngle = 30f;
    public float wobbleSlowFactor = 0.75f;
    public float wobbleMinThreshold = 5.0f;

    [Foldout("Data")] [SerializeField] private float wheelRadius = 10;
    [Foldout("Data")] public Vector2 dragInput = new Vector2(0, 0);
    [Foldout("Data")] public bool isDragDirSet = false;
    [Foldout("Data")] public float dragDir = 0f;
    [Foldout("Data")] public List<TreePaddleController> nearbyTrees = new();

    private void Start()
    {
        aimUi = FindObjectOfType<AimTreeLockUI>(true);
        wheelRegions = FindObjectOfType<WheelRegionsManager>();
        ball = FindObjectOfType<BallStateTracker>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        wheelRadius = wheelRegions.WheelRadius;

        InputManager.LeftDragEvent += OnDragLeftToggle;
        InputManager.RightDragEvent += OnDragRightToggle;
        InputManager.LeftDragVectorEvent += OnLeftDragUpdate;
        InputManager.RightDragVectorEvent += OnRightDragUpdate;
    }


    private void Update()
    {
        Vector2 dir = ((Vector2)wheelRegions.transform.Towards(ball.transform)).normalized;

        dir = dir.RotatedByDegree(
            (dragInput.x * dragMoveRange)
            + (dragDirOffsetAmount * dragDir)
            - (dragStartOffset * dragDir));
        transform.position = wheelRegions.transform.position
                             + (Vector3)dir * wheelRadius;

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

                Region region = target.GetComponentInParent<Region>();
                Vector3 treeSurfacePoint = region.RegionPosition(0.5f, 1f);
                Vector3 distVector = transform.position.Towards(treeSurfacePoint);
                float distPercent = distVector.sqrMagnitude / (circleCollider2D.radius * circleCollider2D.radius);
                float curve = treeBendCurve.Evaluate(Mathf.Clamp01(distPercent));
                float fallOffPercent = 1f - curve;
                fallOffPercent = 1f - dragInput.y * fallOffPercent;

                //Debug.Log("Drag: " + fallOffPercent);

                target.SetTreeState(fallOffPercent, target.DirectionFromPoint(transform.position));

            }
    }

    private void OnDragLeftToggle(bool state)
    {
        dragDir = InputManager.Instance.Mobile ? (invertDragDir ? 1f : -1f) : 0f;
        UpdateDragToggle(state);
    }

    private void OnDragRightToggle(bool state)
    {
        dragDir = InputManager.Instance.Mobile ? (invertDragDir ? -1f : 1f) : 0f;
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


        //if (!dragOffsetPerformed)
        //{
        //    dragOffsetPerformed = true;
        //    if (Mathf.Abs(dragInput.x) > dragDirTolerance.x)
        //        dragOffsetDir = Mathf.Sign(dragInput.x);
        //    else if (dragInput.y > dragDirTolerance.y)
        //        dragOffsetDir = 0f;
        //}
    }

    private void UpdateDragInput(Vector2 screenDragVector)
    {
        // Update Position
        dragInput.x = -Mathf.Clamp(screenDragVector.x / dragScreenSize.x, -1f, 1f) * (invertXInput ? -1f : 1f);
        dragInput.y = Mathf.Abs(Mathf.Clamp(screenDragVector.y / dragScreenSize.y, -1f, 0f));
    }

    private void SetDragDir(bool isLeft)
    {
        if (!isDragDirSet)
        {
            if (InputManager.Instance.Mobile)
            {
                if (isLeft)
                    dragDir = invertDragDir ? 1f : -1f;
                else
                    dragDir = invertDragDir ? -1f : 1f;
            }
            else if (!isDragDirSet)
            {
                if (Mathf.Abs(dragInput.x) > dragDirTolerance.x)
                {
                    dragDir = Mathf.Sign(dragInput.x);
                    if (invertDragDir) dragDir = -dragDir;
                }
                else if (dragInput.y > dragDirTolerance.y)
                    dragDir = 0f;
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