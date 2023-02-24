using System;
using MathUtility;
using Unity.Mathematics;
using UnityEngine;

public class TreePaddleController : MonoBehaviour
{

    private Transform baseTransform;
    private Region region;
    private Rigidbody2D baseRB;
    private DragTreePosition treeActive;
    private TreeBend treeBender;
    private Vector3 startPos;
    private Vector3 topStartScale;


    private float startRotation = 0f;

    [SerializeField] private float goalRotation = 0f;
    [SerializeField] private float launchRotation = 0f;
    [SerializeField] private float wobbleGoalRotation = 0f;

    [SerializeField] private float currentGoalPercent = 0f;

    [SerializeField] private float jumpPercent = 0;


    private float wobbleAngleVelocity = 0.0f;

    private float bendVelocity = 0.0f;
    private float jumpVelocity = 0.0f;
    private float stretchVelocity = 0.0f;


    public bool doDebug = false;


    private void Start()
    {
        region = GetComponentInParent<Region>();
        treeActive = FindObjectOfType<DragTreePosition>();
        treeBender = FindObjectOfType<TreeBend>();
        baseTransform = transform.parent;
        startPos = baseTransform.localPosition;
        topStartScale = transform.localScale;
        baseRB = baseTransform.GetComponent<Rigidbody2D>();
        startRotation = goalRotation = baseRB.rotation;
    }

    private void FixedUpdate()
    {
        //float deltaOrigGoal = Mathf.DeltaAngle(baseRB.rotation, goalRotation);

        wobbleGoalRotation = goalRotation + wobbleAngleVelocity;

        // delta from current towards wobbleGoal

        // rotation towards wobbleGoal
        float newRot = Mathf.SmoothDampAngle(
            baseRB.rotation,
            wobbleGoalRotation,
            ref bendVelocity,
            treeBender.bendTime,
            treeBender.maxSpeed,
            Time.fixedDeltaTime);

        float deltaWobbleGoal = Mathf.DeltaAngle(newRot, wobbleGoalRotation);
        int deltaWobbleDir = Math.Sign(deltaWobbleGoal);
        if ((deltaWobbleDir == 1 && newRot > goalRotation)
            || (deltaWobbleDir == -1 && newRot < goalRotation))
            jumpPercent = 0;


        if (Math.Abs(newRot - wobbleGoalRotation) < 0.1f)
        {
            // has reached wobble end position
            if (Mathf.Abs(wobbleAngleVelocity) < 0.01f)
                wobbleAngleVelocity = 0f;
            else
            {
                wobbleAngleVelocity = wobbleAngleVelocity * treeBender.wobbleFactor;
                wobbleAngleVelocity = -wobbleAngleVelocity;
            }

            // mark Jump's end
            jumpPercent = 0;
        }



        //// is tree moving towards wobbleGoal, or Goal

        //if ((goalDeltaSign == 1f && newRot > goalRotation)
        //    || (goalDeltaSign == -1f && newRot < goalRotation)
        //    || goalDeltaSign == 0f)
        //{
        //    // has rotated past goal
        //    jumpPercent = 0;

        //    if (Mathf.Approximately(0f, wobbleAngleVelocity))
        //        wobbleAngleVelocity = 0f;
        //    else if (goalDeltaSign == 0f)
        //    {
        //        wobbleAngleVelocity = -wobbleAngleVelocity;
        //        wobbleAngleVelocity =
        //            Mathf.Lerp(wobbleAngleVelocity, 0f, 0.75f);
        //        // Mathf.SmoothDamp(wobbleAngleVelocity, 0f, ref wobbleVelocity, treeBender.wobbleTime);
        //    }

        //}




        // Jump and Stretch
        float t = MathU.InverseLerpAngle(startRotation, launchRotation, newRot);
        float yPercent = (treeBender.PopOutHeightCurve.Evaluate(t) * jumpPercent);

        float yMove = yPercent * treeBender.jumpHeight + startPos.y;
        Vector3 newPos = startPos;
        newPos.y = Mathf.SmoothDamp(newPos.y, yMove, ref jumpVelocity, treeBender.jumpTime);
        baseTransform.localPosition = newPos;

        float yScale = yPercent * treeBender.stretchHeight + topStartScale.y;
        Vector3 newScale = topStartScale;
        newScale.y = Mathf.SmoothDamp(newScale.y, yScale, ref stretchVelocity, treeBender.stretchTime);
        transform.localScale = newScale;


        // baseRB.MovePosition(baseTransform.TransformPoint(newPos));

        //float spd = treeBender.rotationSpeed * Time.fixedDeltaTime;
        //newRot = Mathf.MoveTowardsAngle(
        //    CurrentRotation, 
        //    goalRotation, 
        //    spd);

        baseRB.MoveRotation(newRot);

    }

    public void SetTreeState(float collapsePercent, Vector3 benderPos)
    {
        //if (currentGoalPercent > (1f - treeBender.deadRange)
        //    && collapsePercent > treeBender.deadRange) return;

        goalRotation = startRotation + CalcLocalRotation(collapsePercent, benderPos);
        if (collapsePercent < currentGoalPercent)
        {   // Get new goal that is more up-right than previous goal
            // Jump up
            jumpPercent = Mathf.Abs(collapsePercent - currentGoalPercent);
            launchRotation = baseRB.rotation;

            wobbleAngleVelocity = jumpPercent * treeBender.wobbleStrength;
        }
        else
        {
            wobbleAngleVelocity = 0f;
        }

        currentGoalPercent = collapsePercent;
    }

    private float CalcLocalRotation(float collapsePercent, Vector3 benderPos)
    {
        Vector2 newDirection = Vector2.down;

        float distance = WheelRegionsManager.RegionDistanceDelta(
            region.RegionIndex + 0.5f,
            region.WorldToRegionDistance(benderPos));
        //Vector3 relativePoint = region.transform.InverseTransformPoint(benderPos);

        if (distance > 0f)
            newDirection.y = -1; // Right side
        else if (distance < 0f)
            newDirection.y = 1; // Left side

        float angle = MathU.Vector2ToDegree(newDirection);

        if (doDebug)
            Debug.Log("distance: " + distance);

        angle *= collapsePercent;
        return angle;
    }

    public void Reset()
    {
        currentGoalPercent = 0f;
        // jump up
        launchRotation = baseRB.rotation;
        float delta = MathU.DeltaRange(launchRotation, startRotation, 360f);
        jumpPercent = Mathf.Abs(delta / 180f);

        wobbleAngleVelocity = jumpPercent * treeBender.wobbleStrength;
        //jumpPercent = MathU.InverseLerpAngle(startRotation, jumpFromRotation, baseRB.rotation);

        goalRotation = startRotation;
        //jumpFromRotation = startRotation + 180;

    }

    private void OnDrawGizmos()
    {
        if (!doDebug) return;

    }



    //only thing that changes is how much time it has to get to that y height if tree half bent
    //then its only got half the amount of time to return
}