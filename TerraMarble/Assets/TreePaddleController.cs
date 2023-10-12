using MathUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TreePaddleController : MonoBehaviour
{
    // References
    private Transform treeStump;
    private Region treeRegion; // ground tile
    private Rigidbody2D treeRigidbody; // rigidbody2d
    private TreeBend treeBender; // user controller
    private PolygonCollider2D treeCollider; // base of paddle

    // Start State
    private Vector3 treeBaseStartPosition;
    private Vector3 treePaddleStartScale;

    // Data
    [Header("Data")] [SerializeField] private float startRotation;
    [SerializeField] private float benderGoalRotation = 0f;

    private float currentRotation
    {
        get => treeRigidbody.rotation;
        set => treeRigidbody.rotation = value;
    }

    // Wobble feature
    [SerializeField] private float currentGoalRotation = 0f;
    [SerializeField] private float wobbleAngleVelocity = 0f;

    // Launch on Release feature
    [SerializeField] private float launchPowerPercent = 0;
    [SerializeField] private float launchProgress = 1f;

    [SerializeField] private float collisionAngleDesired = 0f;
    [SerializeField] private int collisionBallSideDirection = 1; // Left is 1, Right is -1.
    [SerializeField] private float collisionAngleCurrent = 0f;
    private float collisionAngleVelocity = 0f;

    private float bendVelocity = 0.0f;

    private float launchVelocity = 0.0f;
    private float stretchVelocity = 0.0f;
    private float jumpVelocity = 0.0f;

    public bool doDebug = false;
    private float treeHeightLocal;

    /// World Space
    public float TreeHeight
        => transform.lossyScale.y * treeHeightLocal;

    public Vector2 TreeTipVector
        => transform.up * (transform.lossyScale.y * treeHeightLocal);

    public Vector2 TreeTipPosition
        => transform.position.To2DXY() + TreeTipVector;

    public Region TreeRegion => treeRegion;


    private void Start()
    {
        treeRegion = GetComponentInParent<Region>();
        treeBender = FindObjectOfType<TreeBend>(true);
        treeStump = transform.parent;
        treeRigidbody = treeStump.GetComponent<Rigidbody2D>();
        treeCollider = GetComponent<PolygonCollider2D>();

        treeBaseStartPosition = treeStump.localPosition;
        treePaddleStartScale = transform.localScale;
        startRotation = benderGoalRotation = currentGoalRotation
            = currentRotation;

        launchProgress = 1f;

        treeRigidbody.mass = 1f;
        treeRigidbody.inertia = 1f;

        // Get tree tip's point height
        treeHeightLocal = treeCollider.points.Max(point => point.y);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ProcessCollision(collision);
    }

    private void ProcessCollision(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Ball"))
            return;

        List<ContactPoint2D> contactPoint2Ds = new List<ContactPoint2D>();
        collision.GetContacts(contactPoint2Ds);
        foreach (ContactPoint2D contact in contactPoint2Ds)
        {
            Vector2 towardsDir = treeRigidbody.transform.position.To2DXY()
                .Towards(contact.collider.transform.position.To2DXY())
                .normalized;
            Vector2 leftDir = -treeRigidbody.transform.right.To2DXY();

            //Debug.Log(string.Format("{0}: {1:0.00}",
            //    treeRegion.name, towardsDir.Dot(leftDir)));

            // Left is 1f, Right is -1f.
            int treeSideSign = Math.Sign(towardsDir.Dot(leftDir));
            if (treeSideSign < 1) treeSideSign = -1;
            else treeSideSign = 1;

            collisionBallSideDirection = treeSideSign;
            Vector2 forceDirection = TreeRegion.Base.right.To2DXY() * treeSideSign;

            float forcePower = Mathf.Clamp(
                contact.relativeVelocity.magnitude * treeBender.collisionSensitivity,
                treeBender.collisionForceMin,
                treeBender.collisionForceMax
            );

            float torque = ForceToTorque(treeRigidbody,
                forceDirection * forcePower,
                contact.point,
                ForceMode2D.Impulse);

            // Apply rotation based on torque and adjust for rotation speed factor
            float rotationDelta = torque * treeBender.collisionSensitivity;

            // restrict to Max Angle
            if (Mathf.Abs(rotationDelta) > treeBender.collisionAngleMax)
                rotationDelta = treeBender.collisionAngleMax * Mathf.Sign(rotationDelta);

            // Store the largest result
            if (Mathf.Abs(collisionAngleDesired) < Mathf.Abs(rotationDelta))
                collisionAngleDesired = rotationDelta;
        }
    }

    private void FixedUpdate()
    {
        if (treeRigidbody == null) return;

        UpdateBend();
        // reset collisionDelta for the current FixedUpdate
        collisionAngleDesired = 0f;
    }

    // Calculates the Torque that would be created by RigidBody2D.AddForceAtPosition
    public static float ForceToTorque(Rigidbody2D rb2D, Vector2 force, Vector2 worldPosition,
        ForceMode2D forceMode = ForceMode2D.Force)
    {
        if (Mathf.Approximately(0f, force.sqrMagnitude))
            return 0f;

        // Vector from the force position to the CoM
        Vector2 p = rb2D.worldCenterOfMass - worldPosition;

        // Get the angle between the force and the vector from position to CoM
        float angle = Mathf.Atan2(p.y, p.x) - Mathf.Atan2(force.y, force.x);

        // This is basically like Vector3.Cross, but in 2D, hence giving just a scalar value instead of a Vector3
        float t = p.magnitude * force.magnitude * Mathf.Sin(angle) * Mathf.Rad2Deg;

        // Continuous force
        if (forceMode == ForceMode2D.Force) t *= Time.fixedDeltaTime;

        // Apply inertia
        return t / rb2D.inertia;
    }

    private void UpdateBend()
    {
        if (!Mathf.Approximately(collisionAngleCurrent, collisionAngleDesired))
        {
            float delta = Mathf.DeltaAngle(collisionAngleCurrent, collisionAngleDesired);
            int deltaSign = Math.Sign(delta);
            deltaSign = deltaSign != 0 ? deltaSign : 1;

            float collisionSpeed = collisionBallSideDirection == deltaSign
                ? treeBender.collisionSpeedDecrease
                : treeBender.collisionSpeedIncrease;

            collisionAngleCurrent = Mathf.SmoothDampAngle(collisionAngleCurrent, collisionAngleDesired,
                ref collisionAngleVelocity, collisionSpeed, treeBender.bendMaxSpeed, Time.fixedDeltaTime);
        }

        currentGoalRotation = benderGoalRotation + wobbleAngleVelocity + collisionAngleCurrent;

        if (!Mathf.Approximately(currentRotation, currentGoalRotation))
        {
            float newCurrentRotation = Mathf.SmoothDampAngle(currentRotation, currentGoalRotation,
                ref bendVelocity, treeBender.bendTime, treeBender.bendMaxSpeed, Time.fixedDeltaTime);

            treeRigidbody.MoveRotation(newCurrentRotation);
        }

        UpdateLaunch();

        if (!Mathf.Approximately(0f, wobbleAngleVelocity))
            UpdateWobble();
    }

    private void UpdateLaunch()
    {
        if (!Mathf.Approximately(1f, launchProgress))
            launchProgress = Mathf.SmoothDamp(
                launchProgress, 1f, ref launchVelocity,
                treeBender.launchTime, treeBender.bendMaxSpeed, Time.fixedDeltaTime);

        // Jump and Stretch
        Vector3 newPos = treeStump.localPosition;
        Vector3 newScale = transform.localScale;

        // curve = 0..1..0;
        float launchCurveT = treeBender.launchCurve.Evaluate(launchProgress);
        float goalPosY = treeBaseStartPosition.y + treeBender.jumpHeight * launchPowerPercent * launchCurveT;
        float goalScaleY = treePaddleStartScale.y + treeBender.stretchHeight * launchPowerPercent * launchCurveT;

        if (!Mathf.Approximately(newPos.y, goalPosY))
            newPos.y = Mathf.SmoothDamp(newPos.y, goalPosY, ref jumpVelocity,
                treeBender.launchTime, treeBender.bendMaxSpeed, Time.fixedDeltaTime);

        if (!Mathf.Approximately(newScale.y, goalScaleY))
            newScale.y = Mathf.SmoothDamp(newScale.y, goalScaleY, ref stretchVelocity,
                treeBender.launchTime, treeBender.bendMaxSpeed, Time.fixedDeltaTime);

        treeStump.localPosition = newPos;
        transform.localScale = newScale;
    }

    private void UpdateWobble()
    {
        // if velocity is close to 0, snap to 0.
        if (Mathf.Abs(wobbleAngleVelocity) < treeBender.wobbleMinThreshold)
        {
            wobbleAngleVelocity = 0f;
        }
        else
        {
            // if reached wobble, then reduce and flip wobble
            float deltaAngle = Mathf.DeltaAngle(currentRotation, currentGoalRotation);
            if (Mathf.Abs(deltaAngle) < treeBender.wobbleMinThreshold)
                wobbleAngleVelocity = -wobbleAngleVelocity * treeBender.wobbleSlowFactor;
        }
    }

    public void SetTreeState(float bendPercent, int direction)
    {
        // set new goal
        if (direction == 0) direction = 1;

        float newLocalRotation = treeBender.bendMaxAngle * direction * bendPercent;

        benderGoalRotation = (startRotation + newLocalRotation).ClampToDegrees();

        // reset Wobble
        wobbleAngleVelocity = 0f;
    }

    private float CalcCurrentBentPercent()
    {
        // calc strength based on current rotation
        float delta = MathU.DeltaRange(currentRotation, startRotation, 360f);
        float powerPercent = Mathf.Abs(delta / 180f) * 2f;
        return powerPercent;
    }

    private void InitWobble(float bentPercent)
    {
        wobbleAngleVelocity = bentPercent * treeBender.wobbleMaxAngle;
    }

    private void InitLaunch(float bentPercent)
    {
        launchPowerPercent = bentPercent;
        launchProgress = 0f;
    }

    public void Release()
    {
        float bentPercent = CalcCurrentBentPercent();
        InitWobble(bentPercent);
        InitLaunch(bentPercent);
        // Set Goal
        benderGoalRotation = startRotation;
    }

    public int DirectionFromPoint(Vector2 worldPos)
    {
        float towardsPointDelta = TreeRegion.RegionsMan.RegionDistanceDelta(
            TreeRegion.RegionIndex + 0.5f,
            TreeRegion.WorldToRegionDistance(worldPos));

        if (towardsPointDelta > 0f) // Right side
            return -1;
        else // Left side
            return 1;
    }

    private void OnDrawGizmos()
    {
        if (!doDebug) return;
        Vector3 flipAxis = ((Vector2)TreeRegion.Wheel.transform.position).Towards(TreeRegion.Base.transform.position)
            .normalized;
        Gizmos.DrawRay(TreeRegion.Base.transform.position, flipAxis);
    }


    //private void UpdateFlip()
    //{
    //    Quaternion currentZ = Quaternion.AngleAxis(currentRotation, Vector3.forward);

    //    flipCurrent = Mathf.MoveTowards(flipCurrent, flipGoal, Time.fixedDeltaTime);
    //    flipAxis = ((Vector2)region.Wheel.transform.position).Towards(region.Base.transform.position).normalized;
    //    Quaternion currentFlip = Quaternion.AngleAxis(flipCurrent * flipAmount, flipAxis);


    //    Quaternion newRot = MathU.SmoothDampRotation(baseTransform.rotation, currentFlip * currentZ, ref flipVelocity,
    //        treeBender.flipTime, treeBender.bendMaxSpeed, Time.fixedDeltaTime);

    //    baseTransform.rotation = newRot;

    //    baseRB.MoveRotation(currentRotation);
    //}
}