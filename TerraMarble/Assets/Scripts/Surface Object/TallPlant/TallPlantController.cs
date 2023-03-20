using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.Splines;

public class TallPlantController : MonoBehaviour
{
    [SerializeField] private SplineContainer bodySplineContainer;
    [SerializeField] private LineRenderer bodyLine;

    private Spline bodySpline
        => bodySplineContainer.Spline;

    public float headHeight = 1.5f;
    [Min(2)]
    public int linePointCount = 10;

    [SerializeField] private Vector3[] linePoints;


    void Start()
    {
        bodySplineContainer ??= GetComponentInChildren<SplineContainer>();
        bodyLine ??= GetComponentInChildren<LineRenderer>();
    }

    void OnValidate()
    {
        SetHeight(headHeight);
        UpdateRenderer();
    }

    void SetHeight(float newHeight)
    {
        bodySplineContainer ??= GetComponentInChildren<SplineContainer>();
        if (bodySpline.Count < 1) return;
        
        BezierKnot bezierKnot0 = bodySpline[^1];
        BezierKnot bezierKnot1 = bodySpline[^2];
        bezierKnot0.Position.y = newHeight;
        bezierKnot1.Position.y = newHeight;
        bodySpline[^1] = bezierKnot0;
        bodySpline[^2] = bezierKnot1;
    }

    void UpdateRenderer()
    {
        if (bodySpline.Count < 1) return;
        bodyLine ??= GetComponentInChildren<LineRenderer>();

        linePoints = new Vector3[linePointCount];
        bodyLine.GetPositions(linePoints);

        for (int index = 0; index < linePointCount; index++)
        {
            float curveT = index / (float) (linePointCount - 1);
            linePoints[index] = bodySplineContainer.EvaluatePosition(curveT);
        }

        bodyLine.positionCount = linePointCount;
        bodyLine.SetPositions(linePoints);
    }

    //private void OnDrawGizmos()
    //{
    //    if (linePoints.Length <= 0) return;
    //    bodySplineContainer ??= GetComponentInChildren<SplineContainer>();
    //    float length = bodySplineContainer.CalculateLength();

    //    for (int index = 0; index < linePointCount; index++)
    //    {
    //        float curveT = index / (float)(linePointCount - 1);
    //        Vector3 accel = bodySplineContainer.EvaluateTangent(curveT);
    //        Vector3 pos = bodySplineContainer.EvaluatePosition(curveT);
    //        //SplineUtility.
    //        float curvature = bodySpline.EvaluateCurvature(curveT);
    //        Debug.Log(curvature + "|..." + accel.ToString());
    //        Gizmos.DrawWireSphere(pos, curvature);
    //    }
    //}
}
