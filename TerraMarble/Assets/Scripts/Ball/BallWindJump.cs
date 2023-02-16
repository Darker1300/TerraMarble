using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MathUtility;
using UnityEngine;

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

    [Header("Data")]
    [SerializeField] private float minUpDragInput = 0.1f;
    [SerializeField] private float upDragSize = 0.1f;
    [SerializeField] private float upDragInput = 0f;
    public bool IsJumping = false;
    [SerializeField] private Rigidbody2D ballRb = null;
    [SerializeField] private Wheel wheel = null;
    [SerializeField] private ParticleSystem partSystem = null;
    [SerializeField] private float jumpTimer = 0f;
    [SerializeField] private float particleTimer = 0f;
    //[SerializeField] private float jumpVelocity = 0f;
    [SerializeField] private RaycastHit2D[] hits = new RaycastHit2D[5];


    void Start()
    {
        ballRb = gameObject.GetComponentInParent<Rigidbody2D>();
        partSystem = gameObject.GetComponentInChildren<ParticleSystem>();
        wheel = FindObjectOfType<Wheel>();

        InputManager.LeftDragEvent += ToggleDrag;
        InputManager.RightDragEvent += ToggleDrag;

        InputManager.LeftDragVectorEvent += (vector, delta, screenDragVector) => UpdateDragInput(screenDragVector);
        InputManager.RightDragVectorEvent += (vector, delta, screenDragVector) => UpdateDragInput(screenDragVector);
    }

    private void ToggleDrag(bool state)
    {
        upDragInput = 0f;
    }

    private void UpdateDragInput(Vector2 screenDragVector)
    {
        upDragInput = Mathf.Abs(Mathf.Clamp(screenDragVector.y / upDragSize, 0f, 1f)); ;
    }

    void Update()
    {
        UpdateParticles();

        if (upDragInput > minUpDragInput)
            DoWindJump();
        //if (Input.GetKeyDown(KeyCode.Space)) DoWindJump();
    }

    void FixedUpdate()
    {
        OnWindJumpUpdate();
    }

    public void DoWindJump()
    {
        IsJumping = true;
        UpdateParticles();
        partSystem.Emit( partInitialBurst);
    }

    void OnWindJumpUpdate()
    {
        if (!IsJumping) return;
        // alter Rigidbody
        float t = jumpTimer / jumpDuration;
        float curve = jumpCurve.Evaluate(t);
        //float force = glideSpeed * curve;

        Vector2 upV = wheel.transform.Towards(ballRb.transform).normalized;
        Vector2 rbV = ballRb.velocity;

        Vector2 rbVLocal = partSystem.transform.InverseTransformDirection(ballRb.velocity);
        Vector2 newLocal = rbVLocal;

        float forwardDir = Mathf.Sign(rbVLocal.y);
        forwardDir = Mathf.Approximately(0f, forwardDir) ? 1f : forwardDir;

        float upForce = upwardsSpeed * upDragInput * Time.fixedDeltaTime;
        float forwardForce = Mathf.Max(glideSpeed - upForce, minGlideSpeed) * Time.fixedDeltaTime;

        newLocal.x += upForce;
        if (newLocal.x < 0f)
            newLocal.x = Mathf.MoveTowards(
                newLocal.x, 0f, slowDescentSpeed * Time.fixedDeltaTime);

        newLocal.y += forwardForce * forwardDir;



        //newLocal.y = Mathf.Max(Mathf.Abs(newLocal.y), minGlideSpeed * Time.fixedDeltaTime) * forwardDir * glideSpeed;
        // newLocal.x = Mathf.Abs(newLocal.y) * glideSpeed;




        //float pushF = -Mathf.Min(rbVLocal.x, 0f);
        //float forwardDir = Mathf.Sign(rbVLocal.y);
        //forwardDir = Mathf.Approximately(0f, forwardDir) ? 1f : forwardDir;
        //rbVLocal.y = rbVLocal.y * (1f - curve) + (rbVLocal.y + (glideSpeed * forwardDir)) * curve;

        //if (rbVLocal.x < minDescentForce)
        //    rbVLocal.x = Mathf.MoveTowards(rbVLocal.x, 0, slowDescentSpeed * Time.fixedDeltaTime);

        //float sign = Mathf.Sign(rbVLocal.y);
        //rbVLocal.x = rbVLocal.x * (1f - curve) + (rbVLocal.y + glideSpeed * sign) * curve;

        //rbVLocal.x = rbVLocal.x * (1f - curve) + (glideSpeed * curve);
        Vector2 rbVWorld = partSystem.transform.TransformDirection(newLocal);

        //float rbMag = rbV.magnitude;
        //Vector2 newRbV = Vector2.ClampMagnitude(rbV + (force * upV), Mathf.Max(rbMag, force));

        //float rbMagRemainder = rbMag * (1f - curve);
        //Vector2 newRbV = (rbMagRemainder * rbV.normalized) + (force * upV);

        ballRb.velocity = rbVWorld;

        // Timer
        jumpTimer += Time.fixedDeltaTime;
        if (jumpTimer >= jumpDuration)
            OnWindJumpEnd();
    }

    void UpdateParticles()
    {
        if (!partSystem) return;

        Vector3 dir = ballRb.transform.Towards(wheel.transform).normalized;

        int hitCount = ballRb.Cast(dir, hits, partDistance);
        float hitDst = partDistance;
        if (hitCount > 0)
        {
            RaycastHit2D hit = hits.First();
            hitDst = hit.distance;
        }

        partSystem.transform.position = ballRb.transform.position + (dir * hitDst);
        partSystem.transform.rotation = Quaternion.AngleAxis(MathU.Vector2ToDegree(-(Vector2)dir), Vector3.forward);

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
        IsJumping = false;
    }
}
