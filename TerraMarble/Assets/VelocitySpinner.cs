using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtility;

public class VelocitySpinner : MonoBehaviour
{
    public Transform ToSpin;
   
    public TurretShooter turretShooter;

    public int AmmoPerInterval =1;
    public float initialVelocity = 0f;
    public float spinFactor = 0.5f;
    public float maxSpeed = 10f;
    public float speedDecayRate = 0.5f;
    public float rechargeRate = 0.2f;

    private float currentSpeed = 0f;
    private bool isSpinningLeft = false;

    public float currentAngle = 0f; // Angle rotated by the wheel
    public float fillValue = 0f;    // 0-1 value that fills as the wheel spins
    public float CollectAtPercentange;
    private void Start()
    {
        currentSpeed = initialVelocity;
    }

    private void Update()
    {
        // Apply speed decay over time
        currentSpeed -= speedDecayRate * Time.deltaTime;
        currentSpeed = Mathf.Max(0f, currentSpeed);

        // Update the rotation based on the current speed and direction
        float rotationAmount = currentSpeed * Time.deltaTime;
        if (isSpinningLeft)
        {
            ToSpin.transform.Rotate(Vector3.forward, rotationAmount);
        }
        else
        {
            ToSpin.transform.Rotate(Vector3.back, rotationAmount);
        }

        // Update the angle and fill value based on rotation
        currentAngle += rotationAmount;
        fillValue = currentAngle / 360f; // Assuming a full rotation is 360 degrees

        // Cap the fill value to 1
        fillValue = Mathf.Clamp01(fillValue);
        if (fillValue >= CollectAtPercentange)
        {
            turretShooter.AmmoAmount = Mathf.Clamp(turretShooter.AmmoAmount + AmmoPerInterval, 0, turretShooter.MaxAmmo);
            currentAngle = 0.0f;
        }
    }

    public void AddSpin(bool isLeftDirection)
    {
        if (isSpinningLeft == isLeftDirection)
        {
            // If the new spin direction is the same as the current direction,
            // add to the current speed
            currentSpeed += spinFactor;

            //turretShooter.AmmoAmount = Mathf.Clamp(turretShooter.AmmoAmount + ammoPerInterval, 0, turretShooter.MaxAmmo);
        }
        else
        {
            // If the new spin direction is different, reset the direction and speed
            isSpinningLeft = isLeftDirection;
            currentSpeed = initialVelocity;
        }

        // Cap the speed to the maximum allowed
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
    }
}

