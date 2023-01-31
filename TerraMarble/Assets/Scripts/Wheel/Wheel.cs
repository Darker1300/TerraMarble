using MathUtility;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.Events;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;


public class Wheel : MonoBehaviour
{
    [Foldout("Show Debug Fields")]
    public WindSpin windSpin;

    [Foldout("Show Debug Fields")] public new Rigidbody2D rigidbody2D;
    [Foldout("Show Debug Fields")] public CircleCollider2D wheelCollider2D;
    [Foldout("Show Debug Fields")] public WheelRegionsManager regions;


    private void Start()
    {
        rigidbody2D ??= GetComponent<Rigidbody2D>();
        wheelCollider2D ??= GetComponent<CircleCollider2D>();
        windSpin ??= FindObjectOfType<WindSpin>();
        regions ??= GetComponent<WheelRegionsManager>();

    }


    private void Update()
    {
    }

    private void FixedUpdate()
    {
    }


    


    //[Button]
    //public void ReverseSpin()
    //{
    //    velocity = Mathf.Clamp(-velocity * reverseSpeedUpFactor, -maxSpeed, maxSpeed);
    //}

    //public bool IsMovingWithWheel(Vector2 _worldPos, Vector2 _worldVector)
    //{
    //    Vector2 wheelVector = _worldPos.Towards(transform.position);
    //    float degreesFrom = -Vector2.SignedAngle(_worldVector.normalized, wheelVector.normalized);
    //    int degreesFromSign = Math.Sign(degreesFrom);

    //    return Math.Abs(degreesFrom) < 10f
    //           || degreesFromSign == -Math.Sign(velocity);
    //}
}