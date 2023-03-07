using MathUtility;
using UnityEngine;
using MathUtility;

public class FollowBehavior : MonoBehaviour
{

    private bool CenterOfTwo;
    private float screenHeight;

    [SerializeField] protected Transform trackingTarget;
    [SerializeField] protected Transform trackingTarget2;
    private Rigidbody2D rb;

    [Header("Global Follow System")]
    [SerializeField]
    float defaultCameraSize = 20f;

    private float cameraScaleVelocity;
    [SerializeField]
    private float cameraScaleSpeed;
    [SerializeField]
    private bool CameraZoomInOut;
    [SerializeField] private float followSpeed;

    [SerializeField] private bool isXlocked = false;
    [SerializeField] private bool isYlocked = false;

    [SerializeField] private bool trackTarget1_ZRotation = false;
    [SerializeField] private bool trackTarget2_ZRotation = false;

    [SerializeField]
    [Range(0.0f, 1)]
    private float TargetTwoInfuence;

    public bool trackObject;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;


    [Header("Orbit Follow System")]
    [SerializeField] private bool useOrbitSystem = false;
    [SerializeField] private Vector3 followOffset = Vector3.back;
    [SerializeField] private float rotateTime = .1f;
    [Header("Data")]
    [SerializeField] private float rotateVelocity = 0;
    [SerializeField] private Rigidbody2D trackTarget1_RB;
    private Vector3 veloref;

    [Header("camera Velocity follow")]
    //the velocity it needs to be bellow to start adjusting Focus back to surface
    [SerializeField] private float velocityDownThreshold;
    //the velocity required to dictate ball is going off the screen
    [SerializeField] private float velocityUpMin;
    float alteredVelocity;
    [SerializeField] private float followUpSpeed = 0.3f;
    [SerializeField] private float FixedSpeed = 0.8f;

    private enum CameraState
    {
        Default, WaitForCenter, FollowUp, PrepareDown
    }
    [SerializeField]
    private CameraState cameraState;


    // Start is called before the first frame update
    private void Start()
    {
        ConfigureTargets();

        if (trackingTarget.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            rb = trackingTarget.gameObject.GetComponent<Rigidbody2D>();
        }
        else if (trackingTarget2.gameObject.GetComponent<Rigidbody2D>() != null)
        {
            rb = trackingTarget2.gameObject.GetComponent<Rigidbody2D>();
        }

        screenHeight = Screen.height;
    }

    public void ConfigureTargets()
    {
        if (trackingTarget2 != null)
            CenterOfTwo = true;
        else
            CenterOfTwo = false;
    }

    // Update is called once per frame
    private void Update()
    {
        CameraStateController();
        CameraZoom();
        //CameraZoom();
        //FixedPos();
        //BallIsAboveCenter(1f);


        //if (CameraZoomInOut)
        //{
        //    CameraZoom();
        //}


        //transform.position = new Vector3(trackingTarget.position.x +xOffset, trackingTarget.position.y + yOffset, transform.position.z);
    }
    public void TrackTarget(bool center)
    {
        //is it two objects
        var target = center ? BridgeToDefaultPos() : trackingTarget.transform.position;
        //var target = trackingTarget.transform.position;
        var xTarget = target.x;
        var yTarget = target.y;

        var xNew = transform.position.x;
        var yNew = transform.position.y;

        if (!isXlocked) xNew = Mathf.Lerp(transform.position.x, xTarget, followSpeed * Time.deltaTime);
        if (!isYlocked) yNew = Mathf.Lerp(transform.position.y, yTarget, followSpeed * Time.deltaTime);
        transform.position = new Vector3(xNew, yNew, transform.position.z);

        if (trackTarget1_ZRotation)
            transform.eulerAngles = new Vector3
                (transform.eulerAngles.x, transform.eulerAngles.y, trackingTarget.eulerAngles.z);
        else if (trackTarget2_ZRotation)
            transform.eulerAngles = new Vector3
                (transform.eulerAngles.x, transform.eulerAngles.y, trackingTarget2.eulerAngles.z);


    }
    public void TrackTargetCombine(bool center)
    {

        Quaternion tRotation = GetFollowRotation(trackingTarget, trackingTarget2);
        transform.rotation = tRotation;
        Vector3 towards = trackingTarget2.Towards(rb.transform);
        //transform.position =  new Vector3(towards.x,towards.y,transform.position.z);
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(towards.x, towards.y, transform.position.z),ref veloref,  followUpSpeed);
        //convert rb up to 

    }
    public Vector3 BridgeToDefaultPos()
    {
        Quaternion tRotation = GetFollowRotation(trackingTarget, trackingTarget2);
        transform.rotation = tRotation;

        return ((trackingTarget2.position + (tRotation * followOffset)) + trackingTarget2.Towards(rb.transform)) / 2f;
        //get direction vector from point a to point b

    }
    public Vector3 GetDefaultPos()
    {
        Quaternion tRotation = GetFollowRotation(trackingTarget, trackingTarget2);
        transform.rotation = tRotation;

        return ((trackingTarget2.position + (tRotation * followOffset)));
        //get direction vector from point a to point b

    }
    //public Vector3 ConvertMiddlePoint()
    //{
    //    var target = trackingTarget2.transform.position - trackingTarget.transform.position;
    //    target = trackingTarget.transform.position + target * TargetTwoInfuence;
    //    return target;
    //}


    public bool BallWillGoOffScreen()
    {
        alteredVelocity = 0f;
        //compare velocity with our up planet direction and scale the rb magnitude with our -1 - 1 value
        alteredVelocity = rb.velocity.magnitude * Vector2.Dot(rb.transform.position.normalized, rb.velocity.normalized);
        //Ball will travel off camera ()
        if (alteredVelocity >= velocityUpMin)
        {
            //Keep to center until the ball to planet velocity drops below min(meaning player is about to descend)
            return true;



        }
        //
        return false;

    }
    public bool BallIsAboutToReturnToSurface()
    {
        alteredVelocity = 0f;
        //compare velocity with our up planet direction and scale the rb magnitude with our -1 - 1 value
        alteredVelocity = rb.velocity.magnitude * Vector2.Dot(rb.transform.position.normalized, rb.velocity.normalized);
        //Ball will travel off camera ()
        if (alteredVelocity <= velocityDownThreshold)
        {
            //Keep to center until the ball to planet velocity drops below min(meaning player is about to descend)
            return true;



        }
        //
        return false;

    }

    public void CameraStateController()
    {
        switch (cameraState)
        {
            case CameraState.Default:
                //Checks for big Velocity force going up, is the ball going to go off screen
                
                FixedPos();
                if (BallWillGoOffScreen())
                {
                    cameraState = CameraState.WaitForCenter;
                    return;
                }
                // else default locked position



                break;
            case CameraState.WaitForCenter:
                //we know the ball is going to go off screen but must wait for it to meet center of camera
                FixedPos();
                if (BallIsAboveCenter())
                {
                    cameraState = CameraState.FollowUp;
                    return;
                }


                break;
            case CameraState.FollowUp:

                //TrackTarget(false);
                TrackTargetCombine(false);
                if (BallIsAboutToReturnToSurface())
                {
                    cameraState = CameraState.PrepareDown;
                    //cameraState = CameraState.Default;
                }
                //if not in center of screen yet wait for it boss

                //is center remain there until we are about to begin falling

                break;
            case CameraState.PrepareDown:
                //focus on bottom
                //keep ball in 7/10 of screen ball pos mostly at top until it reaches back to default zone
                
                //TrackTarget(true);
                transform.position =Vector3.Lerp(transform.position, BridgeToDefaultPos(),Time.deltaTime);
                //until the camera position is close enough to 
                if (!BallIsAboveCenter())
                {
                    cameraState = CameraState.Default;
                        return;
                }

                //transform.position 
                //all we need to do is project along that predertermined direction 


                //Vector2 ballOffset = GetDefaultPos() - transform.position;
                //if (ballOffset.sqrMagnitude < 1.1f)
                //{
                //    cameraState = CameraState.Default;
                //    return;
                //}


                break;
            default:
                break;
        }


    }
    public bool BallIsAboveCenter()
    {
        
        //converts ball to screen space and checks if is above
        if (Camera.main.WorldToScreenPoint(rb.transform.position).y < screenHeight / 2)
            return false;
            //our magnitude projection 
            //project our mag onto the up direction to find out where it is
        //    Vector2 ballOffset = transform.Towards(rb.transform);
        //Debug.Log("v2v2 " + ballOffset);
        //if (ballOffset.sqrMagnitude < min)
           
        else return true;


    }
    public void FixedPos()
    {
        Quaternion tRotation = GetFollowRotation(trackingTarget, trackingTarget2);
        transform.rotation = tRotation;

        transform.position = Vector3.SmoothDamp(transform.position, trackingTarget2.position + (tRotation * followOffset),ref veloref, FixedSpeed );


    }
    private void OnDrawGizmosSelected()
    {
        Wheel wheel = FindObjectOfType<Wheel>();
        Gizmos.color = Color.blue;
        // Gizmos.DrawLine(  rb.transform.position,transform.position + (transform.position.normalized  * alteredVelocity));
        Gizmos.DrawLine(Vector2.zero, (transform.position.normalized * alteredVelocity));



        //GizmosExtensions.DrawWireCircle(wheel.transform.position, maxRad, 36, Quaternion.LookRotation(Vector3.up, Vector3.forward));

    }

    public void CameraZoom()
    {
        Quaternion tRotation = GetFollowRotation(trackingTarget, trackingTarget2);
        //transform.rotation = tRotation;

       

        float distance = Vector3.Distance(trackingTarget2.transform.position+ (transform.rotation * followOffset), trackingTarget.position);
        float target = Mathf.Max(distance /2, defaultCameraSize);

        Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, target, ref cameraScaleVelocity, cameraScaleSpeed);


    }

    public Quaternion GetFollowRotation(Transform _top, Transform _bottom)
    {
        Vector3 vector = _bottom.Towards(_top).normalized;
        Vector2 dir = vector.normalized;
        dir = dir.RotatedByDegree(-90f);
        Quaternion desiredAngle = Quaternion.AngleAxis(MathU.Vector2ToDegree(dir), Vector3.forward);

        desiredAngle = MathU.SmoothDampRotation(transform.rotation, desiredAngle, ref rotateVelocity, rotateTime);
        return desiredAngle;
    }
}