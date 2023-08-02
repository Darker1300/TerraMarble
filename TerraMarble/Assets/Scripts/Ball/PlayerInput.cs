using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityUtility;
using Vector2 = UnityEngine.Vector2;

public class PlayerInput : MonoBehaviour
{
    // References

    // Private Fields
    [ShowInInspector] private Vector2 rawDrag = Vector2.zero;
    [ShowInInspector] private int rawSide = 1;

    // Influences
    [SerializeField]
    [Tooltip("percentage of screen used for touch drag input, originating from touch position")]
    private Vector2 dragScreenSize = new(0.05f, 0.125f);

    //[SerializeField] [Tooltip("how much of the screen that the drag needs before setting drag dir")]
    //private Vector2 dragDirTolerance = new(0.1f, 0.05f);

    [SerializeField] private bool applyInputCurve = true;
    [SerializeField] private AnimationCurve inputCurve
        = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private bool invertXDrag = false;
    [SerializeField] private bool invertSide = false;
    [SerializeField] private bool swapXDragOutwards = false;
    [SerializeField] private bool swapXDragInwards = true;
    [SerializeField] private bool invertXDragPost = false;
    [SerializeField] private bool invertSidePost = false;
    [SerializeField] private bool isDragging = false;

    // Properties
    [ShowInInspector] public Vector2 Drag => GetDrag();
    [ShowInInspector] public int Side => GetSide();
    [ShowInInspector] public Vector2 RawDrag => rawDrag;
    [ShowInInspector] public int RawSide => rawSide;

    [ShowInInspector] public bool IsDragging
    {
        get => isDragging;
        private set => isDragging = value;
    }

    public Vector2 DragScreenSize
    {
        get => dragScreenSize;
        private set => dragScreenSize = value;
    }

    private void Start()
    {
        InputManager.LeftDragEvent
            += (a) => OnDragToggle(a, -1);
        InputManager.RightDragEvent
            += (a) => OnDragToggle(a, 1);
        InputManager.LeftDragVectorEvent
            += (a, b, screenDrag) => OnDragUpdate(screenDrag, -1);
        InputManager.RightDragVectorEvent
            += (a, b, screenDrag) => OnDragUpdate(screenDrag, 1);
    }

    private void OnDragToggle(bool state, int side)
    {
        IsDragging = state;

        if (IsDragging) // On Down
            rawSide = side;
        else // On Up
            rawDrag = Vector2.zero;
    }

    private void OnDragUpdate(Vector2 screenDragVector, int side)
    {
        rawDrag = CalcRawDrag(screenDragVector, dragScreenSize);
    }

    private Vector2 GetDrag()
    {
        Vector2 drag = rawDrag;
        if (applyInputCurve)
            drag = ApplyDragInputCurve(drag, inputCurve);
        drag.x *= NegIf(invertXDrag);
        drag.x *= NegIf(swapXDragOutwards && Math.Sign(rawDrag.x) == rawSide);
        drag.x *= NegIf(swapXDragInwards && Math.Sign(rawDrag.x) != rawSide);
        drag.x *= NegIf(invertXDragPost);

        return drag;
    }

    private int GetSide()
    {
        int side = rawSide;
        side *= NegIf(invertSide);
        side *= NegIf(swapXDragOutwards && Math.Sign(rawDrag.x) == rawSide);
        side *= NegIf(swapXDragInwards && Math.Sign(rawDrag.x) != rawSide);
        side *= NegIf(invertSidePost);
        return side;
    }


    private static Vector2 CalcRawDrag(Vector2 _screenDragVector, Vector2 _dragScreenSize)
    {
        return new(
            Mathf.Clamp(_screenDragVector.x / _dragScreenSize.x, -1f, 1f),
            Mathf.Clamp(_screenDragVector.y / _dragScreenSize.y, -1f, 1f)
        //Mathf.Abs(Mathf.Clamp(_screenDragVector.y / _dragScreenSize.y, -1f, 0f))
        );
    }

    private static Vector2 ApplyDragInputCurve(Vector2 _rawDrag, AnimationCurve _inputCurve)
    {
        float x = Mathf.Abs(_rawDrag.x);
        float y = Mathf.Abs(_rawDrag.y);

        x = _inputCurve.Evaluate(x);
        y = _inputCurve.Evaluate(y);

        _rawDrag.x = x * Mathf.Sign(_rawDrag.x);
        _rawDrag.y = y * Mathf.Sign(_rawDrag.y);

        return _rawDrag;
    }

    private int NegIf(bool shouldInvert)
    {
        return shouldInvert ? -1 : 1;
    }
}