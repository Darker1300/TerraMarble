/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.IK;

/// <summary>
/// Component that contains the Axia Bone setting variables, resolves its animation and call the Legs movement 
/// </summary>
public class PL2D_AxialBone : MonoBehaviour
{
    public int axialBoneIndex;
    public bool isMainAxialBone;

    public PL2D_Animator pL2dAnimator;

    public float bodyVelocity;

    float lastFrameBodyVelocity;
    Quaternion lastFrameBodyRotation;
    float angVelocityMagnitude;
    Vector3 angVelocityAxis;

    public float bodyAngularVelocity
    {
        get => ((angVelocityAxis * angVelocityMagnitude) / Time.deltaTime).z;
    }

    // Reference to child legs
    public List<PL2D_Leg> legs;

    public bool isMoving;

    public int actualLeg;

    public float distanceBeforeMoveLeg;
    public float angleBeforeMoveLeg;

    public float rayToGroundLength;
    public float RayToGroundLength
    {
        get
        {
            return rayToGroundLength;
        }
        set
        {
            if (value >= bodyDistanceToGround)
                rayToGroundLength = value;
            else
                rayToGroundLength = bodyDistanceToGround;

            originalRayToGroundLength = value;
        }
    }

    /// <summary>
    /// To also apply changes use the property AutosetRayToGroundLength
    /// </summary>
    public bool autosetRayToGroundLength;
    public bool AutosetRayToGroundLength
    {
        get => autosetRayToGroundLength;
        set
        {
            autosetRayToGroundLength = value;
            RayToGroundLength = mainBoneController.bodyDistanceToGround * 2;
        }
    }

    /// <summary>
    /// To also apply changes use the property AutosetBodyDistanceToGround
    /// </summary>
    public bool autosetBodyDistanceToGround;
    public bool AutosetBodyDistanceToGround
    {
        get => autosetBodyDistanceToGround;
        set
        {
            autosetBodyDistanceToGround = value;
            bodyDistanceToGround = Mathf.Abs((LegsCenter).y - (transform.position).y);
        }
    }

    public float bodyDistanceToGround;

    public AnimationCurve multiplierlegsDistanceOnVPosCurveVI;

    public float bodyStabilizationTopBottomFoot;

    public PL2D_AxialBone mainBoneController;

    public enum PlayerControllerEnum { KeyboardInput, PathFollow };
    public PlayerControllerEnum playerControllerType;

    public enum AngleSolverEnum { Raycast, RelativeToLegs, KeepVertical };
    public AngleSolverEnum angleSolverType;
    public float angleTrackSpeed;

    /// <summary>
    /// Readonly legs center based on settings
    /// </summary>
    /// <value></value>
    public Vector3 LegsCenter
    {
        get
        {
            if (legs.Count > 0)
            {
                return (BottomFoot().limbTarget.transform.position * bodyStabilizationTopBottomFoot) + (TopFoot().limbTarget.transform.position * (1 - bodyStabilizationTopBottomFoot));
            }
            else
                return new Vector3(0,0,0);
        }
    }

    Vector3 lastFrameBodyPosition;
    bool reinitAngleSolver = true;

    public RaycastHit2D axialBoneHit;

    /// <summary>
    /// Direction of the body movement
    /// </summary>
    /// <value></value>
    public Vector3 MoveDirection
    {
        get
        {
            return (transform.right * bodyVelocity).normalized;
        }
    }

    public PL2D_PlayerController pl2d_PlayerController;
    public PL2D_AngleSolver pl2d_AngleSolver;

    /// <summary>
    /// To also apply changes use the property (Recommended)PlayerControllerEnabled
    /// </summary>
    public bool playerControllerEnabled;
    public bool PlayerControllerEnabled
    {
        get => mainBoneController.playerControllerEnabled;
        set
        {
            mainBoneController.playerControllerEnabled = value;
            if (value)
            {
                if (mainBoneController.pl2d_PlayerController == null)
                {
                    mainBoneController.pl2d_PlayerController = mainBoneController.GetComponent<PL2D_PlayerController>();
                    PL2D_PlayerController.Initialize(mainBoneController);
                }

                mainBoneController.pl2d_PlayerController.enabled = true;
            }
            else
            {
                if (mainBoneController.pl2d_PlayerController != null)
                    mainBoneController.pl2d_PlayerController.enabled = false;
            }
        }
    }

    /// <summary>
    /// To also apply changes use the property AngleSolverEnabled
    /// </summary>
    public bool angleSolverEnabled;
    public bool AngleSolverEnabled
    {
        get => angleSolverEnabled;
        set
        {
            angleSolverEnabled = value;
            if (angleSolverEnabled)
            {
                PL2D_AngleSolver.Initialize(this);
            }
            else
            {
                if (pl2d_AngleSolver)
                    pl2d_AngleSolver.enabled = false;
            }
        }
    }

    public bool isGrounded;
    public bool isJumping = false;

    bool isGroundedFirstFrame = true;
    float fallingCount;
    Vector3 rayDirection;
    int previousLeg;
    float lastAngleBeforeMove;
    Vector3 lastPosBeforeMove;

    private void OnValidate()
    {
        if (legs != null)
        {
            foreach (PL2D_Leg leg in legs)
            {
                leg.pl2d_AxialBone = this;
                leg.limbCenter.SetParent(transform);

                if (!leg.restPosition)
                {
                    leg.restPosition = new GameObject("RestPosision " + PL2D_Animator.GetShortName(leg.name)).transform;
                    leg.restPosition.position = leg.limbTarget.position - (transform.up * 2);
                    leg.restPosition.SetParent(transform);
                }
            }
        }
    }

    void Start()
    {
        originalRayToGroundLength = rayToGroundLength;

        isMainAxialBone = PL2D_Animator.GetIndexFromName(name) == 0 ? true : false;

        if (isMainAxialBone)
        {
            foreach (PL2D_AxialBone bone in pL2dAnimator.axialBones)
            {
                bone.mainBoneController = this;
                bone.pl2d_PlayerController = pl2d_PlayerController;
            }
        }

        lastPosBeforeMove = transform.position;
        lastAngleBeforeMove = transform.eulerAngles.z;

        lastFrameBodyRotation = transform.rotation;

        if (isMainAxialBone)
        {
            if (pl2d_PlayerController)
            {
                pl2d_PlayerController.rig2d = GetComponent<Rigidbody2D>();
                pl2d_PlayerController.rig2d.bodyType = RigidbodyType2D.Dynamic;
            }

            if (autosetBodyDistanceToGround)
            {
                bodyDistanceToGround = Mathf.Abs((LegsCenter).y - (transform.position).y);
            }
        }

        PL2D_Animator.layerMask = LayerMask.GetMask("Ground");

        lastFrameBodyPosition = transform.localPosition;

        isMoving = false;
        actualLeg = 0;

        foreach (PL2D_Leg leg in legs)
        {
            leg.limbTarget.position = leg.restPosition.position;
        }

        isGrounded = true;

        lastAngleBeforeMove = transform.eulerAngles.z;

        PL2D_AngleSolver.Initialize(this);
    }

    public float originalRayToGroundLength;

    /// <summary>
    /// This method adjusts legs position and body settings for is or is not grounded states 
    /// </summary>
    /// <param name="value"></param>
    public void SetIsGrounded(bool value)
    {
        if (value == true)
        {
            if (pl2d_PlayerController)
            {
                pl2d_PlayerController.rig2d.gravityScale = 0;
                pl2d_PlayerController.rig2d.velocity = new Vector3(0,0,0);
                pl2d_PlayerController.rig2d.angularVelocity = 0;
            }

            if (isGroundedFirstFrame)
            {
                rayToGroundLength = originalRayToGroundLength;

                if (pl2d_PlayerController)
                {
                    if (playerControllerEnabled)
                    {
                        pl2d_PlayerController.positionTarget.position = axialBoneHit.point;
                        pl2d_PlayerController.positionTarget.up = transform.up;
                    }
                }

                foreach (PL2D_AxialBone axialBone in pL2dAnimator.axialBones)
                {
                    foreach (PL2D_Leg leg in axialBone.legs)
                    {
                        RaycastHit2D flipHit = Physics2D.Raycast(leg.transform.position, -transform.up, 100, PL2D_Animator.layerMask);
                        // v1.1 - Fix: leg glitch on Grounded
                        if (flipHit.transform)
                            StartCoroutine(leg.LerpLegToPosition(flipHit.point));
                    }
                }

                // v1.1 - Fix: isJump not being true until jump end
                if (isJumping) isJumping = false;

                isGroundedFirstFrame = false;

                pL2dAnimator.transform.GetComponentInParent<IKManager2D>().solvers[0].solveFromDefaultPose = true;
            }
        }
        else
        {
            if (playerControllerEnabled)
            {
                rayToGroundLength = originalRayToGroundLength / 2f;
                pl2d_PlayerController.rig2d.gravityScale = 1;
            }

            if (pl2d_PlayerController)
            {
                pl2d_PlayerController.positionTarget.SetParent(pL2dAnimator.transform);
            }

            isGroundedFirstFrame = true;
        }

        isGrounded = value;
    }

    void FixedUpdate()
    {
        // calculate body velocity
        Vector3 forward = transform.TransformDirection(Vector3.right);
        Vector3 toOther = transform.position - lastFrameBodyPosition;
        bodyVelocity = Vector3.Dot(forward, toOther) / Time.deltaTime;

        // calculate the angular velocity
        Quaternion deltaBodyRotation = transform.rotation * Quaternion.Inverse(lastFrameBodyRotation);
        deltaBodyRotation.ToAngleAxis(out angVelocityMagnitude, out angVelocityAxis);

        // previous frame variables
        lastFrameBodyPosition = transform.position;
        lastFrameBodyRotation = transform.rotation;
        lastFrameBodyVelocity = bodyVelocity;
    }

    /// <summary>
    /// Return true if all legs are grounded 
    /// </summary>
    /// <value></value>
    public bool LegsGrounded
    {
        get
        {
            bool legsGrounded = false;
            foreach (PL2D_Leg leg in legs)
            {
                if (leg.footHit.transform)
                    legsGrounded = true;
            }
            return legsGrounded;
        }
    }

    void Update()
    {
        rayDirection = -transform.up;

        axialBoneHit = Physics2D.Raycast(transform.position, rayDirection, mainBoneController.rayToGroundLength, PL2D_Animator.layerMask);
        Debug.DrawRay(transform.position, rayDirection * mainBoneController.rayToGroundLength, Color.green);

        // v1.1 - Improved ground check with legs grounded check
        if (axialBoneHit.transform != null)
            SetIsGrounded(true);
        if (axialBoneHit.transform == null && LegsGrounded == false)
            SetIsGrounded(false);

        if (pL2dAnimator.enableAanimation)
        {
            if (isGrounded && !isJumping)
            {
                if (Vector3.Distance(lastPosBeforeMove, transform.position) > distanceBeforeMoveLeg || Mathf.Abs(lastAngleBeforeMove - transform.eulerAngles.z) > angleBeforeMoveLeg)
                {
                    isMoving = true;

                    lastPosBeforeMove = transform.position;
                    lastAngleBeforeMove = transform.eulerAngles.z;

                    if (!legs[actualLeg].isMoving && pl2d_PlayerController.bodySpeed != 0)
                    {
                        StartCoroutine(legs[actualLeg].MoveLeg());
                    }
                }
                else
                {
                    isMoving = false;
                }
            }
        }

        if (!isMainAxialBone)
        {
            bodyVelocity = mainBoneController.bodyVelocity;
        }

        if (reinitAngleSolver)
        {
            PL2D_AngleSolver pl2d_AngleSolver = GetComponent<PL2D_AngleSolver>();
            if (pl2d_AngleSolver != null)
            {
                if (pl2d_AngleSolver.isActiveAndEnabled)
                {
                    pl2d_AngleSolver.enabled = false;
                    pl2d_AngleSolver.enabled = true;
                }
            }
            reinitAngleSolver = false;
        }
    }

    /// <summary>
    /// Get most bottom foot
    /// </summary>
    /// <returns></returns>
    public PL2D_Leg BottomFoot()
    {
        PL2D_Leg bottomFoot = legs[0];
        int _legsCount = legs.Count;
        for (int i = 1; i < _legsCount; i++)
        {
            if (RelativePoint(legs[i].limbTarget.position).y < RelativePoint(bottomFoot.limbTarget.position).y)
                bottomFoot = legs[i];
        }
        return bottomFoot;
    }

    /// <summary>
    /// Get most top foot
    /// </summary>
    /// <returns></returns>
    PL2D_Leg TopFoot()
    {
        PL2D_Leg topFoot = legs[0];
        int _legsCount = legs.Count;
        for (int i = 1; i < _legsCount; i++)
        {
            if (RelativePoint(legs[i].limbTarget.position).y > RelativePoint(topFoot.limbTarget.position).y)
                topFoot = legs[i];
        }
        return topFoot;
    }

    /// <summary>
    /// Get most back foot
    /// </summary>
    /// <returns></returns>
    PL2D_Leg BackFoot()
    {
        PL2D_Leg backFoot = legs[0];
        int _legsCount = legs.Count;
        for (int i = 1; i < _legsCount; i++)
        {

            if (bodyVelocity > 0)
            {
                if (RelativePoint(legs[i].limbTarget.position).x < RelativePoint(backFoot.limbTarget.position).x)
                    backFoot = legs[i];
            }
            else
            {
                if (RelativePoint(legs[i].limbTarget.position).x > RelativePoint(backFoot.limbTarget.position).x)
                    backFoot = legs[i];
            }
        }
        return backFoot;
    }

    /// <summary>
    /// Get most front foot
    /// </summary>
    /// <returns></returns>
    PL2D_Leg FrontFoot()
    {
        PL2D_Leg frontFoot = legs[0];
        int _legsCount = legs.Count;
        for (int i = 1; i < _legsCount; i++)
        {

            if (bodyVelocity > 0)
            {
                if (RelativePoint(legs[i].limbTarget.position).x > RelativePoint(frontFoot.limbTarget.position).x)
                    frontFoot = legs[i];
            }
            else
            {
                if (RelativePoint(legs[i].limbTarget.position).x < RelativePoint(frontFoot.limbTarget.position).x)
                    frontFoot = legs[i];
            }
        }
        return frontFoot;
    }

    public Vector3 RelativePoint(Vector3 point)
    {
        return transform.InverseTransformPoint(point);
    }
}
