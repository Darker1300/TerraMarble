using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtility;

public class VelocitySpinner : MonoBehaviour
{
    public Transform ToSpin;
    public float initialVelocity = 0f;
    public float spinFactor = 0.5f;
    public float maxSpeed = 10f;
    public float speedDecayRate = 0.5f;
    public float rechargeRate = 0.2f;
    public int ammoPerInterval = 5; // Ammo added at each interval
    public int totalAmmo = 50;
    public TurretShooter turretShooter;


    private float currentSpeed = 0f;
    private bool isSpinningLeft = false;

    public float currentAngle = 0f; // Angle rotated by the wheel
    public float fillValue = 0f;
    public bool test = false;
    public bool testl = false;

    private void Start()
    {
        currentSpeed = initialVelocity;
    }

    private void Update()
    {
        if (test)
        {
            AddSpin(true);
            test = false;
        }
        if (testl)
        {
            AddSpin(false);
            testl = false;
        }

      
            // Apply speed decay over time
            currentSpeed -= speedDecayRate * Time.deltaTime;
            currentSpeed = Mathf.Max(0f, currentSpeed);

            // Update the rotation based on the current speed and direction
            float rotationAmount = currentSpeed * Time.deltaTime;
            if (isSpinningLeft)
            {
               ToSpin. transform.Rotate(Vector3.forward, rotationAmount);
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
        // Check if fillValue reached an interval threshold, add ammo and reset angle
        if (fillValue >= 0.25f && fillValue < 0.5f)
        {
            turretShooter.AmmoAmount =Mathf.Clamp(turretShooter.AmmoAmount + ammoPerInterval,0, turretShooter.MaxAmmo);
            currentAngle = 0f;
        }
        else if (fillValue >= 0.5f && fillValue < 0.75f)
        {
            turretShooter.AmmoAmount = Mathf.Clamp(turretShooter.AmmoAmount + ammoPerInterval, 0, turretShooter.MaxAmmo);
            currentAngle = 0f;
        }
        Debug.Log("fill val " + fillValue);

    }

    public void AddSpin(bool isLeftDirection)
    {
        if (isSpinningLeft == isLeftDirection)
        {
            // If the new spin direction is the same as the current direction,
            // add to the current speed
            currentSpeed += spinFactor;
        }
        else
        {
            // If the new spin direction is different, reset the direction and speed
            isSpinningLeft = isLeftDirection;
            currentSpeed = initialVelocity;
            currentSpeed += spinFactor;
        }

        // Cap the speed to the maximum allowed
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);
    }
}
