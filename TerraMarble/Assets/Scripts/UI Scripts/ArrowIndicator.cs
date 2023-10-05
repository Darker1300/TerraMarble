using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class ArrowIndicator : MonoBehaviour
{
   
        public Transform player;
        public Transform enemy;

        public GameObject indicatorPrefab; // The arrow or indicator sprite.
        private GameObject indicatorInstance;

        public float arrowDistance = 3f; // The distance at which the arrow becomes visible.
        public float arrowRotationSpeed = 5f; // The rotation speed of the arrow.
        public float arrowDistMin;

    [Header("Color Settings")]
    public Color flashColor = new Color(1f, 0f, 0f, 1f); // Color to flash (red in this example).
    public float flashSpeed = 2f; // Speed of the flash effect.
    public float defaultAlpha = 1f; // Default alpha value when not flashing.

    private bool isFlashing = false;
    private Color originalColor;
    public ShapeRenderer[] indicatorShapes;
    public bool test;

    private void Start()
        {
            // Instantiate the indicator GameObject.
            indicatorInstance = Instantiate(indicatorPrefab, transform);
            indicatorInstance.SetActive(false); // Initially, the indicator is not visible.

        ///////////////////////////////////////////////////
        indicatorShapes[0] = indicatorInstance.GetComponent<ShapeRenderer>();
        indicatorShapes[1] = indicatorInstance.transform.Find("tri").GetComponentInChildren<ShapeRenderer>();
            originalColor = indicatorShapes[0].Color;
       
    }

        private void Update()
        {
            if (enemy == null)
            {
                indicatorInstance.SetActive(false);
                return;
            }

            // Calculate the direction from player to enemy.
            Vector3 direction = (enemy.position - player.position).normalized;
       
            // Check if the enemy is within the arrow's display distance.
            float dist =  Vector3.Distance(player.position, enemy.position);
            if (dist <= arrowDistance  )
            {
           
            // Make the indicator visible.
            indicatorInstance.SetActive(true);

                // Calculate the rotation angle to face the enemy.
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                indicatorInstance.transform.rotation = Quaternion.Slerp(indicatorInstance.transform.rotation, rotation, Time.deltaTime * arrowRotationSpeed);
                
        
        }
            else
            {
                // Hide the indicator if the enemy is too far away.
                indicatorInstance.SetActive(false);
            }

        //if (objectToCheck != null)
        //{
        //    // Calculate the direction from the object to the target direction.
        //    Vector2 objectToDirection = (directionToCheck - (Vector2)objectToCheck.position).normalized;

        //    // Get the forward direction of the object (in 2D, it's the right vector).
        //    Vector2 objectForward = objectToCheck.right;

        //    // Calculate the dot product to check if the object is facing the direction.
        //    float dotProduct = Vector2.Dot(objectToDirection, objectForward);

        //    isFacingDirection = dotProduct > facingThreshold;
        //}
        /////////////////////////////////////////////////////////////////////////////
        ///

        if (Vector2.Dot(direction, enemy.right) <= -0.5f)
        {
            Debug.Log("facing you ");
            // Calculate the alpha value using PingPong to create a flashing effect.
            float alpha = Mathf.PingPong(Time.time * flashSpeed, 1f);

            // Set the sprite's color with the flash alpha.
            Color flashAlphaColor = new Color(flashColor.r, flashColor.g, flashColor.b, alpha);
            foreach (var item in indicatorShapes)
            {
                item.Color = flashAlphaColor;
            }
            
        }
        else
        {
            // Return to the default alpha when not flashing.
            foreach (var item in indicatorShapes)
            {
                item.Color = originalColor;
            }
        }
    }

   


    public void StartFlash()
    {
        isFlashing = true;
    }

    public void StopFlash()
    {
        isFlashing = false;
    }
}

