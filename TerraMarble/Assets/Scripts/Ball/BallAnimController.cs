using MathUtility;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BallAnimController : MonoBehaviour
{
    public enum AnimParameter
    {
        Bounce,
        Rolling,
        RollSpeed,
        Flying,
        Falling
    }
    public Dictionary<AnimParameter, int> HashIDs = new();

    [Header("References")]
    public Animator Animator;
    public BallWindJump WindJump = null;
    public Rigidbody2D RigidBody2D = null;
    public WheelRegionsManager RegionsManager = null;

    public PhysicsMaterial2D BounceMaterial = null;

    public float YOffset = 90f;
    public bool DoBounce = false;
    public bool IsTouchingTree = false;
    public bool HasRecentlyTouchedTree
        => CurrentRollTimer > 0f;
    public float MinRollTime = 0.25f;
    public float RollSpeedFactor = 1.5f;
    public float RollVelocityFactor = 0.2f;
    public float CurrentRollTimer = 0f;

    void Awake()
    {
        Animator = GetComponentInChildren<Animator>();

        foreach (AnimParameter parameterName in Enum.GetValues(typeof(AnimParameter)))
            HashIDs.Add(parameterName,
                Animator.StringToHash(Enum.GetName(typeof(AnimParameter), parameterName)));

        WindJump = WindJump != null ? WindJump
            : GetComponentInChildren<BallWindJump>();

        RegionsManager = RegionsManager != null ? RegionsManager
            : FindObjectOfType<WheelRegionsManager>();

        RigidBody2D = RigidBody2D != null ? RigidBody2D
            : GetComponentInChildren<Rigidbody2D>();
    }

    void Update()
    {
        // Flying
        if (WindJump)
        {
            Animator.SetBool(
                HashIDs[AnimParameter.Flying],
                WindJump.IsJumping);
        }

        // Bounce
        if (DoBounce)
        {
            Animator.SetTrigger(
                HashIDs[AnimParameter.Bounce]);
            DoBounce = false;
        }

        // Rolling
        bool isRolling = false;

        if (CurrentRollTimer > 0f)
        {
            CurrentRollTimer -= Time.deltaTime;
            isRolling = true;
            float spd = Mathf.Abs(RigidBody2D.velocity.magnitude) * RollVelocityFactor;
            spd = Mathf.Clamp(spd, 0f, RollSpeedFactor);
            Animator.SetFloat(
                HashIDs[AnimParameter.RollSpeed],
                spd);
        }
        Animator.SetBool(
            HashIDs[AnimParameter.Rolling],
            isRolling);

        IsTouchingTree = false;
    }

    void FixedUpdate()
    {
        if (RigidBody2D)
        {
            Vector2 upVector = RegionsManager.transform.position
                .To2DXY()
                .Towards(RigidBody2D.position)
                .normalized;
            Vector2 velocityVector = RigidBody2D.velocity
                .normalized;

            Vector2 rejection = velocityVector.Proj(upVector);
            rejection = RigidBody2D.transform.InverseTransformVector(rejection);

            float xDir = upVector.PerpDot(velocityVector);
            float yDir = upVector.Dot(velocityVector);

            // Flip
            if (Mathf.Abs(xDir) > 0.5f)
                Animator.transform.localRotation = Quaternion.AngleAxis(
                    YOffset * -Mathf.Sign(xDir),
                    Vector3.up);



            //Debug.Log($"X Dir: {xDir}, Y Dir: {yDir}");
            //Debug.Log($"X Vel: {rejection.x.ToString("F")}, Y Vel: {rejection.y.ToString("F")}");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (BounceMaterial != null)
            if (collision.collider.sharedMaterial
                && collision.collider.sharedMaterial.name == BounceMaterial.name)
            {
                DoBounce = true;
            }

        if (!IsTouchingTree
            && collision.collider.GetComponent<TreePaddleController>())
        {
            CurrentRollTimer = MinRollTime;
            IsTouchingTree = true;
        }
    }
}
