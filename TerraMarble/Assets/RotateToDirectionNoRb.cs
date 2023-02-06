using MathUtility;
using UnityEngine;
using Unity.Mathematics;

public class RotateToDirectionNoRb : MonoBehaviour
{

    private Transform baseTransform;
    private Rigidbody2D baseRB;
    private DragTreePosition treeActive;
    private TreeBend treeBender;
    private float startY;


    private float startRotation = 0f;

    [SerializeField] private float goalRotation = 0f;
    [SerializeField] private float jumpFromRotation = 0f;

    [SerializeField] private float currentGoalPercent = 0f;

    [SerializeField] private float jumpPercent = 0;


    private float bendVelocity = 0.0f;


    private void Start()
    {
        treeActive = FindObjectOfType<DragTreePosition>();
        treeBender = FindObjectOfType<TreeBend>();
        baseTransform = transform.parent;
        startY = baseTransform.localPosition.y;
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

        float newRot;
        newRot = Mathf.SmoothDampAngle(
            baseRB.rotation,
            goalRotation,
            ref bendVelocity,
            treeBender.bendTime,
            treeBender.maxSpeed,
            Time.fixedDeltaTime);

        float t =  MathU.InverseLerpAngle(startRotation, jumpFromRotation, newRot);
        Vector3 pos = baseTransform.localPosition;
        pos.y = (treeBender.PopOutHeightCurve.Evaluate(t) * treeBender.bendHeight * jumpPercent) + startY;
        baseTransform.localPosition = pos;
        //math.remap(0,1,startY,start)
        //float spd = treeBender.rotationSpeed * Time.fixedDeltaTime;
        //newRot = Mathf.MoveTowardsAngle(
        //    CurrentRotation, 
        //    goalRotation, 
        //    spd);

        baseRB.MoveRotation(newRot);

    }

    public void RotateToThis(float collapsePercent, Vector3 pos)
    {
        if (currentGoalPercent > (1f - treeBender.deadRange)
            && collapsePercent > treeBender.deadRange) return;

        goalRotation = startRotation + CalcLocalRotation(collapsePercent, pos);
        if (collapsePercent < currentGoalPercent)
        {
            // Jump up
            jumpPercent = Mathf.Abs(collapsePercent - currentGoalPercent);
            jumpFromRotation = baseRB.rotation;
        }

        currentGoalPercent = collapsePercent;

    }

    private float CalcLocalRotation(float collapsePercent, Vector3 pos)
    {
        Vector2 newDirection = Vector2.down;

        Vector3 relativePoint = baseTransform.InverseTransformPoint(pos);
        if (relativePoint.x < 0f)
            newDirection.y = -1; // Right side
        else if (relativePoint.x > 0f)
            newDirection.y = 1; // Left side

        float angle = MathU.Vector2ToDegree(newDirection);
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

    //only thing that changes is how much time it has to get to that y height if tree half bent
    //then its only got half the amount of time to return
}