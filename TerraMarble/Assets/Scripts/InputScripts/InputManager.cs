using System;
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
    public float LeftStartTime;
    public float minDragAmount;
    public float RightStartTime;


    public enum DragTypes
    {
        CONTROLLER,
        STANDARD,
        ALTERNATE
    }

    [EnumFlags]
    public DragTypes testID = DragTypes.ALTERNATE;

    [SerializeField] private DragTypes dragTypes = DragTypes.CONTROLLER;
    private int screenWidth;

    public bool showDebug = false;
    public float TapTime;
    [SerializeField] private float holdTime;

    public static event DragLeftUpdate LeftDragVectorEvent;
    public static event DragLeftUpdate LeftAlternateDragVectorEvent;


    public static event DragRightUpdate RightDragVectorEvent;
    public static event DragLeft LeftDragEvent;
    public static event DragLeft RightDragEvent;
    public bool hasMoved;

    public static event EventHandler LeftAlternateEvent;

    //TAP 
    public static event EventHandler TapLeftEvent;

    public static event EventHandler TapRightEvent;


    public void OnEnable()

    {
        inputAsset.Enable();

        screenWidth = Screen.width;

        //inputAsset = new InputAsset();

        inputAsset.Player.Enable();

        inputAsset.UI.Disable();
    }

    public void DragStateConfigure(Vector2 drag, Vector2 dragMag)
    {

    }

    public void Start()

    {
        inputAsset.Player.DragLeft.started +=
            ctx =>

            {
                if (Mobile)
                {
                    //if position is left side OR right side
                    if (IsTouchRight(ctx.ReadValue<Vector2>()))
                    {
                        leftMouseDragID = InputIdentifier.isRight;

                        RightStartTime = Time.time;
                        DragRightStartScreenPos = ctx.ReadValue<Vector2>();
                        DragRightEndScreenPos = DragRightStartScreenPos;
                        RightDragEvent?.Invoke(true);
                    }
                    else //touch left
                    {
                        leftMouseDragID = InputIdentifier.IsLeft;
                        LeftStartTime = Time.time;
                        DragLeftStartScreenPos = ctx.ReadValue<Vector2>();
                        DragLeftEndScreenPos = DragLeftStartScreenPos;
                        LeftDragEvent?.Invoke(true);

                    }



                }
                else //PC
                {



                    LeftStartTime = Time.time;

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
                    if (IsTouchRight(ctx.ReadValue<Vector2>()))
                    {

                    }
                }
                //Left start time and left alternative event
                //work out which one you are working with


                Vector2 dragCurrentScreenPos = ctx.ReadValue<Vector2>();
                Vector2 dragCurrentWorldPos = Camera.main.ScreenToWorldPoint(dragCurrentScreenPos);
                Vector2 dragStartWorldPos = Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos);
                Vector2 dragEndWorldPos = Camera.main.ScreenToWorldPoint(DragLeftEndScreenPos);

                switch (dragTypes)
                {
                    case DragTypes.CONTROLLER:

                        //if bellow hold time and magnitude is less then threshold 
                        if (Time.time - LeftStartTime < holdTime &&
                            (dragStartWorldPos -
                             dragCurrentWorldPos).magnitude > minDragAmount)
                        {
                            //if less than threshold

                            dragTypes = DragTypes.STANDARD;


                            if (showDebug) Debug.Log("tap: " + (Time.time - LeftStartTime));


                            break;
                        }
                        else if (Time.time - LeftStartTime >= holdTime)
                        {
                            dragTypes = DragTypes.ALTERNATE;
                            LeftAlternateEvent?.Invoke(null, EventArgs.Empty);
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
            };


        inputAsset.Player.DragLeft.canceled +=
            ctx =>
            {
                //if below drag threshold 

                //how long since held down? is it a tap

                Vector2 dragStartWorldPos = Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos);
                Vector2 dragCurrentScreenPos = ctx.ReadValue<Vector2>();
                Vector2 dragCurrentWorldPos = Camera.main.ScreenToWorldPoint(dragCurrentScreenPos);

                if (Time.time - LeftStartTime <= TapTime
                    && (dragStartWorldPos - dragCurrentWorldPos).magnitude < minDragAmount)
                {
                    if (showDebug) Debug.Log("tap: " + (Time.time - LeftStartTime));

                    //TapLeftEvent?.Invoke(null, EventArgs.Empty);
                    LeftStartTime = 0;
                }


                {
                    if (showDebug) Debug.Log("Drag End");

                    LeftDragEvent?.Invoke(false);

                    LeftStartTime = 0;

                    DragLeftStartScreenPos = Vector2.zero;
                }

                dragTypes = DragTypes.CONTROLLER;
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
                        leftMouseDragID = InputIdentifier.isRight;

                        RightStartTime = Time.time;
                        DragRightStartScreenPos = ctx.ReadValue<Vector2>();
                        DragRightEndScreenPos = DragRightStartScreenPos;
                        RightDragEvent?.Invoke(true);
                    }
                    else
                    {
                        leftMouseDragID = InputIdentifier.IsLeft;
                        LeftStartTime = Time.time;
                        DragLeftStartScreenPos = ctx.ReadValue<Vector2>();
                        DragLeftEndScreenPos = DragLeftStartScreenPos;
                        LeftDragEvent?.Invoke(true);

                    }



                }
                else //PC
                {
                    RightStartTime = Time.time;


                    DragRightStartScreenPos = ctx.ReadValue<Vector2>();

                    DragRightEndScreenPos = DragRightStartScreenPos;

                    if (showDebug) Debug.Log("Start");

                    RightDragEvent?.Invoke(true);
                }



            };


        //drag being performed

        inputAsset.Player.DragRight.performed +=
            ctx =>
            {
                if (Mobile)
                {
                    if (IsTouchRight(ctx.ReadValue<Vector2>()))
                    {

                    }
                }




                //PC
                Vector2 dragCurrentScreenPos = ctx.ReadValue<Vector2>();
                Vector2 dragCurrentWorldPos = Camera.main.ScreenToWorldPoint(dragCurrentScreenPos);
                Vector2 dragStartWorldPos = Camera.main.ScreenToWorldPoint(DragRightStartScreenPos);
                Vector2 dragEndWorldPos = Camera.main.ScreenToWorldPoint(DragRightEndScreenPos);

                DragRightEndScreenPos = dragCurrentScreenPos;
                RightDragVectorEvent?.Invoke(
                    dragCurrentWorldPos - dragStartWorldPos,
                    dragCurrentWorldPos - dragEndWorldPos,
                    DragRightScreenVector);
                if (showDebug) Debug.Log("Drag");
            };


        inputAsset.Player.DragRight.canceled +=
            ctx =>

            {
                //how long since held down? is it a tap

                if (Time.time - RightStartTime <= TapTime)

                {
                    if (showDebug) Debug.Log("tap: " + (Time.time - LeftStartTime));

                    TapRightEvent?.Invoke(null, EventArgs.Empty);
                    RightStartTime = 0;
                    DragRightStartScreenPos = Vector2.zero;
                }

                RightDragEvent?.Invoke(false);

                RightStartTime = 0;

                DragRightStartScreenPos = Vector2.zero;
            };
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
        inputAsset.Disable();
    }

    // Start is called before the first frame update


    // Update is called once per frame

    private void Update()
    {
    }


    public bool IsTouchRight(Vector2 ScreenPos)
    {
        if (ScreenPos.x < screenWidth / 2)

            return true;

        //The User has touched on the left side of the screen

        return false;
    }

    //public delegate void LeftUp(Vector2 screenPosition, bool State);
    //public event TapLeftPos LeftUpEvent;

    //public delegate void RightUp(Vector2 screenPosition, bool State);
    //public event TapLeftPos RightUpEvent;

    #region Singleton

    private Vector2 mouseDelta;

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