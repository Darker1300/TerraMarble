using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.InputSystem.Interactions;

public class TestInputNew : MonoBehaviour
{

    private InputModule inputAsset;
    //Output Variables

    public Vector2 DragLeftStartScreenPos;
    public Vector2 DragRightStartScreenPos;
    public float LeftStartTime; 
    public float RightStartTime;
    public float TapTime;
 


    private int screenWidth;
    public float minDragAmount;
    //Left Right Vectors


    //---DRAG MOUSE/TOUCH--
    //START POS
    public delegate void StartScreenPos(Vector2 StartScreenPos);
    public event StartScreenPos StartPosEvent;




    //CURRENT DRAG SCREENPOS
    public delegate void DragLeftUpdate(Vector2 currentScreenPosition);
    public event DragLeftUpdate LeftDragVectorEvent;

    public delegate void DragRightUpdate(Vector2 currentScreenPosition);
    public event DragRightUpdate RightDragVectorEvent;

    public delegate void DragLeft(bool state);
    public event DragLeft LeftDragEvent;
    public event DragLeft RightDragEvent;


    //public delegate void DragStart(Vector2 currentScreenPosition);
    //public event DragLeftUpdate LeftDragVectorEvent;

    //public delegate void DragRightUpdate(Vector2 currentScreenPosition);
    //public event DragRightUpdate RightDragVectorEvent;

    //TAP 
    //public delegate void TapLeftPos(Vector2 screenPosition);
    public event EventHandler TapLeftEvent;

    //public delegate void TapRightPos(Vector2 screenPosition);
    public event EventHandler TapRightEvent;

    //public delegate void LeftUp(Vector2 screenPosition, bool State);
    //public event TapLeftPos LeftUpEvent;

    //public delegate void RightUp(Vector2 screenPosition, bool State);
    //public event TapLeftPos RightUpEvent;


    public delegate void TwoFingerTap(bool State);
    public event TwoFingerTap twoFingerTapEvent;

    //SLOW TAPS
    public delegate void SlowTapActivate(bool state);
    public event SlowTapActivate SlowTapLeftEvent;


    public event SlowTapActivate SlowTapRightEvent;
    //two finger drag 

    //



    //--TOUCH MOUSE/TOUCH--
    //tap screen event
    public delegate void TapScreen();
    public event TapScreen TapEvent;

    //Hold Down
    public delegate void HoldStart();//pos
    public event HoldStart HoldDownStartEvent;

    //Hold End
    public delegate void HoldRelease();
    public event HoldRelease HoldDownEndEvent;

    //Hold 




    public void OnEnable()
    {


        screenWidth = Screen.width;


        inputAsset = new InputModule();
        inputAsset.Player.Enable();
        inputAsset.UI.Disable();



        //////////////////////////////////////////////////DRAGUPDATE////////////////////////////////////////////////
        //drag mouse
        inputAsset.Player.DragLeft.started +=
            ctx =>
            {

                LeftStartTime = Time.time;

                DragLeftStartScreenPos = ctx.ReadValue<Vector2>();
                Debug.Log("Start");
                LeftDragEvent?.Invoke(false);


            };


        //drag being performed
        inputAsset.Player.DragLeft.performed +=
            ctx =>
            {

                //IF DRAG IS ABOVE X AMOUNT 


                LeftDragVectorEvent?.Invoke(Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>()));
                Debug.Log("Drag");



            };

        inputAsset.Player.DragLeft.canceled +=
            ctx =>
            {

                //if below drag threshold 
                //THIS IS A SAFE HOLD SO QUICK DRAG IS NOT COUNTED AS A TAP
                if ((Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>())).magnitude < minDragAmount)
                {

                    //how long since held down? is it a tap
                    if ((Time.time - LeftStartTime) <= TapTime)
                    {
                        Debug.Log((Time.time - LeftStartTime) + " : tap");
                        TapLeftEvent?.Invoke(null, EventArgs.Empty);
                    }

                    //LeftDragEvent.Invoke(Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>()));   
                }


                Debug.Log("Drag End");
                LeftDragEvent?.Invoke(false);
                LeftStartTime = 0;
                DragLeftStartScreenPos = Vector2.zero;



            };




        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        inputAsset.Player.DragRight.started +=
           ctx =>
           {

               RightStartTime = Time.time;

               DragRightStartScreenPos = ctx.ReadValue<Vector2>();
               Debug.Log("Start");
               RightDragEvent?.Invoke(false);


           };


        //drag being performed
        inputAsset.Player.DragRight.performed +=
            ctx =>
            {
                //Debug.Log("Drag" + (Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>())).magnitude);


                // Debug.Log("Dragging");
                //DragLeftStartScreenPos = ctx.ReadValue<Vector2>();

                //IF DRAG IS ABOVE X AMOUNT 


                RightDragVectorEvent?.Invoke(Camera.main.ScreenToWorldPoint(DragRightStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>()));
                Debug.Log("Drag");



            };

        inputAsset.Player.DragRight.canceled +=
            ctx =>
            {

                //if below drag threshold 
                //THIS IS A SAFE HOLD SO QUICK DRAG IS NOT COUNTED AS A TAP
                if ((Camera.main.ScreenToWorldPoint(DragRightStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>())).magnitude < minDragAmount)
                {

                    //how long since held down? is it a tap
                    if ((Time.time - RightStartTime) <= TapTime)
                    {
                        Debug.Log((Time.time - RightStartTime) + " : tap");
                        TapRightEvent?.Invoke(null, EventArgs.Empty);
                    }

                    //LeftDragEvent.Invoke(Camera.main.ScreenToWorldPoint(DragLeftStartScreenPos) - Camera.main.ScreenToWorldPoint(ctx.ReadValue<Vector2>()));   
                }


                Debug.Log("Drag End");
                RightDragEvent?.Invoke(false);
                RightStartTime = 0;
                DragRightStartScreenPos = Vector2.zero;



            };

    }



    private void MultiTapActivated()
    {
        //Debug.Log("MultiTap");
    }

    private void Tap()
    {


    }

    private void SlowTapRelease()
    {
        //Debug.Log("SlowTapRelease");

    }

    private void SlowTapStarted()
    {
        //Debug.Log("SlowTapStarted");

    }

    public void OnDisable()
    {

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    public bool IsTouchRight(Vector2 ScreenPos)
    {

        if (ScreenPos.x < (screenWidth / 2))
        {
            return true;

            //The User has touched on the left side of the screen
        }
        else
        {
            return false;
            //The user hase touched the right side of the screen 
        }

    }


    

}
