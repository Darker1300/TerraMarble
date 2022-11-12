using UnityEngine;

public class FollowBehavior : MonoBehaviour
{
    private bool CenterOfTwo;
    [SerializeField]
    float defaultCameraSize = 20f;

    private float cameraScaleVelocity;
    [SerializeField]
    private float cameraScaleSpeed;
    [SerializeField] protected float followSpeed;

    [SerializeField] protected bool isXlocked = false;

    [SerializeField] protected bool isYlocked = false;

    [SerializeField] protected bool trackTarget1_ZRotation = false;
    [SerializeField] protected bool trackTarget2_ZRotation = false;

    [Header("CameraFollowTwo")]
    [SerializeField]
    [Range(0.0f, 1)]
    private float TargetTwoInfuence;

    [SerializeField] protected Transform trackingTarget;

    [SerializeField] protected Transform trackingTarget2;

    public bool trackObject;

    [SerializeField] private float xOffset;

    [SerializeField] private float yOffset;

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
        if (trackObject)
        {
            //is it two objects
            var target = CenterOfTwo ? ConvertMiddlePoint() : trackingTarget.transform.position;
            var xTarget = target.x;
            var yTarget = target.y;

            var xNew = transform.position.x;

            if (!isXlocked) xNew = Mathf.Lerp(transform.position.x, xTarget, followSpeed * Time.deltaTime);


            var yNew = transform.position.y;
            if (!isYlocked) yNew = Mathf.Lerp(transform.position.y, yTarget, followSpeed * Time.deltaTime);

            transform.position = new Vector3(xNew, yNew, transform.position.z);

            if (trackTarget1_ZRotation)
                transform.eulerAngles = new Vector3
                    (transform.eulerAngles.x, transform.eulerAngles.y, trackingTarget.eulerAngles.z);
            else if (trackTarget2_ZRotation)
                transform.eulerAngles = new Vector3
                    (transform.eulerAngles.x, transform.eulerAngles.y, trackingTarget2.eulerAngles.z);
        }


        CameraZoom();
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
}