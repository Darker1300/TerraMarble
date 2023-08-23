using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathUtility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtility;

public class BallWindJump : MonoBehaviour
{
    [Header("Config Jump")]
    [SerializeField] private float glideSpeed = 40f;
    [SerializeField] private float minGlideSpeed = 1f;
    [SerializeField] private float upwardsSpeed = 70f;
    //[SerializeField] private bool slowDescent = true;
    [SerializeField] private float slowDescentSpeed = 400f;
    //[SerializeField] private float minDescentSpeed = -2f;


    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private float minUpDragInput = 0.1f;
    [SerializeField] private float upDragUISize = 0.15f;
    [SerializeField] private float minScreenDragForDash = 3f;

    [Header("Config Dash")]
    [SerializeField] private float minVelocityForDash = .5f;
    [SerializeField] private float minVelocityForSideDash = .1f;
    //[SerializeField] private bool canSideDash = true;
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private int dashPartEmitCount = 5;
    public bool useMinVelocityForDash = false;

    [SerializeField]
    private AnimationCurve forceCurve
        = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool doDebug = true;
    [Header("Config Particles")]
    [SerializeField]
    [FormerlySerializedAs("jumpCurve")]
    private AnimationCurve particleRateCurve
        = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField] private float partDistance = 2.5f;
    [SerializeField] private int partInitialBurst = 10;
    [SerializeField] private float partMultiplier = 10f;

    [Header("Data")]
    public float upDragInput;
    public float horDragInput = 1f;
    public Vector2 rawScreenDragInput;


    public bool IsJumping = false;
    [SerializeField] private Rigidbody2D ballRb = null;
    [SerializeField] private Wheel wheel = null;
    [SerializeField] private ParticleSystem partSystem = null;
    [SerializeField] private float jumpTimer = 0f;
    [SerializeField] private float particleTimer = 0f;
    //[SerializeField] private float jumpVelocity = 0f;
    [SerializeField] private RaycastHit2D[] hits = new RaycastHit2D[5];
    private Vector3 downDir = Vector3.down;

    private FlyUI flyUI;
    void Start()
    {
        ballRb = gameObject.GetComponentInParent<Rigidbody2D>();
        partSystem = partSystem != null ? partSystem
            : GetComponentInChildren<ParticleSystem>();

        wheel = FindObjectOfType<Wheel>();
        flyUI = GetComponentInChildren<FlyUI>(true);
        InputManager.LeftDragEvent += ToggleDrag;
        InputManager.RightDragEvent += ToggleDrag;

        InputManager.LeftDragVectorEvent += (vector, delta, screenDragVector) => UpdateDragInput(screenDragVector);
        InputManager.RightDragVectorEvent += (vector, delta, screenDragVector) => UpdateDragInput(screenDragVector);

        //InputManager.TapLeft += () => OnTap(-1);
        //InputManager.TapRight += () => OnTap(1);

        flyUI.SetUI(false);
    }

    private void ToggleDrag(bool state)
    {
        upDragInput = 0f;
        rawScreenDragInput = Vector2.zero;
    }

    private void UpdateDragInput(Vector2 screenDragVector)
    {
        upDragInput = Mathf.Clamp(screenDragVector.y / upDragUISize, 0f, 1f);
        horDragInput = Mathf.Clamp(screenDragVector.x / upDragUISize, -1f, 1f);
        rawScreenDragInput = screenDragVector;
    }

    void Update()
    {
        downDir = ballRb.transform.Towards(wheel.transform).normalized;
        UpdateHits();
        UpdateParticles();

        if (upDragInput > minUpDragInput)
        {
            // if (!IsJumping)
            {
                IsJumping = true;
                DoWindJump();
               

                flyUI.UpdateUI(upDragInput);
            }
        }
        else if (IsJumping)
        {
            IsJumping = false;
            OnWindJumpEnd();
            Camera.main.GetComponent<FollowBehavior>().cameraState = FollowBehavior.CameraState.Default;
        }
        //if (Input.GetKeyDown(KeyCode.Space)) DoWindJump();

    }

    void FixedUpdate()
    {
        OnWindJumpUpdate();

        //ApplySlowDescent();
    }

    //private void ApplySlowDescent()
    //{
    //    if (slowDescent)
    //    {
    //        Vector2 rbVLocal = transform.InverseTransformDirection(ballRb.velocity.To3DXY()).To2DXY();
    //        if (rbVLocal.y < minDescentSpeed)
    //        {
    //            rbVLocal.y = Mathf.MoveTowards(
    //                rbVLocal.y, minDescentSpeed, slowDescentSpeed * Time.fixedDeltaTime);

    //            Vector2 rbVWorld = transform.TransformDirection(rbVLocal).To2DXY();
    //            ballRb.velocity = rbVWorld;
    //        }
    //    }
    //}

    //void OnTap(int side)
    //{
    //    DoDash(dashForce, side);


    //    //// If not at rest
    //    //if (ballRb.velocity.magnitude > minVelocityForDash)
    //    //{
    //    //    Vector2 dashDirection = ballRb.transform.up.To2DXY();

    //    //    // Side Dash
    //    //    Vector2 localVelocity = transform.InverseTransformDirection(ballRb.velocity.To3DXY()).To2DXY();
    //    //    if (Mathf.Abs(localVelocity.x) > minVelocityForSideDash)
    //    //        dashDirection = (ballRb.transform.up.To2DXY() * 2f * Mathf.Sign(localVelocity.y) +
    //    //                                (ballRb.transform.right.To2DXY() * Mathf.Sign(localVelocity.x))).normalized;

    //    //    // Apply Force
    //    //    ballRb.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);

    //    //    // particles
    //    //    partSystem.Emit(dashPartEmitCount);
    //    //}
    //}
    
    /// <returns>If successfully applied force.</returns>
    public bool DoDash(float newDashForce, int side,  bool forceDash = false)
    {
        if (!forceDash && useMinVelocityForDash)
        {
            if (ballRb.velocity.sqrMagnitude < minVelocityForDash)
                return false;
        }

        Vector2 dashDirection;

        // if is dragging
        if (rawScreenDragInput.sqrMagnitude > minScreenDragForDash)
        {
            // Dash towards screen drag direction

            // Convert DragLeftScreenVector to world position
            Vector3 dragLeftWorldVectorStart = Camera.main.ScreenToWorldPoint(
                side == -1 ? InputManager.DragLeftStartScreenPos : InputManager.DragRightStartScreenPos);
            Vector3 dragLeftWorldVectorEnd = Camera.main.ScreenToWorldPoint(
                side == -1 ? InputManager.DragLeftEndScreenPos : InputManager.DragRightEndScreenPos);
            // Calculate the direction in world space
            Vector2 worldDirection = dragLeftWorldVectorEnd - dragLeftWorldVectorStart;

            // Normalize the local direction
            dashDirection = worldDirection.normalized;
        }
        // else if is moving
        else if (ballRb.velocity.sqrMagnitude > minVelocityForSideDash)
        {
            // Dash forwards
            dashDirection = ballRb.velocity.normalized;
        }
        else
        {
            // Dash upwards
            dashDirection = ballRb.transform.up.To2DXY();
        }

        // apply force
        ballRb.velocity = dashDirection * newDashForce;

        // particles
        partSystem.Emit(dashPartEmitCount);

        return true;
    }

    public void DoWindJump()
    {
        //IsJumping = true;
        UpdateParticles();
        partSystem.Emit(partInitialBurst);

        flyUI.SetUI(true);
        Camera.main.GetComponent<FollowBehavior>().cameraState = FollowBehavior.CameraState.FollowUp;
    }

    void OnWindJumpUpdate()
    {
        if (!IsJumping) return;

        Vector2 rbVLocal = transform
            .InverseTransformDirection(ballRb.velocity).To2DXY();

        float forwardDir = horDragInput;
        // if not dragging yet, keep moving forward
        if (Mathf.Approximately(0f, forwardDir))
            forwardDir = Mathf.Sign(rbVLocal.x);

        float upForce = upwardsSpeed * forceCurve.Evaluate(upDragInput) * Time.fixedDeltaTime;
        float forwardForce = Mathf.Max(glideSpeed - upForce, minGlideSpeed) * Time.fixedDeltaTime;

        // Up
        rbVLocal.y += upForce;
        if (rbVLocal.y < 0f)
            rbVLocal.y = Mathf.MoveTowards(
                rbVLocal.y, 0f, slowDescentSpeed * Time.fixedDeltaTime);
        // Forward
        rbVLocal.x += forwardForce * forwardDir;

        Vector2 rbVWorld = transform
            .TransformDirection(rbVLocal).To2DXY();
        ballRb.velocity = rbVWorld;

        // Timer
        jumpTimer += Time.fixedDeltaTime;
        //if (jumpTimer >= jumpDuration)
        //    OnWindJumpEnd();
    }

    void UpdateParticles()
    {
        if (!partSystem) return;

        float hitDst = partDistance;
        if (hits.Length > 0)
        {
            RaycastHit2D hit = hits.First();
            hitDst = hit.distance;
        }

        partSystem.transform.position = ballRb.transform.position + (downDir * hitDst);
        partSystem.transform.rotation = Quaternion.AngleAxis(MathU.Vector2ToDegree(-(Vector2)downDir), Vector3.forward);

        if (!IsJumping) return;

        float t = jumpTimer / jumpDuration;
        float curveT = particleRateCurve.Evaluate(t);
        float deltaCount = curveT * partMultiplier * Time.deltaTime;
        particleTimer += deltaCount;

        int emitCount = Mathf.FloorToInt(particleTimer);
        particleTimer -= emitCount;

        partSystem.Emit(emitCount);
    }

    void OnWindJumpEnd()
    {
        jumpTimer = 0f;
        particleTimer = 0f;
        //jumpVelocity = 0f;
        //IsJumping = false;

        flyUI.SetUI(false);
    }

    private void UpdateHits()
    {
        ballRb.Cast(downDir, hits, partDistance);
    }

    private void OnDrawGizmosSelected()
    {
        if (!doDebug || hits.Length < 1) return;
        if (wheel == null) wheel = FindObjectOfType<Wheel>();
        CircleCollider2D collider = GetComponentInChildren<CircleCollider2D>();

        Gizmos.color = Color.gray;
        GizmosExtensions.DrawWireCircle(hits.First().point, collider.radius,
            72, Quaternion.LookRotation(Vector3.up, Vector3.forward));
    }


}
