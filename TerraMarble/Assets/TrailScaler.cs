using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailScaler : MonoBehaviour
{

    public TrailRenderer trailRenderer;
    public float targetScaleFactor1 = 2.0f; // Adjust the target scale factor
    public float targetScaleFactor2 = 2.0f; // Adjust the target scale factor
    public float targetScaleFactorLength = 2.0f; // Adjust the target scale factor
    [SerializeField]
    private float minAuraTrailSize1;
    [SerializeField]
    private float maxAuraTrailSize1;
    [SerializeField]
    private float minAuraTrailSize2;
    [SerializeField]
    private float maxAuraTrailSize2;
    [SerializeField]
    private float minAuraTrailLength;
    [SerializeField]
    private float maxAuraTrailSizeLength;

    public float lerpSpeed = 1.0f; // Adjust the speed of the interpolation

    private float currentScaleFactor1 = 1.0f;
    private float currentScaleFactor2 = 1.0f;
    private float currentScaleFactorLength = 1.0f;
    public bool isScaling = false;


    [Range(0,1)]
    public float testamount;
    public bool test;
    void Start()
    {
        //// Ensure a TrailRenderer component is attached
        //if (trailRenderer == null)
        //{
        //    trailRenderer = GetComponent<TrailRenderer>();
        //}
    }
    public void UpdateTrail(float percent)
    {
        targetScaleFactor1 = Mathf.Lerp(minAuraTrailSize1, maxAuraTrailSize1, percent);
        targetScaleFactor2 = Mathf.Lerp(minAuraTrailSize2, maxAuraTrailSize2, percent);
        targetScaleFactorLength = Mathf.Lerp(minAuraTrailLength,maxAuraTrailSizeLength,percent);
        isScaling = true;

    }
    void Update()
    {

        if (test)
        {
            UpdateTrail(testamount);
            test = false;
        }

        if (isScaling && (Mathf.Abs(currentScaleFactor1 - targetScaleFactor1) > 0.01f))
        {
            // Interpolate between the current and target scale factors
            currentScaleFactor1 = Mathf.Lerp(currentScaleFactor1, targetScaleFactor1, lerpSpeed * Time.deltaTime);
            currentScaleFactor2 = Mathf.Lerp(currentScaleFactor2, targetScaleFactor2, lerpSpeed * Time.deltaTime);
            currentScaleFactorLength = Mathf.Lerp(currentScaleFactorLength,targetScaleFactorLength, lerpSpeed * Time.deltaTime);
            // Modify the two key points of the width curve
            Keyframe[] keys = trailRenderer.widthCurve.keys;
            keys[0].value = currentScaleFactor1;
            keys[1].value = currentScaleFactor2;

            // Create a modified curve and apply it to the TrailRenderer
            AnimationCurve modifiedCurve = new AnimationCurve(keys);
            trailRenderer.widthCurve = modifiedCurve;
            trailRenderer.time = currentScaleFactorLength;
            // Check if we've reached the target scale factor with a small tolerance
            if (Mathf.Abs(currentScaleFactor1 - targetScaleFactor1) < 0.01f)
            {
                isScaling = false;

            }
        }
    }

    // Call this method to start the scaling process
    public void StartScaling()
    {
        isScaling = true;
    }
}

