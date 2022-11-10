using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBehavior : MonoBehaviour
{
    [SerializeField]
    protected float followSpeed;

    [SerializeField]
    protected Transform trackingTarget;

    [SerializeField]
    protected Transform trackingTarget2;

    [SerializeField]
    float xOffset;
    [SerializeField]
    float yOffset;

    [SerializeField]
    protected bool isYlocked = false;
    [SerializeField]
    protected bool isXlocked = false;
    public bool trackObject;
    private bool CenterOfTwo;
    [Header("CameraFollowTwo")]
    [SerializeField]
    [Range(0.0f, 1)]
    private float TargetTwoInfuence;

    // Start is called before the first frame update
    void Start()
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
    void Update()
    {
        if (trackObject)
        {
            //is it two objects
            Vector3 target = CenterOfTwo? ConvertMiddlePoint(): trackingTarget.transform.position;
            float xTarget = target.x;
            float yTarget = target.y; 

            float xNew = transform.position.x;

            if (!isXlocked)
            {
                xNew = Mathf.Lerp(transform.position.x, xTarget, followSpeed * Time.deltaTime);

            }



            float yNew = transform.position.y;
            if (!isYlocked)
            {
                yNew = Mathf.Lerp(transform.position.y, yTarget, followSpeed * Time.deltaTime);


            }

            transform.position = new Vector3(xNew, yNew, transform.position.z);



        }


        //transform.position = new Vector3(trackingTarget.position.x +xOffset, trackingTarget.position.y + yOffset, transform.position.z);

    }
    public Vector3 ConvertMiddlePoint()
    {
        Vector3 target = (  trackingTarget2.transform.position - trackingTarget.transform.position);
        target = trackingTarget.transform.position + (target * TargetTwoInfuence);
        return target;

    }
}
