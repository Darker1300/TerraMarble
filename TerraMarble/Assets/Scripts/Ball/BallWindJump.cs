using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathUtility;
using UnityEngine;
using UnityUtility;

public class BallWindJump : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float glideSpeed = 10f;
    [SerializeField] private float minGlideSpeed = 1f;
    [SerializeField] private float upwardsSpeed = 2f;
    [SerializeField] private float slowDescentSpeed = 1f;
    [SerializeField] private float jumpDuration = 1f;
    //[SerializeField] private float slowDescentSpeed = 1f;
    //[SerializeField] private float minDescentForce = 0.1f;
    //[SerializeField] private float jumpVerticalTime = 0.5f;
    [SerializeField]
    private AnimationCurve jumpCurve
        = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField] private float partDistance = 2.5f;
    [SerializeField] private int partInitialBurst = 10;
    [SerializeField] private float partMultiplier = 10f;
    [SerializeField] private bool doDebug = true;

    [Header("Data")]
    [SerializeField] private float minUpDragInput = 0.1f;
    [SerializeField] private float upDragUISize = 0.1f;
    [SerializeField] public float upDragInput = 0f;
    [SerializeField] public float horDragInput = 1f;
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
        partSystem = gameObject.GetComponentInChildren<ParticleSystem>();
        wheel = FindObjectOfType<Wheel>();
        flyUI = GetComponentInChildren<FlyUI>(true);
        InputManager.LeftDragEvent += ToggleDrag;
        InputManager.RightDragEvent += ToggleDrag;

        InputManager.LeftDragVectorEvent += (vector, delta, screenDragVector) => UpdateDragInput(screenDragVector);
        InputManager.RightDragVectorEvent += (vector, delta, screenDragVector) => UpdateDragInput(screenDragVector);

        flyUI.SetUI(false);
    }

    private void ToggleDrag(bool state)
    {
        upDragInput = 0f;
    }

    private void UpdateDragInput(Vector2 screenDragVector)
    {
        upDragInput = Mathf.Abs(Mathf.Clamp(screenDragVector.y / upDragUISize, 0f, 1f)); ;
        horDragInput = Mathf.Clamp(screenDragVector.x / upDragUISize, -1f, 1f);
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
            }
        }
        else
        {
            IsJumping = false;
        }
        //if (Input.GetKeyDown(KeyCode.Space)) DoWindJump();
    }

    void FixedUpdate()
    {
        OnWindJumpUpdate();
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
            .InverseTransformDirection(ballRb.velocity).To2DXY()
            .Rotate270();

        float forwardDir;// = Mathf.Sign(rbVLocal.y);
        forwardDir = -horDragInput;
        forwardDir = Mathf.Approximately(0f, forwardDir) ? -1f : forwardDir;

        float upForce = upwardsSpeed * upDragInput * Time.fixedDeltaTime;
        float forwardForce = Mathf.Max(glideSpeed - upForce, minGlideSpeed) * Time.fixedDeltaTime;

        //Up
        rbVLocal.x += upForce;
        if (rbVLocal.x < 0f)
            rbVLocal.x = Mathf.MoveTowards(
                rbVLocal.x, 0f, slowDescentSpeed * Time.fixedDeltaTime);
        //Forward
        rbVLocal.y += forwardForce * forwardDir;

        flyUI.UpdateUI(upDragInput);

        Vector2 rbVWorld = transform
            .TransformDirection(rbVLocal).To2DXY()
            .Rotate90();
        ballRb.velocity = rbVWorld;

        // Timer
        jumpTimer += Time.fixedDeltaTime;
        if (jumpTimer >= jumpDuration)
            OnWindJumpEnd();
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
        float curveT = jumpCurve.Evaluate(t);
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
