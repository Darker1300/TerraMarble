using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using System;

public class RightAim : MonoBehaviour
{
    //this is the start drag direction that will be converted to angle
    private Vector2 AimStartingVector;
    public float minDragAmount;
    private Vector2 aimDirection;
    private bool hasMin = false;
    private float percentOfMaxDrawOne;
    public LineRenderer lineIndicator;

    private Vector3 linePosition;
    private UpdateGravity updateGravityScript;
    public Disc Ui;
    public BallStateTracker ballState;

    public SlowTime timeSlowDown;
    public int tapAmount;
    public enum tapState
    {
        DEFAULT, ONE, WAITING, TWO
    }
    public tapState STATE;

    [SerializeField] private bool DoDebug = false;
    [Header("Aim Tweaks")]
    [SerializeField]
    [Range(0.0f,1)]
    private float AimSensitivity= 0.5f;
    [SerializeField]
    private bool aimReAdjust;
    // Start is called before the first frame update
    private void Start()
    {
       
    }

    private void OnEnable()
    {
        ballState = GetComponent<BallStateTracker>();
        lineIndicator = GetComponent<LineRenderer>();
        updateGravityScript = GetComponent<UpdateGravity>();

        timeSlowDown = GetComponent<SlowTime>();

        InputManager.LeftDragEvent += Release;
        InputManager.LeftAlternateEvent += AlternateStart;
        //InputManager.RightDragEvent += TapDrag;
        InputManager.LeftAlternateDragVectorEvent += AimRestrictor;
        // Ui.enabled = true;
    }


    private void OnDisable()
    {
        InputManager.RightDragVectorEvent -= AimRestrictor;
        InputManager.RightDragEvent -= Release;
        // Ui.enabled = false;
    }

    private void FixedUpdate()
    {
    }
    private void AlternateStart(object sender, EventArgs e)
    {
        lineIndicator.enabled = true;
        timeSlowDown.startSlowMotion();
       
        
        //timeSlowDown.startSlowMotion();

    }


    // Update is called once per frame
    private void Update()
    {
        lineIndicator.SetPosition(1, linePosition);
        lineIndicator.SetPosition(0, transform.position);
    }

    public void Release(bool buttonDown)
    {
        
        if (!buttonDown)
        {
            lineIndicator.enabled = false;
            timeSlowDown.StopSlowMotion();
            //timeSlowDown.StopSlowMotion();
            hasMin = false;
            // GetComponent<Rigidbody2D>().AddRelativeForce(aimDirection.normalized * 100);
            //add and extra force value for our gravity script
            updateGravityScript.stompForceVector = aimDirection * 2;
            ballState.StartStomp();
            Ui.enabled = false;
            aimDirection = Vector2.zero;
        }
        else if (buttonDown)

        {
            timeSlowDown.startSlowMotion();
             //StartCoroutine(Countdown());
        }
    }
    private IEnumerator Countdown()
    {
        float duration = 0.08f; // 3 seconds you can change this 
        //Ui.gameObject.SetActive(true);                 //to whatever you want
        float normalizedTime = 0;
        Ui.Radius = 5;
        while (normalizedTime <= 1f)
        {
           // Ui.Radius = 10 - normalizedTime;
            //countdownImage.fillAmount = normalizedTime;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        Ui.enabled = true;
        //STATE = tapState.DEFAULT;
    }

    //public void TapDrag(bool tap)
    //{
    //    if (tap)
    //    {


    //        switch (STATE)
    //        {
    //            case tapState.DEFAULT:
    //                StartCoroutine(Countdown());
    //                STATE = tapState.ONE;
    //                break;
    //            case tapState.ONE:

    //                InputManager.RightDragVectorEvent += AimRestrictor;
    //                STATE = tapState.WAITING;
    //                break;
    //            case tapState.WAITING:
    //                break;
    //            case tapState.TWO:
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //    else if (STATE == tapState.WAITING)
    //    {
    //        InputManager.RightDragVectorEvent -= AimRestrictor;
    //    }

    //}
    //    public void TapDrag(Vector2 dragVector,Vector2 dragDelta)
    //{

    //    switch (STATE)
    //    {
    //        case tapState.DEFAULT:

    //            STATE = tapState.ONE;
    //            break;
    //        case tapState.ONE:
    //            //ui on 
    //            StartCoroutine(Countdown());
    //            STATE = tapState.WAITING;
    //            break;
    //        case tapState.WAITING:

    //            break;
    //        case tapState.TWO:
    //            break;
    //        default:
    //            break;
    //    }
    //}
    public void AimRestrictor(Vector2 dragVector, Vector2 dragDelta, Vector2 screenDragVector)
    {
        

        //Debug.DrawLine(transform.position, transform.position + (Vector3)(transform.rotation * dragVector * 30), Color.blue);
        //
        //dragVector = transform.rotation * dragVector;
        if (dragVector.magnitude > minDragAmount && !hasMin) // and if it is not locked already
        {
            //cache the vector
            //!!!!!cache the drag make sure this doesnt keep getting updated

            AimStartingVector = dragVector;
            hasMin = true;
        }
        if (!aimReAdjust)
        {
           

            //if Forward do nothing
            //if not forward
            if (Vector2.Dot((Vector2)updateGravityScript.wheelDir.normalized, dragVector) <= 0)
            {
                aimDirection = -dragVector;
                linePosition = transform.position + (Vector3)(aimDirection * 30);

            }
            else//NORMAL DRAG FORWARD
            {

                aimDirection = dragVector;
                linePosition = transform.position + (Vector3)(aimDirection * 30);
            }

        }
        else
        {
            float realAngle = 0;
            realAngle += GetRelativeAimRotationAngle(AimStartingVector, dragVector, 90.0f);

            if (realAngle != 0)
            {
                //get one vector from both drag angles
                aimDirection = RotateToAngle(aimDirection, realAngle * AimSensitivity);
                //transform.up = -updateGravityScript.direction;


                // ___________________________CALCULATE DRAWBACK VALUE________________________________
                //our Draw back  magnitude and cap our aim limiter to the this v
                //float DrawBackTrueMagOne = ProjectileAimLimiter(ref aimDirection, 45.0f, ref percentOfMaxDrawOne);

                Debug.DrawLine(transform.position, transform.position + (Vector3)(aimDirection * 30), Color.cyan);

                //lineIndicator.End = transform.InverseTransformVector( (Vector3)(aimDirection * 30));
                linePosition = transform.position + (Vector3)(aimDirection * 30);

                //leftDragStartLine.End = leftDragStartThumbRad.gameObject.transform.rotation * (aimDirection.normalized * (percentOfMaxDrawOne * 45));
                //leftDragStartThumbIndicator.gameObject.transform.up = leftDragStartThumbRad.gameObject.transform.rotation * aimDirection;

                //leftDragStartThumbIndicator.ColorOuter = new Color(colorAim.r, colorAim.g, colorAim.b, percentOfMaxDrawOne);

                //TwoFingerDrawBackEvent?.Invoke(percentOfMaxDrawOne);

                //// ___________________________APPLY EVERYTHING________________________________
                ////-WINGS-Rotation lerp
                ////Wing Holder Rotation
                //WingsHolderLocal.localRotation = Quaternion.LookRotation(Vector3.forward, (Vector3)aimDirection.normalized);
                ////Wing Left Rotation
                //WingLeftLocal.localRotation = Quaternion.LookRotation(Vector3.forward, (Vector3)Vector2.Lerp(new Vector2(-0.95f, -0.05f), new Vector2(0.5f, 0.5f), percentOfMaxDrawOne));
                ////Wing Right Rotation
                //WingRightLocal.localRotation = Quaternion.LookRotation(Vector3.forward, (Vector3)Vector2.Lerp(new Vector2(0.95f, -0.05f), new Vector2(-0.5f, 0.5f), percentOfMaxDrawOne));


                ////-WINGS-position LERP  make it more angle like reduce to the 1.5 times factor to 1.2
                ////X
                //float wingX = (float)Mathf.LerpUnclamped(WingObjectChargeStartPosX, WingObjectChargeEndPosX, percentOfMaxDrawOne * 1.5f);
                ////Y
                //float wingY = (float)Mathf.LerpUnclamped(WingObjectChargeStartPosY, WingObjectChargeEndPosY, percentOfMaxDrawOne * 1.5f);
                //WingLeftLocal.localPosition = new Vector3(-wingX, wingY, 0);
                //WingRightLocal.localPosition = new Vector3(wingX, wingY, 0);
                ////_________________
                //float ArmX = (float)Mathf.LerpUnclamped(TopObjectChargeStartPosX, TopObjectChargeEndPosX, percentOfMaxDrawOne);
                //float ArmY = (float)Mathf.LerpUnclamped(TopObjectChargeStartPosY, TopObjectChargeEndPosY, percentOfMaxDrawOne);
                //TopLeft.transform.localPosition = new Vector3(-ArmX, ArmY, 0);
                //TopRight.transform.localPosition = new Vector3(ArmX, ArmY, 0);
                ////_________________  
                //Eye.localPosition = Vector3.SlerpUnclamped(EyeChargeStartPos, EyeChargeEndPos, percentOfMaxDrawOne * 1.2f);


                ////-PROJECTILE-
                ////Projectile Charge Position
                //float ProjectileDrawBackVect = (float)Mathf.Lerp(ProjObjectChargeStartPos, ProjObjectChargeEndPos, percentOfMaxDrawOne * 1.2f);

                ////ProjectChamber.transform.localPosition = aimDirection * (DrawBackTrueMagOne);
                //ProjectChamber.transform.localPosition = aimDirection * ProjectileDrawBackVect;
                ////Projectile Rotation
                //ProjectChamber.transform.localRotation = WingsHolderLocal.localRotation;
                //BowStringUpdatePullbackPos(ProjectChamber.transform.position);
                ////Debug.Log("projState" + percentOfMaxDrawOne);
            }
        }
    }

    public float ProjectileAimLimiter(ref Vector2 RealDirection, float MaxMag, ref float zeroToOnePercent)
    {
        //CONVERT TO GET TRUE start vector DIRECTION


        var dot = Vector2.Dot(RealDirection.normalized, Vector2.up);
        //Debug.Log("upDragMag" + TouchLeftDrag.DragVector.magnitude);


        var clampedUpdatedMag = Vector2.ClampMagnitude(AimStartingVector * 0.5f, MaxMag);

        //THIS  DECREASES THE MAG THE CLOSER IT SIDES OF BOW GIVING IT THE CURVE
        RealDirection = RealDirection.normalized * dot;
        //LETS 
        var t = clampedUpdatedMag.magnitude / MaxMag;
        t = Mathf.Sin(t * Mathf.PI * 0.5f);
        zeroToOnePercent = t;
        //Debug.Log("dottooo" + t);
        return t;
    }

    public Vector2 RotateToAngle(Vector2 AimDirection, float angle)
    {
        AimDirection = (Vector2)(Quaternion.Euler(0, 0, angle) * updateGravityScript.wheelDir.normalized);
        //Vector2 dirFake = (Vector2)(Quaternion.Euler(0, 0, absFakeAngle) * Vector2.up);
        //Vector3 direction = new Vector2((float)Mathf.Cos(angle * Mathf.PI / 180), (float)Mathf.Sin(angle * Mathf.PI / 180));
        //Debug.Log("angle" + absFakeAngle);
        AimDirection = AimDirection * 20;
        //Debug.DrawLine(transform.position, transform.position + transform.rotation * (Vector3)AimDirection, Color.red);
        //Debug.DrawLine(transform.position, transform.position + transform.rotation * (Vector3)dirFake, Color.blue);
        return AimDirection;
    }

    public float GetRelativeAimRotationAngle(Vector2 FromRotateVectorDirection, Vector2 TooVectorToCompare,
        float clampRotation)
    {
        //Vector2 startDragVectorDirection = (Vector2)Camera.main.ScreenToWorldPoint(startProjectileDragEndScreen) - (Vector2)Camera.main.ScreenToWorldPoint(StartMouseScreenPos);

        // Debug.Log("startDrag: " + startDragVectorDirection.normalized + "endDrag: " + updatedDragWorldVector.normalized);
        //convert to local 
        //transform up 

        //Debug.DrawLine(transform.position, transform.position + (Vector3)updatedDragWorldVector * 20, Color.yellow);


        Debug.DrawLine(transform.position,
            transform.position + (transform.position + (Vector3)TooVectorToCompare * 10), Color.blue);
        //start Direction
        //Debug.DrawLine(test.gameObject.transform.position, test.gameObject.transform.position +(Vector3)startDragVectorDirection, Color.yellow);

        //current Dragvector

        // ///GET THE X VALUE DIFFERENCE 
        var x = TooVectorToCompare.x / 200.0f;
        x = 1f - Mathf.Cos(x * Mathf.PI * 0.5f);

        ///MAKE THE Y VALUE ABSOLLUTE 
        var y = Mathf.Abs(TooVectorToCompare.y);
        ///APPLY OUR MAGIC FORMULATE
        //Debug.Log("newV " + TooVectorToCompare.x);

        //
        var trueAngle = Vector2.SignedAngle(FromRotateVectorDirection.normalized, TooVectorToCompare.normalized);

        var isCorrectAngle = Mathf.Clamp(trueAngle, -clampRotation, clampRotation);
        //is valid angle solve rest
        if (isCorrectAngle != 0.0f)
        {
            //float absFakeAngle = Mathf.Abs(isCorrectAngle);

            //absFakeAngle = absFakeAngle / 45.0f;
            //absFakeAngle = 1f - Mathf.Cos(absFakeAngle * Mathf.PI * 0.5f);


            //Debug.Log("Fakeangle" + (absFakeAngle * angle));
            ////////////////////////////////////////////

            if (trueAngle < -1.0f)
                if (DoDebug) Debug.Log("angle left" + trueAngle);
            if (trueAngle > 1.0f)
                if (DoDebug) Debug.Log("angle right" + trueAngle);


            return isCorrectAngle;
        }
        ////////////////////////////////////////////

        return 0.0f;
    }
}