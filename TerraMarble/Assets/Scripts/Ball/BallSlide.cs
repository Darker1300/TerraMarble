using System.Collections;
using System.Collections.Generic;
using MathUtility;
using UnityEngine;

public class BallSlide : MonoBehaviour
{
    private WheelRegionsManager wheelRegions;
    private TreeBend treeBend;
    private PlayerInput playerInput;
    private Rigidbody2D ballRb;
    public bool IsSliding = false;

    private void Awake()
    {
        wheelRegions = FindObjectOfType<WheelRegionsManager>();
        treeBend = FindObjectOfType<TreeBend>(true);
        ballRb = GetComponent<Rigidbody2D>();
        playerInput = FindObjectOfType<PlayerInput>();
    }

    private void FixedUpdate()
    {
        FixedUpdateSliding();
    }

    private void FixedUpdateSliding()
    {
        Vector2 worldPos = wheelRegions.transform.position;
        Vector2 upVector = worldPos.Towards(ballRb.transform.position).normalized;
        Vector2 rightVector = upVector.Perp();

        float sideDot = ballRb.velocity.normalized.Dot(rightVector);
        //Debug.Log(string.Format("spd: {0}", ballRb.velocity.magnitude));

        // direction test
        // || speed test
        float absSideDot = Mathf.Abs(sideDot);
        if (
            !playerInput.IsDragging ||
            absSideDot < treeBend.slideDirectionThreshold // direction test
            || ballRb.velocity.magnitude < treeBend.minSlideVelocityNeeded) // speed test
        {
            IsSliding = false;
            //Debug.Log("END");
            return;
        }

        //if (IsSliding == false)
        //{
        //    //Debug.Log("START");
        //}

        // Is Sliding
        IsSliding = true;

        // Get the direction of the velocity (right = 1, left = -1)
        float direction = Mathf.Sign(Vector2.Dot(rightVector, ballRb.velocity.normalized));

        // Calculate the target velocity
        Vector2 targetVelocity = (rightVector * direction).Rotate(treeBend.slideLiftAngle * direction);

        // Calculate the target velocity
        targetVelocity = targetVelocity.normalized * ballRb.velocity.magnitude;

        // Rotate velocity towards adjustedTargetVelocity
        ballRb.velocity = RotateTowards(
            ballRb.velocity,
            targetVelocity,
            treeBend.slideAdjustmentSpeed * Time.fixedDeltaTime);

        float horizontalSpd = Mathf.Abs(ballRb.velocity.Rej(upVector).x);
        if (horizontalSpd < treeBend.slideMinForce)
        {
            float clampedBoost = Mathf.Max(0f, treeBend.slideBoostForce - Mathf.Abs(horizontalSpd)) *
                                 Time.fixedDeltaTime;
            ballRb.velocity += rightVector * sideDot * clampedBoost;
        }
    }


    private Vector2 RotateTowards(Vector2 current, Vector2 target, float maxDegreesDelta)
    {
        float angleDiff = Vector2.SignedAngle(current, target);
        float changeInAngle = Mathf.Clamp(angleDiff, -maxDegreesDelta, maxDegreesDelta);
        return current.Rotate(changeInAngle);
    }
}