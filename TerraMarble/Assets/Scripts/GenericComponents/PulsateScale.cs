using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulsateScale : MonoBehaviour
{
      public float minScale = 2.5f;
        public float maxScale = 3.5f;
        public float pulseSpeed = 1.0f;

        private IEnumerator scaleCoroutine;

        private void Start()
        {
            // Start the coroutine when the script is first enabled
            scaleCoroutine = ScaleCoroutine();
            StartCoroutine(scaleCoroutine);
        }

        private IEnumerator ScaleCoroutine()
        {
            while (true)
            {
                // Scale up
                while (transform.localScale.x < maxScale)
                {
                    transform.localScale += new Vector3(pulseSpeed * Time.deltaTime, pulseSpeed * Time.deltaTime, 0);
                    yield return null;
                }

                // Scale down
                while (transform.localScale.x > minScale)
                {
                    transform.localScale -= new Vector3(pulseSpeed * Time.deltaTime, pulseSpeed * Time.deltaTime, 0);
                    yield return null;
                }
            }
        }

        private void OnDisable()
        {
            // Stop the coroutine when the script is disabled
            StopCoroutine(scaleCoroutine);
        }
    }