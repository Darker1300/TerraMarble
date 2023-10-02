using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityUtility;
using Vector2 = UnityEngine.Vector2;

public class PlayerInput : MonoBehaviour
{
    // References

    // Private Fields
    [ShowInInspector] private int rawSide = 1;
    [ShowInInspector] private Vector2 rawDrag = Vector2.zero;
    [ShowInInspector] private Vector2 rawScreenDrag = Vector2.zero;

    // Influences
    [SerializeField]
    [Tooltip("percentage of screen used for touch drag input, originating from touch position")]
    private Vector2 dragScreenSize = new(0.05f, 0.125f);

    //[SerializeField] [Tooltip("how much of the screen that the drag needs before setting drag dir")]
    //private Vector2 dragDirTolerance = new(0.1f, 0.05f);

    [SerializeField] private bool applyInputCurve = true;
    [SerializeField] private AnimationCurve inputCurve
        = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private bool applyClampInputCurve = true;
    [SerializeField] private AnimationCurve clampInputCurve
        = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField] private bool invertXDrag = false;
    [SerializeField] private bool invertSide = false;
    [SerializeField] private bool swapXDragOutwards = false;
    [SerializeField] private bool swapXDragInwards = true;
    [SerializeField] private bool isDragging = false;

    // Properties
    [ShowInInspector] public Vector2 TreeDrag => GetTreeDrag();
    [ShowInInspector] public int TreeDragSide => GetTreeSide();
    public Vector2 RawDrag => rawDrag;
    public Vector2 RawScreenDrag => rawScreenDrag;
    public int RawSide => rawSide;

    [Header("Config Button")]
    [SerializeField] private int treeInputConfigSelection = 0;
    [SerializeField] private List<string> treeInputConfigNames = new()
    {
        "Directional",
        "Sided"
    };
    [SerializeField] private List<bool[]> treeInputConfigData = new()
    {
        new[] { false, true, false, true },
        new[] { false, true, false, false }
    };
    private const string inputConfigButtonName = "InputConfigButton";
    [SerializeField] private CycleButton treeInputButton;

    [ShowInInspector]
    public bool IsDragging
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

        treeInputButton = treeInputButton != null ? treeInputButton
            : UnityU.FindObjectByName<CycleButton>(inputConfigButtonName, true);

        treeInputButton?.Initialise(treeInputConfigSelection, treeInputConfigNames, UpdateInputConfig);
    }

    private void UpdateInputConfig(string _configOption)
    {
        int index = treeInputConfigNames.IndexOf(_configOption);
        bool[] data = treeInputConfigData[index];

        invertXDrag = data[0];
        invertSide = data[1];
        swapXDragOutwards = data[2];
        swapXDragInwards = data[3];
    }

    private void OnDragToggle(bool state, int side)
    {
        IsDragging = state;

        if (IsDragging) // On Down
            this.rawSide = side;
        else // On Up
            rawDrag = Vector2.zero;

        rawScreenDrag = Vector2.zero;
    }

    private void OnDragUpdate(Vector2 screenDragVector, int side)
    {
        rawDrag = CalcRawDrag(screenDragVector, dragScreenSize);
        rawScreenDrag = screenDragVector;
    }

    private Vector2 GetTreeDrag()
    {
        Vector2 drag = rawDrag;
        if (applyInputCurve)
            drag = ApplyDragInputCurves(drag, inputCurve, clampInputCurve);
        drag.x *= NegIf(invertXDrag);
        drag.x *= NegIf(swapXDragOutwards && Math.Sign(rawDrag.x) == rawSide);
        drag.x *= NegIf(swapXDragInwards && Math.Sign(rawDrag.x) != rawSide);

        return drag;
    }

    private int GetTreeSide()
    {
        int side = this.rawSide;
        side *= NegIf(invertSide);
        side *= NegIf(swapXDragOutwards && Math.Sign(rawDrag.x) == this.rawSide);
        side *= NegIf(swapXDragInwards && Math.Sign(rawDrag.x) != this.rawSide);
        return side;
    }


    private static Vector2 CalcRawDrag(Vector2 _screenDragVector, Vector2 _dragScreenSize)
    {
        return new(
            Mathf.Clamp(_screenDragVector.x / _dragScreenSize.x, -1f, 1f),
            Mathf.Clamp(_screenDragVector.y / _dragScreenSize.y, -1f, 1f)
        );
    }

    private static Vector2 ApplyDragInputCurves(Vector2 _rawDrag, AnimationCurve _inputCurve, AnimationCurve _clampCurve)
    {
        // absolute
        float x = Mathf.Abs(_rawDrag.x);
        float y = Mathf.Abs(_rawDrag.y);

        // smooth curve
        x = _inputCurve.Evaluate(x);
        y = _inputCurve.Evaluate(y);

        // clamp curve
        float newX = _clampCurve.Evaluate(y);
        if (newX > x) x = newX;

        // bring back sign
        _rawDrag.x = x * Mathf.Sign(_rawDrag.x);
        _rawDrag.y = y * Mathf.Sign(_rawDrag.y);

        return _rawDrag;
    }

    private int NegIf(bool shouldInvert)
    {
        return shouldInvert ? -1 : 1;
    }
}