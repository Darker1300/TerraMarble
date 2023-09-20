using MathUtility;
using UnityEngine;
using MathUtility;

public class FollowBehavior : MonoBehaviour
{

    private bool CenterOfTwo;
    private float screenHeight;
    [SerializeField]
    private Renderer playerMeshRenderer;
    public Transform trackingTarget;
     public Transform trackingTarget2;
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
    [SerializeField] private Vector3 ZoomOffset = Vector3.back;
    [Header("PrepareDown Follow System")]
    [SerializeField] private Vector3 followOffsetPrepareDown = Vector3.back;
    public float smoothSpeed = 0.125f; // Smoothing factor
    public float yOffsetPrepareDown = 2.0f; // Vertical offset to keep the object in the top half

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

    public enum CameraState
    {
        Default, WaitForCenter, FollowUp, PrepareDown
    }
    [SerializeField]
    public CameraState cameraState;


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


        playerMeshRenderer = trackingTarget.GetComponent<BallAnimController>().BodyBase.GetComponentInChildren<SkinnedMeshRenderer>();
    }
    private void LateUpdate()
    {
        if (cameraState == CameraState.PrepareDown)
        {
            Vector3 desiredPosition = trackingTarget.position + new Vector3(0, yOffset, 0);
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
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
        transform.position = Vector3.SmoothDamp(transform.position, new Vector3(towards.x, towards.y, transform.position.z), ref veloref, followUpSpeed);
       

    }
    public Vector3 BridgeToDefaultPos()
    {
        Quaternion tRotation = GetFollowRotation(trackingTarget, trackingTarget2);
        transform.rotation = tRotation;
        Vector3 bridge = ((trackingTarget2.position + (tRotation * followOffsetPrepareDown)) + trackingTarget2.Towards(rb.transform)) / 2f;
        return bridge;

        ////if the bridge point to player projected up is not lower than camupdist else add the diference to bridge point
        ////project bridge to camera up compare that with view port height(/ 2),get the diference then project it back onto

        //// bridge point to player vector 
        //Vector3 toPlayerFromBridge = trackingTarget.position- bridge;
        //Debug.DrawLine(bridge,bridge + toPlayerFromBridge, Color.blue);


        //Vector3 cameraTopViewportPoint = new Vector3(0.5f, 1.0f, 0.0f); // Center of the top edge of the viewport
        //Vector3 worldPointAtTopViewport = Camera.main.ViewportToWorldPoint(cameraTopViewportPoint);
        //Vector3 BridgeTocamera = Vector3.Project(worldPointAtTopViewport,bridge.normalized) - bridge;
        //Debug.DrawLine(bridge, bridge + BridgeTocamera, Color.red);


        //    Debug.Log("B to Player  " + toPlayerFromBridge.sqrMagnitude + "  |  B to Cam  " + BridgeTocamera.sqrMagnitude);
        //if (BridgeTocamera.sqrMagnitude < toPlayerFromBridge.sqrMagnitude)
        //{
        //   float difference = toPlayerFromBridge.sqrMagnitude - BridgeTocamera.sqrMagnitude;
        //   bridge =   bridge.normalized * (difference + bridge.sqrMagnitude);
        //    //bridge = bridge.normalized * (difference + (bridge.sqrMagnitude - (worldPointAtTopViewport - transform.position).sqrMagnitude));
        //}

        ////(pb) (project toPlayerFromBridge onto camera transform up) 

        ////float dot = Vector2.Dot( Vector3.up, toPlayerFromBridge) ;
        ////Debug.Log("dott" + dot);
        ////Vector3 projectPlayerTo =  (toPlayerFromBridge.magnitude * dot) * transform.up ;

        ////(top camera) get distance to top of camera (viewport)the 

        ////make sure to player(pb) is not higher than (top camera) 
        ////if difference get the diference then project it back onto the world to player vector
        ////
        ////get its magnitude and the bridge magnitude add them together then times that by planet to world normalized direction

        //return bridge;


        //project 



        //if player is at the top of the screen 

        //Vector2 playerOffScreenV2 = IsPlayerInView();
        //if (playerOffScreenV2 == Vector2.zero)
        //{
        //    //(wheel + playerRot) * offset) + 
        //    //direction 
        //    return ((trackingTarget2.position + (tRotation * followOffsetPrepareDown)) + trackingTarget2.Towards(rb.transform)) / 2f;
        //}
        //else
        //{
        //    return ((((trackingTarget2.position + (tRotation * followOffsetPrepareDown)) + trackingTarget2.Towards(rb.transform)) / 2f) + transform.up * Mathf.Abs(playerOffScreenV2.y));
        //}



        //get direction vector from point a to point b

    }
    public Vector2 IsPlayerInView()
    {

        if (!playerMeshRenderer.isVisible)

        {
            Vector3 objectPosition =trackingTarget. transform.position;

            float cameraHeight = 2f * Camera.main.orthographicSize;
            float cameraWidth = cameraHeight * Camera.main.aspect;

            float distanceOffScreenX = 0;
            float distanceOffScreenY = 0;

            if (objectPosition.x < Camera.main.transform.position.x - cameraWidth / 2)
                distanceOffScreenX = Mathf.Abs(Camera.main.transform.position.x - cameraWidth / 2 - objectPosition.x);
            else if (objectPosition.x > Camera.main.transform.position.x + cameraWidth / 2)
                distanceOffScreenX = Mathf.Abs(objectPosition.x - Camera.main.transform.position.x - cameraWidth / 2);

            if (objectPosition.y < Camera.main.transform.position.y - cameraHeight / 2)
                distanceOffScreenY = Mathf.Abs(Camera.main.transform.position.y - cameraHeight / 2 - objectPosition.y);
            else if (objectPosition.y > Camera.main.transform.position.y + cameraHeight / 2)
                distanceOffScreenY = Mathf.Abs(objectPosition.y - Camera.main.transform.position.y - cameraHeight / 2);

            Debug.Log($"Object is off the screen by {distanceOffScreenX} units horizontally and {distanceOffScreenY} units vertically.");
            return new Vector2(distanceOffScreenX,distanceOffScreenY);
        }
        else return Vector2.zero;


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
                transform.position = Vector3.Lerp(transform.position, BridgeToDefaultPos().WithZ(-10) , Time.deltaTime);
                //until the camera position is close enough to 
               
               //project 

               
                if ( Vector3.Distance(trackingTarget.position, trackingTarget2.position) < 30f)
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

        transform.position = Vector3.SmoothDamp(transform.position, trackingTarget2.position + (tRotation * followOffset), ref veloref, FixedSpeed).WithZ(-10);


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



        float distance = Vector3.Distance(trackingTarget2.transform.position + (transform.rotation * ZoomOffset), trackingTarget.position);
        float target = Mathf.Max(distance / 2, defaultCameraSize);

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