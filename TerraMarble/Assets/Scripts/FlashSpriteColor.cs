using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashSpriteColor : MonoBehaviour
{
    public Image imageToChange;
    public Color startColor;
    public Color endColor;
    public AnimationCurve colorChangeCurve;
    public float duration = 3f;
    public bool loop = true;

    private float timer = 0f;
    private bool isChangingColor = false;

    private void Update()
    {
        if (isChangingColor)
        {
            timer += Time.deltaTime;

            // Calculate the t value based on the curve
            float t = timer / duration;
            float curveValue = colorChangeCurve.Evaluate(t);

            // Interpolate the color
            Color lerpedColor = Color.Lerp(startColor, endColor, curveValue);
            imageToChange.color = lerpedColor;

            if (timer >= duration)
            {
                isChangingColor = false;

                if (loop)
                {
                    // Reset the timer for looping
                    timer = 0f;
                    isChangingColor = true;
                    Color temp = startColor;
                    startColor = endColor;
                    endColor = temp;
                }
            }
        }
    }
    private void OnEnable()
    {
        StartColorChange();
    }

    public void StartColorChange()
    {
        // Reset the timer and start changing color
        timer = 0f;
        isChangingColor = true;
    }
}
