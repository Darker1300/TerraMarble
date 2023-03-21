using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenticleRetainSize : MonoBehaviour
{
    public int length;

    public LineRenderer lineRend;
    public Vector3[] segmentPoses;
    public Vector3[] segmentV;
    public Transform targetDir;
    public float targetDist;
   
    [Range(0.02f,0.18f)]
    public float GrowSize = 0.1f;
    [SerializeField]
   

    public float wiggleSpeed;
    public float wiggleMagnitude;
    public Transform wiggleDir;
    public Vector3 endPos;

    public Transform followObject;

    // Start is called before the first frame update
    void Start()
    {
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
    }

    // Update is called once per frame
    void Update()
    {
        wiggleDir.localRotation = Quaternion.Euler(0, 0, Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude);

        segmentPoses[0] = targetDir.position;
        for (int i = 1; i < segmentPoses.Length; i++)
        {
            Vector3 targetPos = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i - 1]).normalized * targetDist;
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentV[i], GrowSize);
        }
        lineRend.SetPositions(segmentPoses);
        followObject.position = segmentPoses[segmentPoses.Length - 1];
        //endPos = segmentPoses[segmentPoses.Length - 1];

    }
}
