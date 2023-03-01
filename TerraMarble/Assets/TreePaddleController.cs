using MathUtility;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    [Header("Data")]
    [SerializeField] private float startRotation;
    [SerializeField] private float launchStartRotation;
    [SerializeField] private float goalRotation = 0f;

    private float currentRotation
    {
        get => baseRB.rotation;
        set => baseRB.rotation = value;
    }

    [SerializeField] private float wobbleGoalRotation = 0f;
    [SerializeField] private float wobbleAngleVelocity = 0f;

    [SerializeField] private float launchPowerPercent = 0;
    [SerializeField] private float launchProgress = 1f;

    private float bendVelocity = 0.0f;
    private float launchVelocity = 0.0f;
    private float stretchVelocity = 0.0f;
    private float jumpVelocity = 0.0f;

    //private float flipVelocity = 0.0f;
    //[SerializeField] private float flipGoal = 0f;
    //[SerializeField] private float flipCurrent = 0f;
    //[SerializeField] private float flipAmount = 180f;
    //[SerializeField] private Vector3 flipAxis;

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

        launchProgress = 1f;
    }

    private void FixedUpdate()
    {
        UpdateBend();
    }

    private void UpdateBend()
    {
        wobbleGoalRotation = goalRotation + wobbleAngleVelocity;

        float newCurrentRotation = currentRotation;

        if (!Mathf.Approximately(newCurrentRotation, wobbleGoalRotation))
        {
            newCurrentRotation = Mathf.SmoothDampAngle(newCurrentRotation, wobbleGoalRotation,
                ref bendVelocity, treeBender.bendTime, treeBender.bendMaxSpeed, Time.fixedDeltaTime);

            baseRB.MoveRotation(newCurrentRotation);
        }

        UpdateLaunch();

        if (!Mathf.Approximately(0f, wobbleAngleVelocity))
            UpdateWobble();
    }

    private void UpdateLaunch()
    {
        //if (launchProgress >= 1f) return;

        if (!Mathf.Approximately(1f, launchProgress))
            launchProgress = Mathf.SmoothDamp(
                launchProgress, 1f, ref launchVelocity,
                treeBender.launchTime, treeBender.bendMaxSpeed, Time.fixedDeltaTime);

        // Jump and Stretch
        Vector3 newPos = baseTransform.localPosition;
        Vector3 newScale = transform.localScale;

        // curve = 0..1..0;
        float launchCurveT = treeBender.launchCurve.Evaluate(launchProgress);
        float goalPosY = startPos.y + treeBender.jumpHeight * launchPowerPercent * launchCurveT;
        float goalScaleY = topStartScale.y + treeBender.stretchHeight * launchPowerPercent * launchCurveT;

        if (!Mathf.Approximately(newPos.y, goalPosY))
            newPos.y = Mathf.SmoothDamp(newPos.y, goalPosY, ref jumpVelocity,
                treeBender.launchTime, treeBender.bendMaxSpeed, Time.fixedDeltaTime);

        if (!Mathf.Approximately(newScale.y, goalScaleY))
            newScale.y = Mathf.SmoothDamp(newScale.y, goalScaleY, ref stretchVelocity,
                treeBender.launchTime, treeBender.bendMaxSpeed, Time.fixedDeltaTime);

        baseTransform.localPosition = newPos;
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
            float deltaAngle = Mathf.DeltaAngle(currentRotation, wobbleGoalRotation);
            if (Mathf.Abs(deltaAngle) < treeBender.wobbleMinThreshold)
                wobbleAngleVelocity = -wobbleAngleVelocity * treeBender.wobbleSlowFactor;
        }
    }

    private void SetWobble(float _wobbleVelocity)
    {
        wobbleAngleVelocity = _wobbleVelocity;
    }

    public void SetTreeState(float upPercent, int direction)
    {
        // set new goal
        if (direction == 0) direction = 1;

        float newLocalRotation = MathU.Vector2ToDegree(Vector2.up * direction) * (1f - upPercent);
        
        goalRotation = startRotation + newLocalRotation;

        SetWobble(0f);
    }

    private void InitLaunch()
    {
        launchStartRotation = currentRotation;

        float delta = MathU.DeltaRange(launchStartRotation, startRotation, 360f);
        float powerPercent = Mathf.Abs(delta / 180f) * 2f;
        launchPowerPercent = powerPercent;

        launchProgress = 0f;

        // prepare for wobble
        SetWobble(launchPowerPercent * treeBender.wobbleMaxAngle);
    }

    public void Release()
    {
        // launch
        InitLaunch();

        // Goal
        goalRotation = startRotation;
    }

    private void OnDrawGizmos()
    {
        if (!doDebug) return;
        Vector3 flipAxis = ((Vector2) region.Wheel.transform.position).Towards(region.Base.transform.position).normalized;
        Gizmos.DrawRay(region.Base.transform.position, flipAxis);
    }

    public int DirectionFromPoint(Vector2 worldPos)
    {
        float towardsPointDelta = WheelRegionsManager.RegionDistanceDelta(
            region.RegionIndex + 0.5f,
            region.WorldToRegionDistance(worldPos));

        if (towardsPointDelta > 0f)
        { // Right side
            return -1;
        }
        else //if (towardsPointDelta < 0f)
        { // Left side
            return 1;
        }
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