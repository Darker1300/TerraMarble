using MathUtility;
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
    [SerializeField] private float jumpFromRotation = 0f;

    [SerializeField] private float currentGoalPercent = 0f;

    [SerializeField] private float jumpPercent = 0;


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
        if (Mathf.Approximately(baseRB.rotation, goalRotation))
        {
            jumpPercent = 0;
            return;
        }

        // Rotation
        float newRot = Mathf.SmoothDampAngle(
            baseRB.rotation,
            goalRotation,
            ref bendVelocity,
            treeBender.bendTime,
            treeBender.maxSpeed,
            Time.fixedDeltaTime);

        // Jump and Stretch
        float t = MathU.InverseLerpAngle(startRotation, jumpFromRotation, newRot);
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
        {
            // Jump up
            jumpPercent = Mathf.Abs(collapsePercent - currentGoalPercent);
            jumpFromRotation = baseRB.rotation;
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
        jumpFromRotation = baseRB.rotation;
        float delta = MathU.DeltaRange(jumpFromRotation, startRotation, 360f);
        jumpPercent = Mathf.Abs(delta / 180f);
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