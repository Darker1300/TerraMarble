using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using System;
using UnityEngine.Events;

public class ChangeColorOverTime : MonoBehaviour
{
    public Color targetColor; // the color we want to lerp to
    public float duration = 2f; // how long the color will take to become fully transparent
    public AnimationCurve ColorCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f); // the curve used to determine how the transparency changes over time
   // public Renderer objectRenderer; // the object's renderer that we'll be changing the color of
    public UnityEvent  colorChangeEnd;
    private Color initialColor; // the object's initial color before transparency is applied
    public ShapeRenderer shapeRender;
    void Start()
    {
        // save the object's initial color so we can restore it later
        initialColor = shapeRender.Color;
    }
    public void LerpToColor()
    {
        // start the coroutine to gradually lerp the object's color
        StartCoroutine(LerpColorRoutine());
    }

    IEnumerator LerpColorRoutine()
    {
        float elapsedTime = 0f;
        Color currentColor = initialColor;
        //colorChangeStart?.Invoke(true);
        while (elapsedTime < duration)
        {
            // calculate the percentage of time that has elapsed so far, using the animation curve to influence it
            float curveTime = elapsedTime / duration;
            float curveValue = ColorCurve.Evaluate(curveTime);

            // lerp the color from the initial color to the target color based on the animation curve
            currentColor = Color.Lerp(initialColor, targetColor, curveValue);
            shapeRender.Color = currentColor;

            // increment the elapsed time and wait for the next frame
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ensure the object is the target color after the coroutine has completed
        shapeRender.Color = targetColor;
        colorChangeEnd?.Invoke();
        
    }

    public void ResetColor()
    {
        // restore the object's initial color
        shapeRender.Color = initialColor;
    }
    public void MakeTransparent()
    {
        // start the coroutine to gradually make the object transparent
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float elapsedTime = 0f;
        Color currentColor = initialColor;

        while (elapsedTime < duration)
        {
            // calculate the percentage of time that has elapsed so far, using the animation curve to influence it
            float curveTime =1- (elapsedTime / duration);
            float curveValue = ColorCurve.Evaluate(curveTime);

            // set the object's color to the current color with alpha adjusted based on the animation curve
            currentColor.a = curveValue;
            shapeRender.Color = currentColor;

            // increment the elapsed time and wait for the next frame
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // ensure the object is fully transparent after the coroutine has completed
        currentColor.a = 0f;
        shapeRender.Color = currentColor;
        colorChangeEnd?.Invoke();
    }

   
}