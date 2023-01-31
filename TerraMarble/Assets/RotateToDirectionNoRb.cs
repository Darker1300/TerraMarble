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

    [SerializeField] private float maxRotation = 75f;
    [SerializeField] private float goalRotation = 0f;
    private float CurrentRotation => baseRB.rotation;


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
        if (Mathf.Approximately(CurrentRotation, goalRotation)) return;

        //float diff = MathU.DeltaRange(
        //    Mathf.Repeat(CurrentRotation, 360f), 
        //    goalRotation, 360f);
        //float diffSign = Mathf.Sign(diff);
        //float newRot = CurrentRotation + (spd * diffSign);


        float spd = treeBender.rotationSpeed * Time.fixedDeltaTime;
        float newRot = Mathf.MoveTowardsAngle(
            CurrentRotation, 
            goalRotation, 
            spd);

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
        angle *= 1f - distPercent;


        goalRotation = startRotation + angle;

        // Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward) * startLocalRot;
        // baseTransform.localRotation = Quaternion.Slerp(baseTransform.localRotation, rotation, rotationSpeed * Time.deltaTime);
    }

    //private IEnumerator ReturnToDefaultAngle()
    //{
    //    float Timer = 0;


    //    while (Timer <= durationReset)
    //    {
    //        Timer = Timer + Time.deltaTime;
    //        float percent = Mathf.Clamp01(Timer / durationReset);

    //        float angle = Mathf.Atan2(0, 1) * Mathf.Rad2Deg;
    //        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward) * startLocalRot;
    //        baseTransform.localRotation = Quaternion.Slerp(baseTransform.localRotation, rotation, percent);


    //        yield return null;
    //    }
    //}
}