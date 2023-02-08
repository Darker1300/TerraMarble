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
    public float maxSpeed = 1000f;

    [SerializeField] private float dragRange = 20f;
    [SerializeField] private Vector2 dragSize = new Vector2(0.1f, 0.2f);
    [Range(0f, 1f)] public float deadRange = 0.25f;
    [SerializeField] private AnimationCurve treeBendCurve;

    [SerializeField] private float dragInitalOffset = 20f;
    [SerializeField] private Vector2 dragDirTolerance = new Vector2(0.05f, 0.1f);


    [Header("bend Y Variables")]
    public AnimationCurve PopOutHeightCurve;
    public float bendHeight = 0.5f;

    [Header("Data")]
    [SerializeField] private float wheelDst = 10;
    public Vector2 dragInput = new Vector2(0, 0);

    [SerializeField] private bool dragOffsetPerformed = false;
    [SerializeField] private float dragOffsetDir = 0f;

    public List<RotateToDirectionNoRb> nearbyTrees = new();

    private void Start()
    {
        aimUi = FindObjectOfType<AimTreeLockUI>(true);
        wheelRegions = FindObjectOfType<WheelRegionsManager>();
        ball = FindObjectOfType<BallStateTracker>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        wheelDst = wheelRegions.RegionTemplate.RegionPosition(0f, 1f).x;

        InputManager.LeftDragEvent += OnDragToggle;
        InputManager.LeftDragVectorEvent += OnDragUpdate;
    }


    private void Update()
    {
        Vector2 dir = ((Vector2)wheelRegions.transform.Towards(ball.transform)).normalized;
        //float xAxis = math.remap(0, 1, -1, 1, dragInput.x);
        //Debug.Log("X Axis: " + dragInput.x);
        dir = dir.RotatedByDegree(
            (dragInput.x * dragRange)
            + (dragInitalOffset * dragOffsetDir));
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
            foreach (RotateToDirectionNoRb target in nearbyTrees)
            {
                float fallOffPercent =
                   1f - treeBendCurve.Evaluate(
                        Mathf.Clamp01(
                            transform.Towards(target.transform).sqrMagnitude
                            / (circleCollider2D.radius * circleCollider2D.radius)));
                fallOffPercent = dragInput.y * fallOffPercent;

                //Debug.Log("Drag: " + fallOffPercent);

                target.RotateToThis(fallOffPercent, transform.position);
                
            }
    }

    private void OnDragToggle(bool state)
    {
        dragOffsetPerformed = false;
    }

    public void OnDragUpdate(Vector2 dragVector, Vector2 dragDelta, Vector2 screenDragVector)
    {
        // Update Position
        // float camAngle = Camera.main.transform.rotation.eulerAngles.z;
        Vector2 cameraDragVector = screenDragVector;//.normalized;//.RotatedByDegree(camAngle + 90f);
        dragInput.x = -Mathf.Clamp(cameraDragVector.x / dragSize.x, -1f, 1f);
        dragInput.y = Mathf.Abs(Mathf.Clamp(cameraDragVector.y / dragSize.y, -1f, 0f));

        if (!dragOffsetPerformed)
        {
            dragOffsetPerformed = true;
            if (Mathf.Abs(dragInput.x) > dragDirTolerance.x)
                dragOffsetDir = Mathf.Sign(dragInput.x);
            else if (dragInput.y > dragDirTolerance.y)
                dragOffsetDir = 0f;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var rotToDir = collision.gameObject.GetComponentInParent<RotateToDirectionNoRb>();
        if (rotToDir && !nearbyTrees.Contains(rotToDir)) nearbyTrees.Add(rotToDir);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var rotToDir = collision.gameObject.GetComponentInParent<RotateToDirectionNoRb>();
        if (!rotToDir) return;

        nearbyTrees.Remove(rotToDir);
        rotToDir.Reset();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
        Vector2 worldSize = InputManager.ScreenWorldSize * dragSize;

        float camAngle = Camera.main.transform.rotation.eulerAngles.z;
        
        bool xSmaller = (dragSize.x < dragSize.y);
        if (xSmaller)
            GizmosExtensions.DrawWireCapsule(center, worldSize.x, worldSize.y * 2f,
                Quaternion.AngleAxis(90f + 0f + camAngle, Vector3.forward));
        else
            GizmosExtensions.DrawWireCapsule(center, worldSize.y, worldSize.x * 2f,
                Quaternion.AngleAxis(90f + 90f + camAngle, Vector3.forward));

        //Gizmos.DrawWireMesh();
    }

}