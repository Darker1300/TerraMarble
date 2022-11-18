using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class InputManager : MonoBehaviour
{
    public delegate void DragLeft(bool state);
    //Left Right Vectors


    //---DRAG MOUSE/TOUCH--
    //START POS


    //CURRENT DRAG SCREENPOS
    public delegate void DragLeftUpdate(Vector2 currentScreenPosition, Vector2 Delta);

    public delegate void DragRightUpdate(Vector2 currentScreenPosition, Vector2 Delta);

    public static Vector2 DragLeftStartScreenPos;
    public static Vector2 DragLeftEndScreenPos;

    public static Vector2 DragRightStartScreenPos;

    [SerializeField] private InputModule inputAsset;
    public float LeftStartTime;
    public float minDragAmount;
    public float RightStartTime;


    private int screenWidth;

    public bool showDebug = false;
    public float TapTime;
    public static event DragLeftUpdate LeftDragVectorEvent;
    public static event DragRightUpdate RightDragVectorEvent;
    public static event DragLeft LeftDragEvent;
    public static event DragLeft RightDragEvent;


    //public delegate void DragStart(Vector2 currentScreenPosition);
    //public event DragLeftUpdate LeftDragVectorEvent;

    //public delegate void DragRightUpdate(Vector2 currentScreenPosition);
    //public event DragRightUpdate RightDragVectorEvent;

    //TAP 
    //public delegate void TapLeftPos(Vector2 screenPosition);
    public static event EventHandler TapLeftEvent;

    //public delegate void TapRightPos(Vector2 screenPosition);
    public static event EventHandler TapRightEvent;


    public void OnEnable()

    {
        inputAsset.Enable();

        screenWidth = Screen.width;


        //inputAsset = new InputAsset();

        inputAsset.Player.Enable();

        inputAsset.UI.Disable();


        #region unused

        ////////////////////////////////////////////////////LEFT////////////////////////////////////////////////


        //MOVEMENT BOOST AND sHORT tURN


        //inputAsset.Player.LeftTap.started +=

        //ctx =>

        //{


        //    Debug.Log("click");


        //};

        //inputAsset.Player.LeftClick.started +=

        //ctx =>

        //{

        //    Debug.Log("Down");

        //    if (ctx.interaction is SlowTapInteraction)

        //    {

        //        //if ((Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(inputAsset.Player.DragLeft.ReadValue<Vector2>())).magnitude < minDragAmount)

        //        //{


        //        //}


        //        //TRIGGER SLOW TAP IF DRAG MAG Below ThreshHold

        //        //if ((Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(inputAsset.Player.DragLeft.ReadValue<Vector2>())).magnitude < minDragAmount)

        //        //{

        //        //    Debug.Log("left SlowStart");

        //        //    slowTap = true;

        //        //    SlowTapLeftEvent?.Invoke(true);


        //        //}

        //        //else if(ctx.interaction is not TapInteraction)

        //        //{

        //        //    LeftDragEvent?.Invoke(true);


        //        //    Debug.Log(" Drag Left Started");

        //        //}

        //        //Drag


        //        // Debug.Log((Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(inputAsset.Player.DragLeft.ReadValue<Vector2>())).magnitude);

        //    }

        //};

        ////performed full charge (!have a canceled one to deal )

        //inputAsset.Player.LeftClick.performed +=

        //      ctx =>

        //    {

        //        // get side 

        //        //check if other side is down (Two finger tap)

        //        DragLeftStartScreenPos = Vector2.zero;


        //        //if (ctx.interaction is SlowTapInteraction)

        //        //{

        //        //    //if (slowTap == true)

        //        //    //{

        //        //    //    Debug.Log("Slow left TapFinished");

        //        //    //    SlowTapLeftEvent?.Invoke(false);

        //        //    //    slowTap = false;


        //        //    //}

        //        //    //else if (ctx.interaction is not TapInteraction)

        //        //    //{

        //        //    //    Debug.Log("Drag Left FInished");

        //        //    //    LeftDragEvent?.Invoke(false);


        //        //    //}


        //        //    //Debug.Log("sLowTap" + inputAsset.Player.Drag.ReadValue<Vector2>());


        //        //}

        //        //else

        //        //{

        //        //    Debug.Log("leftTap");


        //        //    TapLeftEvent?.Invoke(null, EventArgs.Empty);


        //        //}


        //    };


        ////////////////////////////////////////////////////RIGHT////////////////////////////////////////////////


        // inputAsset.Player.RightClick.started +=

        //ctx =>

        //{


        //    if (ctx.interaction is SlowTapInteraction)

        //    {

        //        //TRIGGER SLOW TAP IF DRAG MAG Below ThreshHold

        //        //if ((Camera.main.ScreenToWorldPoint(DragRightStartScreenPos) - Camera.main.ScreenToWorldPoint(inputAsset.Player.DragRight.ReadValue<Vector2>())).magnitude < minDragAmount)

        //        //{


        //        //    slowTap = true;

        //        //    SlowTapRightEvent?.Invoke(true);

        //        //    Debug.Log("slow Right tapStart");


        //        //}

        //        //else

        //        //{

        //        //    RightDragEvent?.Invoke(true);


        //        //    Debug.Log("Drag right Event Start");

        //        //    //Drag


        //        //}

        //    }

        //};

        //performed full charge (!have a canceled one to deal )

        //inputAsset.Player.RightClick.performed +=

        //      ctx =>

        //      {

        //          // get side 

        //          //check if other side is down (Two finger tap)


        //          if (ctx.interaction is SlowTapInteraction)

        //          {

        //              if (slowTap == true)

        //              {

        //                  Debug.Log("Slow Tap right finished");

        //                  SlowTapRightEvent?.Invoke(false);

        //                  slowTap = false;


        //              }

        //              else

        //              {

        //                  Debug.Log("drag right finished");

        //                  RightDragEvent?.Invoke(false);


        //              }


        //              //Debug.Log("sLowTap" + inputAsset.Player.Drag.ReadValue<Vector2>());


        //          }

        //          else

        //          {


        //              TapRightEvent?.Invoke(null, EventArgs.Empty);


        //              //Tap();

        //              Debug.Log("RIght Tap");


        //          }


        //      };

        //inputAsset.Player.OneButtonMultiTap.performed +=

        //    ctx =>

        //    {

        //        //Debug.Log("MultiTap");


        //    };

        //FINGER DRAG

        //chache the started pos (for comparing later)

        //on performed check the cur pos with start pos

        //(if moved outside of threshold) then call drag shoot event

        //


        //     inputAsset.Player.TwoDrag.started +=

        //        ctx =>

        //        {


        //            Debug.Log("TwoStarted");


        //        };

        //     inputAsset.Player.TwoDrag.performed +=

        //        ctx =>

        //        {


        //            Debug.Log("TwoDragging");


        //        };

        //     inputAsset.Player.TwoDrag.canceled +=

        //ctx =>

        //{


        //    Debug.Log("TwoDraggingFinished");


        //};

        ////////////////////////////////////////////////////DRAGUPDATE////////////////////////////////////////////////

        //drag mouse

        #endregion
    }


    public void Start()

    {
        inputAsset.Player.DragLeft.started +=
            ctx =>

            {
                LeftStartTime = Time.time;


                DragLeftStartScreenPos = ctx.ReadValue<Vector2>();
                DragLeftEndScreenPos = DragLeftStartScreenPos;

                if (showDebug) Debug.Log("Start");

                LeftDragEvent?.Invoke(true);
            };


        //drag being performed

        inputAsset.Player.DragLeft.performed +=
            ctx =>

            {
                //if (showDebug) Debug.Log("Drag" + (Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>())).magnitude);


                // if (showDebug) Debug.Log("Dragging");

                //DragLeftStartScreenPos = ctx.ReadValue<Vector2>();


                //IF DRAG IS ABOVE X AMOUNT 

                Vector2 currentScreenPos = ctx.ReadValue<Vector2>();

                if ((currentScreenPos - DragLeftStartScreenPos).magnitude > minDragAmount)
                {
                    LeftDragVectorEvent?.Invoke(
                        Camera.main.ScreenToWorldPoint(currentScreenPos) -
                        Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos), mouseDelta);

                    DragLeftEndScreenPos = currentScreenPos;

                    if (showDebug)
                        Debug.Log("Drag" + (Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>()) -
                                            Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos)));
                }
            };

        //DRAG DELTA

        //inputAsset.Player.Delta.performed +=

        //   ctx =>

        //   {


        //       mouseDelta = ctx.ReadValue<Vector2>();

        //      // if (showDebug) Debug.Log("Delta" + ctx.ReadValue<Vector2>());


        //   };


        inputAsset.Player.DragLeft.canceled +=
            ctx =>

            {
                //if below drag threshold 


                //how long since held down? is it a tap

                if (Time.time - LeftStartTime <= TapTime)

                {
                    if (showDebug) Debug.Log(Time.time - LeftStartTime + " : tap");

                    TapLeftEvent?.Invoke(null, EventArgs.Empty);
                }

                else

                {
                    if (showDebug) Debug.Log("Drag End");

                    LeftDragEvent?.Invoke(false);

                    LeftStartTime = 0;

                    DragLeftStartScreenPos = Vector2.zero;
                }


                //LeftDragEvent.Invoke(Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>()));   
            };


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        inputAsset.Player.DragRight.started +=
            ctx =>

            {
                RightStartTime = Time.time;


                DragRightStartScreenPos = ctx.ReadValue<Vector2>();

                if (showDebug) Debug.Log("Start");

                RightDragEvent?.Invoke(true);
            };


        //drag being performed

        inputAsset.Player.DragRight.performed +=
            ctx =>

            {
                //if (showDebug) Debug.Log("Drag" + (Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>())).magnitude);


                // if (showDebug) Debug.Log("Dragging");

                //DragLeftStartScreenPos = ctx.ReadValue<Vector2>();


                //IF DRAG IS ABOVE X AMOUNT 

                //(Vector2)Camera.main.ScreenToWorldPoint(end) - (Vector2)Camera.main.ScreenToWorldPoint(start)


                RightDragVectorEvent?.Invoke(
                    Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>()) - Camera.main.ScreenToWorldPoint(DragRightStartScreenPos
                    ), mouseDelta);

                if (showDebug) Debug.Log("Drag");
            };


        inputAsset.Player.DragRight.canceled +=
            ctx =>

            {
                //if below drag threshold 

                //THIS IS A SAFE HOLD SO QUICK DRAG IS NOT COUNTED AS A TAP

                if ((Camera.main.ScreenToWorldPoint(DragRightStartScreenPos) -
                     Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>())).magnitude < minDragAmount)

                    //how long since held down? is it a tap

                    if (Time.time - RightStartTime <= TapTime)

                    {
                        if (showDebug) Debug.Log(Time.time - RightStartTime + " : tap");

                        TapRightEvent?.Invoke(null, EventArgs.Empty);
                    }

                //LeftDragEvent.Invoke(Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>()));   


                if (showDebug) Debug.Log("Drag End");

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
