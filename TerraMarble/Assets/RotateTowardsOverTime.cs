using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsOverTime : MonoBehaviour
{
  
        public Transform trackedObject;
        public float rotationSpeed = 90.0f; // Degrees per second

        private void Update()
        {
            if (trackedObject != null)
            {
                Vector3 dir = trackedObject.position - transform.position;
                float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

                float step = rotationSpeed * Time.deltaTime;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, step);
            }
        }
    
}
