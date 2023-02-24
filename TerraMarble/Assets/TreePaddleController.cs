using System;
using MathUtility;
using Unity.Mathematics;
using UnityEngine;

public class TreePaddleController : MonoBehaviour
{
    // References
    private Transform baseTransform;
    private Region region;
    private Rigidbody2D baseRB;
    private TreeBend treeBender;

    // Start State
    private Vector3 startPos;
    private Vector3 topStartScale;

    // Data
    [SerializeField] private float startRotation;
    [SerializeField] private float launchStartRotation;
    [SerializeField] private float goalRotation = 0f;
    private float currentRotation { get => baseRB.rotation; set => baseRB.rotation = value; }


    //[SerializeField] private float launchEndPercent = 1f;

    [SerializeField] private float wobbleGoalRotation = 0f;
    [SerializeField] private float wobbleAngleVelocity = 0f;

    [SerializeField] private float launchPowerPercent = 0;
    [SerializeField] private float launchProgress = 1f;


    private float bendVelocity = 0.0f;
    private float launchVelocity = 0.0f;


    public bool doDebug = false;


    private void Start()
    {
        region = GetComponentInParent<Region>();
        treeBender = FindObjectOfType<TreeBend>(true);
        baseTransform = transform.parent;
        baseRB = baseTransform.GetComponent<Rigidbody2D>();

        startPos = baseTransform.localPosition;
        topStartScale = transform.localScale;
        startRotation = launchStartRotation = goalRotation = wobbleGoalRotation
            = currentRotation;

        //launchEndPercent = 1f;
        launchProgress = 1f;
    }

    private void FixedUpdate()
    {
        UpdateBend();
    }

    void StartBend() { }

    void UpdateBend()
    {
        wobbleGoalRotation = goalRotation + wobbleAngleVelocity;

        float newCurrentRotation = Mathf.SmoothDampAngle(
            currentRotation, wobbleGoalRotation,
            ref bendVelocity, treeBender.bendTime,
            treeBender.bendMaxSpeed, Time.fixedDeltaTime);

        baseRB.MoveRotation(newCurrentRotation);

        UpdateLaunch();

        UpdateWobble();
    }

    void UpdateWobble()
    {
        // if velocity is close to 0, snap to 0.
        if (Mathf.Abs(wobbleAngleVelocity) < treeBender.wobbleMinThreshold)
            wobbleAngleVelocity = 0f;
        else
        {
            // if reached wobble, then reduce and flip wobble
            float deltaAngle = Mathf.DeltaAngle(currentRotation, wobbleGoalRotation);
            if (Mathf.Abs(deltaAngle) < treeBender.wobbleMinThreshold)
                wobbleAngleVelocity = -wobbleAngleVelocity * treeBender.wobbleSlowFactor;
        }
    }

    void UpdateLaunch()
    {
        if (launchProgress >= 1f) return;

        launchProgress = Mathf.SmoothDamp(
            launchProgress, 1f,
            ref launchVelocity, treeBender.launchTime,
            treeBender.bendMaxSpeed, Time.fixedDeltaTime);

        //launchProgress = MathU.InverseLerpAngle(
        //    launchStartRotation, goalRotation, currentRotation);

        // curve = 0..1..0;
        float launchCurveT = treeBender.launchCurve.Evaluate(launchProgress);
        Vector3 newPos = baseTransform.localPosition;
        Vector3 newScale = topStartScale;

        //update Jump
        newPos.y = Mathf.Lerp(
            startPos.y,
            startPos.y + treeBender.jumpHeight * launchPowerPercent,
            launchCurveT);

        //update Stretch
        newScale.y = Mathf.Lerp(
            topStartScale.y,
            topStartScale.y + treeBender.stretchHeight * launchPowerPercent,
            launchCurveT);

        baseTransform.localPosition = newPos;
        transform.localScale = newScale;
    }

    void SetWobble(float _wobbleVelocity)
    {
        wobbleAngleVelocity = _wobbleVelocity;
    }

    public void SetTreeState(float upPercent, Vector3 benderPos)
    {
        // set new goal
        goalRotation = startRotation + CalcLocalRotation(upPercent, benderPos);
        SetWobble(0f);

        //if (upPercent > launchEndPercent)
        //{ // bending upwards

        //    // percent from current rotation to new goal
        //    float currentLaunchPercent
        //        = 1f - Mathf.Abs(upPercent - launchEndPercent);
        //    SetLaunch(currentLaunchPercent); // also prepares wobble
        //}
        //else
        //{ // bending downwards
        //    // clear wobble
        //    SetWobble(0f);
        //}

        //launchEndPercent = upPercent;
    }

    void SetLaunch()
    {
        launchStartRotation = currentRotation;

        float delta = MathU.DeltaRange(launchStartRotation, startRotation, 360f);
        float powerPercent = Mathf.Abs(delta / 180f) * 2f;
        launchPowerPercent = powerPercent;

        launchProgress = 0f;
        //launchEndPercent = 1f;

        // prepare for wobble
        SetWobble(launchPowerPercent * treeBender.wobbleMaxAngle);
    }

    public void Release()
    {
        // launch

        SetLaunch();


        // Goal
        goalRotation = startRotation;

    }

    private float CalcLocalRotation(float upPercent, Vector3 benderPos)
    {
        Vector2 newDirection = Vector2.down;

        float distance = WheelRegionsManager.RegionDistanceDelta(
            region.RegionIndex + 0.5f,
            region.WorldToRegionDistance(benderPos));

        if (distance > 0f)
            newDirection.y = -1; // Right side
        else if (distance < 0f)
            newDirection.y = 1; // Left side

        float angle = MathU.Vector2ToDegree(newDirection);

        if (doDebug)
            Debug.Log("distance: " + distance);

        angle *= 1f - upPercent;
        return angle;
    }



    //  private void FixedUpdate()
    //  {
    //      //float deltaOrigGoal = Mathf.DeltaAngle(baseRB.rotation, goalRotation);
    //  
    //      wobbleGoalRotation = goalRotation + wobbleAngle;
    //  
    //      // delta from current towards wobbleGoal
    //  
    //      // rotation towards wobbleGoal
    //      float newRot = Mathf.SmoothDampAngle(
    //          baseRB.rotation,
    //          wobbleGoalRotation,
    //          ref bendVelocity,
    //          treeBender.bendTime,
    //          treeBender.bendMaxSpeed,
    //          Time.fixedDeltaTime);
    //  
    //      float deltaWobbleGoal = Mathf.DeltaAngle(newRot, wobbleGoalRotation);
    //      int deltaWobbleDir = Math.Sign(deltaWobbleGoal);
    //      if ((deltaWobbleDir == 1 && newRot > goalRotation)
    //          || (deltaWobbleDir == -1 && newRot < goalRotation))
    //          launchPercent = 0;
    //  
    //  
    //      if (Math.Abs(newRot - wobbleGoalRotation) < 0.1f)
    //      {
    //          // has reached wobble end position
    //          if (Mathf.Abs(wobbleAngle) < 0.01f)
    //              wobbleAngle = 0f;
    //          else
    //          {
    //              wobbleAngle = wobbleAngle * treeBender.wobbleFactor;
    //              wobbleAngle = -wobbleAngle;
    //          }
    //  
    //          // mark Jump's end
    //          launchPercent = 0;
    //      }
    //  
    //  
    //  
    //      //// is tree moving towards wobbleGoal, or Goal
    //  
    //      //if ((goalDeltaSign == 1f && newRot > goalRotation)
    //      //    || (goalDeltaSign == -1f && newRot < goalRotation)
    //      //    || goalDeltaSign == 0f)
    //      //{
    //      //    // has rotated past goal
    //      //    jumpPercent = 0;
    //  
    //      //    if (Mathf.Approximately(0f, wobbleAngleVelocity))
    //      //        wobbleAngleVelocity = 0f;
    //      //    else if (goalDeltaSign == 0f)
    //      //    {
    //      //        wobbleAngleVelocity = -wobbleAngleVelocity;
    //      //        wobbleAngleVelocity =
    //      //            Mathf.Lerp(wobbleAngleVelocity, 0f, 0.75f);
    //      //        // Mathf.SmoothDamp(wobbleAngleVelocity, 0f, ref wobbleVelocity, treeBender.wobbleTime);
    //      //    }
    //  
    //      //}
    //  
    //  
    //  
    //  
    //      // Jump and Stretch
    //      float t = MathU.InverseLerpAngle(startRotation, launchRotation, newRot);
    //      float yPercent = (treeBender.launchCurve.Evaluate(t) * launchPercent);
    //  
    //      float yMove = yPercent * treeBender.jumpHeight + startPos.y;
    //      Vector3 newPos = startPos;
    //      newPos.y = Mathf.SmoothDamp(newPos.y, yMove, ref launchVelocity, treeBender.jumpTime);
    //      baseTransform.localPosition = newPos;
    //  
    //      float yScale = yPercent * treeBender.stretchHeight + topStartScale.y;
    //      Vector3 newScale = topStartScale;
    //      newScale.y = Mathf.SmoothDamp(newScale.y, yScale, ref stretchVelocity, treeBender.stretchTime);
    //      transform.localScale = newScale;
    //  
    //  
    //      // baseRB.MovePosition(baseTransform.TransformPoint(newPos));
    //  
    //      //float spd = treeBender.rotationSpeed * Time.fixedDeltaTime;
    //      //newRot = Mathf.MoveTowardsAngle(
    //      //    CurrentRotation, 
    //      //    goalRotation, 
    //      //    spd);
    //  
    //      baseRB.MoveRotation(newRot);
    //  
    //  }


    //private void OnDrawGizmos()
    //{
    //    if (!doDebug) return;

    //}
}