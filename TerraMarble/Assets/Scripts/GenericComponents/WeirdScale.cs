using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeirdScale : MonoBehaviour
{

    public Transform objectToScale;
    public float startScale = 0.5f;
    public float targetScale = 1.0f;
    public float scaleSpeed = 1.0f;
    public float overshootAmount = 0.2f;

    private Vector3 originalScale;

    private void Start()
    {

        //StartCoroutine(ScaleObject());
    }
    public void StartScaleObject()
    {
        StartCoroutine(ScaleObject());
    }
    private IEnumerator ScaleObject()
    {
        originalScale = objectToScale.localScale;
        float currentScale = startScale;

        while (currentScale < targetScale + overshootAmount)
        {
            currentScale += scaleSpeed * Time.deltaTime;
            float xScale = originalScale.x * currentScale;
            float yScale = originalScale.y * currentScale;
            float zScale = originalScale.z * currentScale;
            objectToScale.localScale = new Vector3(xScale, yScale, zScale);
            yield return null;
        }

        while (currentScale > targetScale)
        {
            currentScale -= scaleSpeed * Time.deltaTime;
            float xScale = originalScale.x * currentScale;
            float yScale = originalScale.y * currentScale;
            float zScale = originalScale.z * currentScale;
            objectToScale.localScale = new Vector3(xScale, yScale, zScale);
            yield return null;
        }

        float targetXScale = originalScale.x * targetScale;
        float targetYScale = originalScale.y * targetScale;
        float targetZScale = originalScale.z * targetScale;
        objectToScale.localScale = new Vector3(targetXScale, targetYScale, targetZScale);
    }
}