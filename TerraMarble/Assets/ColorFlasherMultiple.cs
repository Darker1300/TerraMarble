using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class ColorFlasherMultiple : MonoBehaviour
{

      [Header("Color Settings")]
    public Color flashColor = new Color(1f, 0f, 0f, 1f); // Color to flash (red in this example).
    public float flashSpeed = 2f; // Speed of the flash effect.
    public float defaultAlpha = 1f; // Default alpha value when not flashing.

    private bool isFlashing = false;
    private Color originalColor;
    public ShapeRenderer[] indicatorShapes;
    public bool test;
    public bool IsFlashing;

    // Start is called before the first frame update
    void Start()
    {
        originalColor = indicatorShapes[0].Color;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsFlashing)
        { 
        float alpha =Mathf.Clamp( Mathf.PingPong(Time.time * flashSpeed, flashColor.a), originalColor.a,flashColor.a);

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
}
