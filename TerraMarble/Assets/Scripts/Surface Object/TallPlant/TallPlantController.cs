using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
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
        int lastIndex = bodySpline.Count - 1;
        var lastBezierKnot = bodySpline[lastIndex];
        lastBezierKnot.Position.y = newHeight;
        bodySpline[lastIndex] = lastBezierKnot;
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
}
