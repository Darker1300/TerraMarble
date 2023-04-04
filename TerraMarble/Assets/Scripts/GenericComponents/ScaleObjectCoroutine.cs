using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleObjectCoroutine : MonoBehaviour
{

    public Transform objectToScale;


    public float startScale = 0.5f;
    public float targetScale = 1.0f;
    public float scaleSpeed = 1.0f;
    public float overshootAmount = 0.2f;

    private void Start()
    {

    }

    public void StartScale()
    {
        StartCoroutine(ScaleObject());

    }

    private IEnumerator ScaleObject()
    {
        float currentScale = startScale;

        while (currentScale < targetScale + overshootAmount)
        {
            currentScale += scaleSpeed * Time.deltaTime;

            objectToScale.localScale = new Vector3(currentScale, currentScale, currentScale);

            yield return null;
        }

        while (currentScale > targetScale)
        {
            currentScale -= scaleSpeed * Time.deltaTime;

            objectToScale.localScale = new Vector3(currentScale, currentScale, currentScale);

        }

        yield return null;



        objectToScale.localScale = new Vector3(targetScale, targetScale, targetScale);

    }


}