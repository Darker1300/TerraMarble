using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TrackSpin : MonoBehaviour
{
    [Header("Config")]
    public bool track = true;
    public bool trackClockwise = true;
    public bool trackAntiClockwise = true;
    public bool invertRotation = false;
    public Transform trackedObject = null;
    [Header("Debug")]
    public float lastFrameAngle = 0f;

    void Start()
    {
        if (!trackedObject) return;
        lastFrameAngle = trackedObject.eulerAngles.z;

    }

    private void OnValidate()
    {
        if (!trackedObject) return;
        lastFrameAngle = trackedObject.eulerAngles.z;
    }

    void Update()
    {
        if (!track || !trackedObject) return;

        float currentFrameAngle = trackedObject.eulerAngles.z;
        float delta = Mathf.DeltaAngle(lastFrameAngle, currentFrameAngle);

        float currentAngle = transform.eulerAngles.z;

        if (trackClockwise && Math.Sign(delta) == 1)
            currentAngle += (-delta * (invertRotation ? -1f : 1f));
        if (trackAntiClockwise && Math.Sign(delta) == -1)
            currentAngle += (delta *2f * (invertRotation ? -1f : 1f));
        

        transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);

        lastFrameAngle = trackedObject.eulerAngles.z;
    }

}
