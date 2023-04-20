using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class LookTowards : MonoBehaviour
{
    public bool track = true;
    public bool useRigidBody = false;
    public Transform trackedObject = null;
    public float rotationOffset = 0f;
    private Rigidbody2D rb2D = null;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!track || !trackedObject) return;
        Vector3 dir = (trackedObject.position - transform.position).normalized;
        float angle = Vector3.SignedAngle(Vector3.up, dir, Vector3.forward);
        float finalAngle = angle + rotationOffset;
        if (useRigidBody && rb2D)
            rb2D.MoveRotation(finalAngle);
        else transform.rotation = Quaternion.AngleAxis(finalAngle, Vector3.forward);
    }
}
