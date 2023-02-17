using MathUtility;
using UnityEngine;
using MathUtility;

public class FollowBehavior : MonoBehaviour
{

    private bool CenterOfTwo;

    [SerializeField] protected Transform trackingTarget;
    [SerializeField] protected Transform trackingTarget2;

    [Header("Global Follow System")]
    [SerializeField]
    float defaultCameraSize = 20f;

    private float cameraScaleVelocity;
    [SerializeField]
    private float cameraScaleSpeed;
    [SerializeField]
    private bool CameraZoomInOut;
    [SerializeField] protected float followSpeed;

    [SerializeField] protected bool isXlocked = false;
    [SerializeField] protected bool isYlocked = false;

    [SerializeField] protected bool trackTarget1_ZRotation = false;
    [SerializeField] protected bool trackTarget2_ZRotation = false;

    [SerializeField] [Range(0.0f, 1)]
    private float TargetTwoInfuence;

    public bool trackObject;
    [SerializeField] private float xOffset;
    [SerializeField] private float yOffset;


    [Header("Orbit Follow System")]
    [SerializeField] protected bool useOrbitSystem = false;
    [SerializeField] private Vector3 followOffset = Vector3.back;
    [SerializeField] private float rotateTime = .1f;
    [Header("Data")]
    [SerializeField] private float rotateVelocity = 0;



    // Start is called before the first frame update
    private void Start()
    {
        ConfigureTargets();
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

        if (useOrbitSystem)
        {
            Quaternion tRotation = GetFollowRotation(trackingTarget, trackingTarget2);
            transform.rotation = tRotation;

            transform.position = trackingTarget2.position + (tRotation * followOffset);
            
            return;
        }
        
        if (trackObject)
        {
            //is it two objects
            var target = CenterOfTwo ? ConvertMiddlePoint() : trackingTarget.transform.position;
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
        if (CameraZoomInOut)
        {
            CameraZoom();
        }

        
        //transform.position = new Vector3(trackingTarget.position.x +xOffset, trackingTarget.position.y + yOffset, transform.position.z);
    }

    public Vector3 ConvertMiddlePoint()
    {
        var target = trackingTarget2.transform.position - trackingTarget.transform.position;
        target = trackingTarget.transform.position + target * TargetTwoInfuence;
        return target;
    }

    public void CameraZoom()
    {


        float distance = Vector3.Distance(trackingTarget2.transform.position, trackingTarget.position);
        float target = Mathf.Max(distance + 3.0f, defaultCameraSize);

        Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, target, ref cameraScaleVelocity, cameraScaleSpeed);


    }

    public Quaternion GetFollowRotation(Transform _top, Transform _bottom)
    {
        Vector3 vector = _bottom.Towards(_top).normalized;
        Vector2 dir = vector.normalized;
        dir = dir.RotatedByDegree(-90f);
        Quaternion desiredAngle = Quaternion.AngleAxis(MathU.Vector2ToDegree(dir), Vector3.forward);

        desiredAngle = MathU.SmoothDampRotation(transform.rotation, desiredAngle,ref rotateVelocity, rotateTime);
        return desiredAngle;
    }
}