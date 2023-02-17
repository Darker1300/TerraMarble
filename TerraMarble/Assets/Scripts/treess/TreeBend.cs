using MathUtility;
using System.Collections.Generic;
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

    [Header("Config")]
    public float bendTime = 0.05f;
    public float jumpTime = 0.05f;
    public float stretchTime = 0.01f;
    public float maxSpeed = 1000f;

    public float dragRangeOffset = 5f;
    public float dragRange = 20f;
    public Vector2 dragSize = new Vector2(0.1f, 0.2f);
    //[Range(0f, 1f)] public float deadRange = 0.1f;
    [SerializeField] private AnimationCurve treeBendCurve;

    public float dragInitalOffset = 20f;
    [SerializeField] private Vector2 dragDirTolerance = new Vector2(0.05f, 0.1f);

    [SerializeField] private bool invertXInput = false;
    [SerializeField] private bool invertDragDir = false;

    [Header("Bend Height Config")]
    public AnimationCurve PopOutHeightCurve;
    public float stretchHeight = 1f;
    [FormerlySerializedAs("bendHeight")]
    public float jumpHeight = 0.75f;

    [Header("Data")]
    [SerializeField] private float wheelDst = 10;
    public Vector2 dragInput = new Vector2(0, 0);

    public bool dragOffsetPerformed = false;
    public float dragOffsetDir = 0f;

    public List<TreePaddleController> nearbyTrees = new();

    private void Start()
    {
        aimUi = FindObjectOfType<AimTreeLockUI>(true);
        wheelRegions = FindObjectOfType<WheelRegionsManager>();
        ball = FindObjectOfType<BallStateTracker>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        wheelDst = wheelRegions.RegionTemplate.RegionPosition(0f, 1f).x;

        InputManager.LeftDragEvent += OnDragLeftToggle;
        InputManager.RightDragEvent += OnDragRightToggle;
        InputManager.LeftDragVectorEvent += OnLeftDragUpdate;
        InputManager.RightDragVectorEvent += OnRightDragUpdate;
    }


    private void Update()
    {
        Vector2 dir = ((Vector2)wheelRegions.transform.Towards(ball.transform)).normalized;
        //float xAxis = math.remap(0, 1, -1, 1, dragInput.x);
        //Debug.Log("X Axis: " + dragInput.x);
        dir = dir.RotatedByDegree(
            (dragInput.x * dragRange)
            + (dragInitalOffset * dragOffsetDir)
            - (dragRangeOffset * dragOffsetDir));
        transform.position = wheelRegions.transform.position
                             + (Vector3)dir * wheelDst;

        UpdateTrees();
    }

    private void OnDisable()
    {
        foreach (var target in nearbyTrees)
            target.Reset();
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
                fallOffPercent = dragInput.y * fallOffPercent;

                //Debug.Log("Drag: " + fallOffPercent);

                target.RotateToThis(fallOffPercent, transform.position);

            }
    }

    private void OnDragLeftToggle(bool state)
    {
        dragOffsetDir = InputManager.Instance.Mobile ? (invertDragDir ? 1f : -1f) : 0f;
        UpdateDragToggle(state);
    }

    private void OnDragRightToggle(bool state)
    {
        dragOffsetDir = InputManager.Instance.Mobile ? (invertDragDir ? -1f : 1f) : 0f;
        UpdateDragToggle(state);
    }

    private void UpdateDragToggle(bool state)
    {
        dragOffsetPerformed = false;
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

    private void SetDragDir(bool isLeft)
    {
        if (!dragOffsetPerformed)
        {
            if (InputManager.Instance.Mobile)
            {
                if (isLeft)
                    dragOffsetDir = invertDragDir ? 1f : -1f;
                else
                    dragOffsetDir = invertDragDir ? -1f : 1f;
            }
            else if (!dragOffsetPerformed)
            {
                if (Mathf.Abs(dragInput.x) > dragDirTolerance.x)
                {
                    dragOffsetDir = Mathf.Sign(dragInput.x);
                    if (invertDragDir) dragOffsetDir = -dragOffsetDir;
                }
                else if (dragInput.y > dragDirTolerance.y)
                    dragOffsetDir = 0f;
            }
            dragOffsetPerformed = true;
        }
    }

    private void UpdateDragInput(Vector2 screenDragVector)
    {
        // Update Position
        dragInput.x = -Mathf.Clamp(screenDragVector.x / dragSize.x, -1f, 1f) * (invertXInput ? -1f : 1f);
        dragInput.y = Mathf.Abs(Mathf.Clamp(screenDragVector.y / dragSize.y, -1f, 0f));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var rotToDir = collision.gameObject.GetComponentInParent<TreePaddleController>();
        if (rotToDir && !nearbyTrees.Contains(rotToDir)) nearbyTrees.Add(rotToDir);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var rotToDir = collision.gameObject.GetComponentInParent<TreePaddleController>();
        if (!rotToDir) return;

        nearbyTrees.Remove(rotToDir);
        rotToDir.Reset();
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