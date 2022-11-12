using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class LookTowards : MonoBehaviour
{
    public bool track = false;
    public Transform trackedObject = null;
    public float rotationOffset = 0f;
    void Update()
    {
        if (!track) return;
        Vector3 dir = (trackedObject.position - transform.position).normalized;
        float angle = Vector3.SignedAngle(Vector3.up, dir, Vector3.forward);
        transform.rotation = Quaternion.AngleAxis(angle + rotationOffset, Vector3.forward);
    }
}
