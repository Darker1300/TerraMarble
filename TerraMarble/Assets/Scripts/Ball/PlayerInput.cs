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
    [ShowInInspector] private Vector2 rawDrag = Vector2.zero;
    [ShowInInspector] private int side = 1;

    // Influences
    [SerializeField]
    [Tooltip("percentage of screen used for touch drag input, originating from touch position")]
    private Vector2 dragScreenSize = new(0.05f, 0.125f);

    //[SerializeField] [Tooltip("how much of the screen that the drag needs before setting drag dir")]
    //private Vector2 dragDirTolerance = new(0.1f, 0.05f);

    [SerializeField] private bool applyInputCurve = true;

    [SerializeField]
    private AnimationCurve inputCurve
        = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private bool invertXDrag = false;
    [SerializeField] private bool invertSide = false;
    [SerializeField] private bool swapXDragOutwards = false;
    [SerializeField] private bool swapXDragInwards = true;
    [SerializeField] private bool isDragging = false;

    // Properties
    [ShowInInspector] public Vector2 TreeDrag => GetDrag();
    [ShowInInspector] public int TreeDragSide => GetSide();
    [ShowInInspector] public Vector2 Drag
        => applyInputCurve ? ApplyDragInputCurve(rawDrag, inputCurve) : rawDrag;
    public Vector2 RawDrag => rawDrag;
    public int Side => side;

    [Serializable] public class InputConfigOption
    {
        public string name = "Option";
        public bool[] data = new bool[4];
    }

    [SerializeField] private int treeInputConfigSelection = 0;
    [SerializeField] private List<InputConfigOption> treeInputConfig;
    [SerializeField] private TextMeshProUGUI treeInputTextUI;
    private const string inputConfigButtonName = "Input Config Text";

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

        treeInputTextUI = treeInputTextUI != null ? treeInputTextUI
            : UnityU.FindObjectByName<TextMeshProUGUI>(inputConfigButtonName, true);

        if (treeInputTextUI != null && treeInputConfig.Count > 0)
        {
            treeInputTextUI.text = treeInputConfig[treeInputConfigSelection].name;
            treeInputTextUI.GetComponentInParent<Button>(true)?.onClick.AddListener(SetNextInputConfigOption);
        }
    }

    public void SetNextInputConfigOption()
    {

        treeInputConfigSelection++;
        if (treeInputConfigSelection >= treeInputConfig.Count)
            treeInputConfigSelection = 0;

        invertXDrag = treeInputConfig[treeInputConfigSelection].data[0];
        invertSide = treeInputConfig[treeInputConfigSelection].data[1];
        swapXDragOutwards = treeInputConfig[treeInputConfigSelection].data[2];
        swapXDragInwards = treeInputConfig[treeInputConfigSelection].data[3];

        treeInputTextUI.text = treeInputConfig[treeInputConfigSelection].name;
    }

    private void OnDragToggle(bool state, int side)
    {
        IsDragging = state;

        if (IsDragging) // On Down
            this.side = side;
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
        drag.x *= NegIf(swapXDragOutwards && Math.Sign(rawDrag.x) == side);
        drag.x *= NegIf(swapXDragInwards && Math.Sign(rawDrag.x) != side);

        return drag;
    }

    private int GetSide()
    {
        int side = this.side;
        side *= NegIf(invertSide);
        side *= NegIf(swapXDragOutwards && Math.Sign(rawDrag.x) == this.side);
        side *= NegIf(swapXDragInwards && Math.Sign(rawDrag.x) != this.side);
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