using System;
using System.Collections;
using MathUtility;
using UnityEngine;


public class RotateToDirectionNoRb : MonoBehaviour
{

    private Transform baseTransform;
    private Rigidbody2D baseRB;
    private DragTreePosition treeActive;
    private TreeBend treeBender;

    private float startRotation = 0f;
    
    [SerializeField] private float goalRotation = 0f;

    private float bendVelocity = 0.0f;


    private void Start()
    {
        treeActive = FindObjectOfType<DragTreePosition>();
        treeBender = FindObjectOfType<TreeBend>();
        baseTransform = transform.parent;
        baseRB = baseTransform.GetComponent<Rigidbody2D>();
        startRotation = goalRotation = baseRB.rotation;
    }

    private void FixedUpdate()
    {
        if (Mathf.Approximately(baseRB.rotation, goalRotation)) return;

        float newRot;
        newRot = Mathf.SmoothDampAngle(
            baseRB.rotation,
            goalRotation,
            ref bendVelocity,
            treeBender.bendTime,
            treeBender.maxSpeed,
            Time.fixedDeltaTime);

        //float spd = treeBender.rotationSpeed * Time.fixedDeltaTime;
        //newRot = Mathf.MoveTowardsAngle(
        //    CurrentRotation, 
        //    goalRotation, 
        //    spd);

        baseRB.MoveRotation(newRot);
    }

    public void RotateToThis(float distPercent, Vector3 pos)
    {
        Vector2 newDirection = Vector2.down;

        Vector3 relativePoint = baseTransform.InverseTransformPoint(pos);
        if (relativePoint.x < 0f)
            newDirection.y = -1; // Right side
        else if (relativePoint.x > 0f)
            newDirection.y = 1; // Left side

        float angle = MathU.Vector2ToDegree(newDirection);
        angle *= distPercent;

        goalRotation = startRotation + angle;
    }
}