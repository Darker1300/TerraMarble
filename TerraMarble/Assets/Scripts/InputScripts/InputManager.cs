using System;
using System.Collections;
using System.Numerics;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Vector2 = UnityEngine.Vector2;

public class InputManager : MonoBehaviour
{

    public delegate void DragLeft(bool state);
    //Left Right Vectors
    public bool Mobile;

    //if mobile when input starts dictates which side its on,
    //if dragleft 
    public enum InputIdentifier
    {
        IsLeft, isRight, Undeclared
    }
    InputIdentifier leftMouseDragID;
    InputIdentifier rightMouseDragID;
    //---DRAG MOUSE/TOUCH--
    //START POS


    //CURRENT DRAG SCREENPOS
    /// <param name="dragVector">World-space vector</param>
    /// <param name="dragDelta">World-space vector</param>
    /// <param name="screenDragVector">-1..1 vector, viewport space</param>
    public delegate void DragLeftUpdate(Vector2 dragVector, Vector2 dragDelta, Vector2 screenDragVector);

    /// <param name="dragVector">World-space vector</param>
    /// <param name="dragDelta">World-space vector</param>
    /// <param name="screenDragVector">-1..1 vector, viewport space</param>
    public delegate void DragRightUpdate(Vector2 dragVector, Vector2 dragDelta, Vector2 screenDragVector);

    public static Vector2 DragLeftStartScreenPos;
    public static Vector2 DragLeftEndScreenPos;

    public static Vector2 DragRightStartScreenPos;
    public static Vector2 DragRightEndScreenPos;

    public static Vector2 DragLeftScreenVector
        => (DragLeftEndScreenPos - DragLeftStartScreenPos) / ScreenSize;
    public static Vector2 DragRightScreenVector
        => (DragRightEndScreenPos - DragRightStartScreenPos) / ScreenSize;

    public static Vector2 ScreenSize
        => new(Screen.width, Screen.height);

    public static Vector2 ScreenWorldSize
        => new(
            (Camera.main.orthographicSize * 2.0f) * Camera.main.aspect,
            Camera.main.orthographicSize * 2.0f);

    [SerializeField] private InputModule inputAsset;
    public float startTime;
    public float minDragAmount;
    [SerializeField] float TapMinDragAmount = 20;
    private float lastTapTime;
    private bool tapTimer;
    IEnumerator DoubleTapQuery;
    [SerializeField] private float QueryTimer;


    public enum DragTypes
    {
        CONTROLLER,
        STANDARD,
        ALTERNATE
    }

    [EnumFlags]
    public DragTypes testID = DragTypes.ALTERNATE;

    [SerializeField] private DragTypes leftDragState = DragTypes.CONTROLLER;
    [SerializeField] private DragTypes rightDragState = DragTypes.CONTROLLER;
    private int screenWidth;

    public bool showDebug = false;
    public float TapTime = 0.2f;
    [SerializeField] private float holdTime;
    [SerializeField] private float doubleTapTimeThreshold = 0.2f;

    public static event DragLeftUpdate LeftDragVectorEvent;
    public static event DragLeftUpdate LeftAlternateDragVectorEvent;


    public static event DragRightUpdate RightDragVectorEvent;
    public static event DragRightUpdate RightAlternateDragVectorEvent;
    public static event DragLeft LeftDragEvent;
    public static event DragLeft RightDragEvent;
    public bool hasMoved;
    //will return false if canceled,this
    public static Action TapLeft;
    public static Action TapRight; 
    public static Action DoubleTapLeft;
    public static Action DoubleTapRight;

    
    public static event EventHandler LeftTap;
    public static event EventHandler RightTap;

    //TAP 
    //public static event EventHandler TapLeftEvent;

    //public static event EventHandler TapRightEvent;
    

    public void OnEnable()

    {

        inputAsset?.Enable();

        screenWidth = Screen.width;

        //inputAsset = new InputAsset();

        inputAsset?.Player.Enable();

        inputAsset?.UI.Disable();
    }

    public void DragStateConfigure(Vector2 drag, Vector2 dragMag)
    {

    }
    //public bool IsBothInput()
    //{
    //    if (Mobile)
    //    {

    //    }
    //}
    public void Start()

    {
        inputAsset.Player.DragLeft.started +=
            ctx =>

            {
                //Debug.Log("dragStarttttttttted");
                if (Mobile )
                {

                    //if position is left side OR right side
                    if (IsTouchRight(ctx.ReadValue<Vector2>()))
                    {
                        leftMouseDragID = InputIdentifier.isRight;

                        startTime = Time.time;
                        DragRightStartScreenPos = ctx.ReadValue<Vector2>();
                        DragRightEndScreenPos = DragRightStartScreenPos;
                        RightDragEvent?.Invoke(true);
                    }
                    else //touch left
                    {
                        leftMouseDragID = InputIdentifier.IsLeft;
                        startTime = Time.time;
                        DragLeftStartScreenPos = ctx.ReadValue<Vector2>();
                        DragLeftEndScreenPos = DragLeftStartScreenPos;
                        LeftDragEvent?.Invoke(true);

                    }



                }
                else //PC
                {



                    startTime = Time.time;

                    DragLeftStartScreenPos = ctx.ReadValue<Vector2>();
                    DragLeftEndScreenPos = DragLeftStartScreenPos;

                    if (showDebug) Debug.Log("Start");

                    LeftDragEvent?.Invoke(true);

                }


            };


        //drag being performed

        inputAsset.Player.DragLeft.performed +=
            ctx =>

            {

                if (Mobile)
                {
                    //is it the left check start value if true pass info to drag left switch


                    if (leftMouseDragID == InputIdentifier.IsLeft)
                    {
                        InputLeftSwitch(ctx.ReadValue<Vector2>(), ref leftDragState);
                    }
                    else
                    {
                        InputRightSwitch(ctx.ReadValue<Vector2>(), ref rightDragState);

                    }
                }
                else
                //its pc and is left 
                {
                    InputLeftSwitch(ctx.ReadValue<Vector2>(), ref leftDragState);
                }
                //Left start time and left alternative event
                //work out which one you are working with




            };



        inputAsset.Player.DragLeft.canceled +=
            ctx =>
            {
                if (Mobile)
                {
                    //is it the left check start value if true pass info to drag left switch


                    if (leftMouseDragID == InputIdentifier.IsLeft)
                    {
                        DragLeftEnd(ctx.ReadValue<Vector2>());
                    }
                    else
                    {
                        DragRightEnd(ctx.ReadValue<Vector2>());



                    }
                    leftMouseDragID = InputIdentifier.Undeclared;
                }
                else
                //its pc and is left 
                {
                    DragLeftEnd(ctx.ReadValue<Vector2>());
                }
                //if below drag threshold 

                //how long since held down? is it a tap


                //LeftDragEvent.Invoke(Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>()));   
            };



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        inputAsset.Player.DragRight.started +=
            ctx =>

            {
                if (Mobile)
                {
                    //if position is left side OR right side
                    if (IsTouchRight(ctx.ReadValue<Vector2>()))
                    {
                        rightMouseDragID = InputIdentifier.isRight;

                        startTime = Time.time;
                        DragRightStartScreenPos = ctx.ReadValue<Vector2>();
                        DragRightEndScreenPos = DragRightStartScreenPos;
                        RightDragEvent?.Invoke(true);
                    }
                    else
                    {
                        rightMouseDragID = InputIdentifier.IsLeft;
                        startTime = Time.time;
                        DragLeftStartScreenPos = ctx.ReadValue<Vector2>();
                        DragLeftEndScreenPos = DragLeftStartScreenPos;
                        LeftDragEvent?.Invoke(true);

                    }



                }
                else //PC
                {
                    startTime = Time.time;


                    DragRightStartScreenPos = ctx.ReadValue<Vector2>();

                    DragRightEndScreenPos = DragRightStartScreenPos;

                    //if (showDebug) Debug.Log("Start");

                    RightDragEvent?.Invoke(true);
                }



            };


        //drag being performed

        inputAsset.Player.DragRight.performed +=
            ctx =>
            {
                if (Mobile)
                {
                    //is it the left check start value if true pass info to drag left switch


                    if (rightMouseDragID == InputIdentifier.IsLeft)
                    {
                        InputLeftSwitch(ctx.ReadValue<Vector2>(), ref leftDragState);
                    }
                    else
                    {
                        InputRightSwitch(ctx.ReadValue<Vector2>(), ref rightDragState);

                    }
                }
                else
                //its pc and is left 
                {
                    InputRightSwitch(ctx.ReadValue<Vector2>(), ref rightDragState);
                }
            };


        inputAsset.Player.DragRight.canceled +=
            ctx =>

            {
                if (Mobile)
                {
                    //is it the left check start value if true pass info to drag left switch


                    if (rightMouseDragID == InputIdentifier.IsLeft)
                    {
                        DragLeftEnd(ctx.ReadValue<Vector2>());

                    }
                    else
                    {
                        DragRightEnd(ctx.ReadValue<Vector2>());



                    }
                    rightMouseDragID = InputIdentifier.Undeclared;
                }
                else
                //its pc and is left 
                {
                    DragRightEnd(ctx.ReadValue<Vector2>());
                }
            };
    }

    public void InputLeftSwitch(Vector2 drag, ref DragTypes dragtype)
    {

        Vector2 dragCurrentScreenPos = drag;
        Vector2 dragCurrentWorldPos = Camera.main.ScreenToWorldPoint(dragCurrentScreenPos);
        Vector2 dragStartWorldPos = Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos);
        Vector2 dragEndWorldPos = Camera.main.ScreenToWorldPoint(DragLeftEndScreenPos);

        switch (dragtype)
        {
            case DragTypes.CONTROLLER:

                //if bellow hold time and magnitude is less then threshold 
                if (
                    (dragStartWorldPos -
                     dragCurrentWorldPos).magnitude > minDragAmount)
                {
                    //if less than threshold

                    dragtype = DragTypes.STANDARD;


                    // if (showDebug) Debug.Log("tap: " + (Time.time - startTime));


                    break;
                }



                break;
            case DragTypes.STANDARD:
                DragLeftEndScreenPos = dragCurrentScreenPos;
                LeftDragVectorEvent?.Invoke(
                    dragCurrentWorldPos - dragStartWorldPos,
                    dragCurrentWorldPos - dragEndWorldPos,
                    DragLeftScreenVector);

                break;
            case DragTypes.ALTERNATE:
                DragLeftEndScreenPos = dragCurrentScreenPos;
                LeftAlternateDragVectorEvent?.Invoke(
                    dragCurrentWorldPos - dragStartWorldPos,
                    dragCurrentWorldPos - dragEndWorldPos,
                    DragLeftScreenVector);
                break;
            default:
                break;
        }
    }
    public void InputRightSwitch(Vector2 drag, ref DragTypes dragtype)
    {

        Vector2 dragCurrentScreenPos = drag;
        Vector2 dragCurrentWorldPos = Camera.main.ScreenToWorldPoint(dragCurrentScreenPos);
        Vector2 dragStartWorldPos = Camera.main.ScreenToWorldPoint(DragRightStartScreenPos);
        Vector2 dragEndWorldPos = Camera.main.ScreenToWorldPoint(DragRightEndScreenPos);

        switch (dragtype)
        {
            case DragTypes.CONTROLLER:

                //if bellow hold time and magnitude is less then threshold 
                if ((dragStartWorldPos - dragCurrentWorldPos).magnitude > minDragAmount)
                {
                    //if less than threshold

                    dragtype = DragTypes.STANDARD;


                    // if (showDebug) Debug.Log("tap: " + (Time.time - startTime));


                    break;
                }



                break;
            case DragTypes.STANDARD:
                DragRightEndScreenPos = dragCurrentScreenPos;
                RightDragVectorEvent?.Invoke(
                    dragCurrentWorldPos - dragStartWorldPos,
                    dragCurrentWorldPos - dragEndWorldPos,
                    DragRightScreenVector);

                break;
            case DragTypes.ALTERNATE:
                DragRightEndScreenPos = dragCurrentScreenPos;
                RightAlternateDragVectorEvent?.Invoke(
                    dragCurrentWorldPos - dragStartWorldPos,
                    dragCurrentWorldPos - dragEndWorldPos,
                    DragRightScreenVector);
                break;
            default:
                break;
        }
    }

    public void DragLeftEnd(Vector2 drag)
    {

        Vector2 dragStartWorldPos = Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos);
        Vector2 dragCurrentScreenPos = drag;
        Vector2 dragCurrentWorldPos = Camera.main.ScreenToWorldPoint(dragCurrentScreenPos);
        //Debug.Log("Also Working");
        if (Time.time - lastTapTime < doubleTapTimeThreshold)
        {

            TapLeft?.Invoke();
            //coroutine 
            if (DoubleTapQuery != null)
            {
                StopCoroutine(DoubleTapQuery);
                DoubleTapQuery = null;
            }
            Debug.Log("doubleTap");
            DoubleTapLeft?.Invoke();
        }
        else if ((Time.time - startTime) <= TapTime)
        {
           

            //
            //TapLeftEvent?.Invoke(null, EventArgs.Empty);
            DoubleTapQuery = DoubleTapTimer();
            StartCoroutine(DoubleTapQuery);
            TapLeft?.Invoke();
            lastTapTime = Time.time;
        }


        {
            if (showDebug) Debug.Log("Drag End");

            LeftDragEvent?.Invoke(false);



            DragLeftStartScreenPos = Vector2.zero;
        }

        leftDragState = DragTypes.CONTROLLER;
    }
    public void DragRightEnd(Vector2 drag)
    {

        Vector2 dragStartWorldPos = Camera.main.ScreenToWorldPoint(DragRightStartScreenPos);
        Vector2 dragCurrentScreenPos = drag;
        Vector2 dragCurrentWorldPos = Camera.main.ScreenToWorldPoint(dragCurrentScreenPos);
        if (Time.time - lastTapTime < doubleTapTimeThreshold)
        {
            TapRight?.Invoke();
            //coroutine 
            if (DoubleTapQuery != null)
            {
                StopCoroutine(DoubleTapQuery);
                DoubleTapQuery = null;
            }
            DoubleTapRight?.Invoke();
            Debug.Log("doubleTap");
        }
        else if ((Time.time - startTime) <= TapTime)
        {
            TapRight?.Invoke();

            //
            //TapLeftEvent?.Invoke(null, EventArgs.Empty);
            DoubleTapQuery = DoubleTapTimer();
            StartCoroutine(DoubleTapQuery);

            lastTapTime = Time.time;
        }


        {
            if (showDebug) Debug.Log("Drag End");

            RightDragEvent?.Invoke(false);



            DragRightStartScreenPos = Vector2.zero;
        }

        rightDragState = DragTypes.CONTROLLER;
    }
    private IEnumerator DoubleTapTimer()
    {

        yield return new WaitForSeconds(QueryTimer);
        //while (timer > 0f)
        //{
        //    if (tap)
        //    {

        //    }
        //    yield return null;

        //    timer -= Time.deltaTime;
        //}
        TapRight?.Invoke();
        if (showDebug) Debug.Log("tap: " + (Time.time - startTime));
    }
   

    private void MultiTapActivated()
    {
        //if (showDebug) Debug.Log("MultiTap");
    }


    private void Tap()
    {
    }


    private void SlowTapRelease()
    {
        //if (showDebug) Debug.Log("SlowTapRelease");
    }


    private void SlowTapStarted()
    {
        //if (showDebug) Debug.Log("SlowTapStarted");
    }


    public void OnDisable()
    {
        inputAsset?.Disable();
    }

    // Start is called before the first frame update


    // Update is called once per frame

    private void Update()
    {

    }


    public bool IsTouchRight(Vector2 ScreenPos)
    {
        if (ScreenPos.x < screenWidth / 2)

            return false;

        //The User has touched on the left side of the screen

        return true;
    }

    //public delegate void LeftUp(Vector2 screenPosition, bool State);
    //public event TapLeftPos LeftUpEvent;

    //public delegate void RightUp(Vector2 screenPosition, bool State);
    //public event TapLeftPos RightUpEvent;
    
    #region Singleton

    private Vector2 mouseDelta;
    private bool tap;


    public static InputManager Instance { get; private set; }

    private void Awake()
    {
        EnhancedTouchSupport.Enable();
        // Initialize Singleton
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        inputAsset = new InputModule();
    }

    #endregion
}


