using MathUtility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

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
    public Transform BodyBase;
    public BallWindJump WindJump = null;
    public Rigidbody2D RigidBody2D = null;
    public WheelRegionsManager RegionsManager = null;
    public NearbySensor NearbySensor = null;
    [SerializeField] private PhysicsMaterial2D BounceMaterial = null;

    [Header("Flip")]
    [SerializeField] private bool useYaw = true;
    [SerializeField] private float yawOffset = 0f;

    [Header("Rotation")]
    [SerializeField] private bool usePitch = true;
    //[SerializeField] private float pitchPosSpeed = 30f;
    //[SerializeField] private float pitchUprightSpeed = 180f; // Degrees
    [SerializeField] private float pitchOffset = 0f;
    [SerializeField] private float pitchPosOffset = 0.5f;
    [SerializeField] private Vector2 pitchDefaultDirection = Vector2.down;

    [Header("Tree Rolling")]
    [SerializeField] private float pitchMaxAngleWhileRolling = 15f; // Degrees
    [SerializeField] private float MinRollTime = 0.1f;
    [SerializeField] private string surfaceBufferName = "Surface";
    [SerializeField] private float RollSpeedFactor = 1.5f;
    [SerializeField] private float RollVelocityFactor = 0.2f;
    public bool isTreeRolling = false;
    private bool IsTouchingTreeThisFrame = false;
    [SerializeField] private float CurrentRollTimer = 0f;
    [SerializeField] private NearbySensor.ColliderBuffer surfaceColliders;
    bool IsNearbySurface
        => surfaceColliders.ColliderSet.Stay.Count > 0;

    [Header("Bounce")]
    [SerializeField] private bool DoBounce = false;

    void Start()
    {
        Animator = Animator != null ? Animator
            : GetComponentInChildren<Animator>();

        HashIDs = UnityU.EnumToHashIDs<AnimParameter>();

        WindJump = WindJump != null ? WindJump
            : GetComponentInChildren<BallWindJump>();

        NearbySensor = NearbySensor != null ? NearbySensor
            : GetComponentInChildren<NearbySensor>();

        RegionsManager = RegionsManager != null ? RegionsManager
            : FindObjectOfType<WheelRegionsManager>();

        RigidBody2D = RigidBody2D != null ? RigidBody2D
            : GetComponentInChildren<Rigidbody2D>();

        BodyBase = BodyBase != null ? BodyBase
            : Animator.transform.parent;
        
        surfaceColliders = NearbySensor.FindBuffer(surfaceBufferName);
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
        if (IsTouchingTreeThisFrame || (isTreeRolling && IsNearbySurface))
            CurrentRollTimer = MinRollTime;

        isTreeRolling = false;
        
        if (CurrentRollTimer > 0f)
        {
            CurrentRollTimer -= Time.deltaTime;
            isTreeRolling = true;


            float velocitySpeed = RigidBody2D.velocity.magnitude;


            float clampedSpeed = Mathf.Abs(velocitySpeed) * RollVelocityFactor;
            clampedSpeed = Mathf.Clamp(clampedSpeed, 0f, RollSpeedFactor);
            Animator.SetFloat(
                HashIDs[AnimParameter.RollSpeed],
                clampedSpeed);
        }
        Animator.SetBool(
            HashIDs[AnimParameter.Rolling],
            isTreeRolling);

        IsTouchingTreeThisFrame = false;

        // Rotation / Flip
        if (RigidBody2D)
        {
            Vector2 upVector = RegionsManager.transform.position
                .To2DXY()
                .Towards(RigidBody2D.position)
                .normalized;
            Vector2 velocityVector = RigidBody2D.velocity
                .normalized;

            // Vector2 rejection = velocityVector.Proj(upVector);
            // rejection = RigidBody2D.transform.InverseTransformVector(rejection);

            float xDir = upVector.PerpDot(velocityVector);
            //float yDir = upVector.Dot(velocityVector);

            // Flip
            Quaternion localYaw = Quaternion.identity;
            float xDirSign = Mathf.Sign(xDir);
            if (Mathf.Abs(xDir) > 0.5f)
            {
                localYaw = Quaternion.AngleAxis(
                    (yawOffset + 90f) * -xDirSign,
                    Vector3.up);
            }

            //if (usePitch)
            //{
            //  // Vector2 velocityVector = RigidBody2D.velocity.normalized;
            //  // RegionsManager.transform is the WorldCenter

            // flying: rotate towards local velocity vector
            // rolling: rotate towards up-20f while touching trees. ?

            Vector2 localVelocityVector = RigidBody2D.transform.InverseTransformVector(velocityVector).To2DXY();
            // Set default when at rest
            float velocitySqrMag = localVelocityVector.sqrMagnitude;
            if (Mathf.Approximately(velocitySqrMag, 0f))
                localVelocityVector = -pitchDefaultDirection;

            // Calculate rotation
            Vector2 localFacingVector = -localVelocityVector;// * -xDirSign; // flips the pitch, so it moves towards the right side
            localFacingVector = localFacingVector.Perp();  // goes from up(towards) to right 
            localFacingVector = localFacingVector.Rotate(pitchOffset); // so can rotate, for debugging purposes
            float localFacingAngle = localFacingVector.ToDegrees();

            if (isTreeRolling)  // keep Upright while Rolling
                localFacingAngle = MathU.ClampAngle(localFacingAngle, -pitchMaxAngleWhileRolling, pitchMaxAngleWhileRolling);

            Quaternion localPitchRotation = Quaternion.AngleAxis(localFacingAngle, Vector3.forward);

            // Apply rotation
            BodyBase.localRotation = localPitchRotation;

            //BodyBase.localRotation =
            //    Quaternion.RotateTowards(
            //        BodyBase.localRotation,
            //        localPitchRotation,
            //        pitchUprightSpeed * Time.deltaTime);

            // Calculate and apply offset to BodyBase's local position, so it is opposite the velocity's direction.
            Vector2 localPositionOffset = -localFacingAngle.DegreesToVector2().Perp() * pitchPosOffset;
            BodyBase.localPosition = localPositionOffset;
            //BodyBase.localPosition =
            //    Vector2.MoveTowards(BodyBase.localPosition.To2DXY(),
            //            localPositionOffset, pitchPosSpeed * Time.deltaTime)
            //        .To3DXY(BodyBase.localPosition.z);
            //}
            //else
            //{
            //    Vector3 targetPosition = (Vector2.down * pitchPosOffset).To3DXY(BodyBase.localPosition.z);
            //    BodyBase.localPosition = Vector3.MoveTowards(
            //        BodyBase.localPosition,
            //        targetPosition,
            //        pitchPosSpeed * Time.deltaTime);

            //    Quaternion targetRotation = Quaternion.identity;
            //    BodyBase.localRotation =
            //        Quaternion.RotateTowards(
            //            BodyBase.localRotation,
            //            targetRotation,
            //            pitchUprightSpeed * Time.deltaTime);
            //}

            // Flip
            Animator.transform.localRotation = localYaw;
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

        if (!IsTouchingTreeThisFrame
            && collision.collider.GetComponent<TreePaddleController>())
        {
            IsTouchingTreeThisFrame = true;
        }
    }
}
