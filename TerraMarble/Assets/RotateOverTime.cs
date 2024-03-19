using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    public float rotationSpeed = 45f; // Adjust the speed as needed

    void Update()
    {
        // Rotate the object around the Z-axis
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);
    }
}