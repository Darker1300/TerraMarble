using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Shapes;
using UnityEngine;
using UnityEngine.Events;

public class ToggleColors : Toggle
{
    [Serializable]
    public class ShapeColorPair
    {
        public enum ColorType
        {
            SINGLE,
            TRI
        }

        public Shapes.ShapeRenderer shape;
        public ColorType colorType;
        public Color colorStart;
        [Foldout("Tri Color")] public Color colorStartB;
        [Foldout("Tri Color")] public Color colorStartC;
        public Color colorEnd;
        [Foldout("Tri Color")] public Color colorEndB;
        [Foldout("Tri Color")] public Color colorEndC;
        public bool Toggled;
    }

    public ShapeColorPair[] ShapeRenderers;

    [Button]
    public void ToggleAllColor()
    {
        for (var index = 0; index < ShapeRenderers.Length; index++)
        {
            ShapeColorPair shapeColorPair = ShapeRenderers[index];

            if (shapeColorPair.shape == null) continue;

            // Single
            shapeColorPair.shape.Color = shapeColorPair.Toggled ? shapeColorPair.colorStart : shapeColorPair.colorEnd;
            // Triangle
            if (shapeColorPair.colorType == ShapeColorPair.ColorType.TRI)
            {
                Triangle triShape = (Triangle)shapeColorPair.shape;
                if (triShape.ColorMode == Triangle.TriangleColorMode.PerCorner)
                {
                    triShape.ColorB = shapeColorPair.Toggled ? shapeColorPair.colorStartB : shapeColorPair.colorEndB;
                    triShape.ColorC = shapeColorPair.Toggled ? shapeColorPair.colorStartC : shapeColorPair.colorEndC;
                }
            }
            // Toggle
            ShapeRenderers[index].Toggled = !shapeColorPair.Toggled;
        }

        if (ShapeRenderers[0] != null)
        {
            DoToggle();
        }
    }

    [Header("Config")]
    public Color defaultEndColor = Color.gray;

    [Button]
    public void GetStartColors()
    {
        for (var index = 0; index < ShapeRenderers.Length; index++)
        {
            ShapeColorPair shapeColorPair = ShapeRenderers[index];
            if (shapeColorPair.shape == null) continue;
            // Single
            shapeColorPair.colorStart = shapeColorPair.shape.Color;
            // Triangle
            if (shapeColorPair.colorType == ShapeColorPair.ColorType.TRI)
            {
                Triangle triShape = (Triangle)shapeColorPair.shape;
                if (triShape.ColorMode == Triangle.TriangleColorMode.PerCorner)
                {
                    shapeColorPair.colorStartB = triShape.ColorB;
                    shapeColorPair.colorStartC = triShape.ColorC;
                }
            }
        }
    }

    [Button]
    public void SetDefaultEndColor()
    {
        for (var index = 0; index < ShapeRenderers.Length; index++)
        {
            ShapeColorPair shapeColorPair = ShapeRenderers[index];
            shapeColorPair.colorEnd
                = shapeColorPair.colorEndB
                    = shapeColorPair.colorEndC
                        = defaultEndColor;
        }
    }
}
