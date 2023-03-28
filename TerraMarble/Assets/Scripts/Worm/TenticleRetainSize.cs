using UnityEngine;

public class TenticleRetainSize : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRend;
    public Transform targetDir;
    public Transform wiggleDir;

    [Header("Config")]
    public int length = 20;
    public float targetDist = 0.02f;
    [Range(0.02f, 0.18f)] public float GrowSize = 0.02f;
    public float wiggleSpeed = 350f;
    public float wiggleMagnitude = 1.62f;
    public Transform followObject = null;

    [Header("Data")]
    public Vector3[] segmentPoses;
    public Vector3[] segmentV;

    void Start()
    {
        lineRend = lineRend != null ? lineRend : GetComponentInChildren<LineRenderer>();
        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];
    }

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
        if (followObject != null)
            followObject.position = segmentPoses[segmentPoses.Length - 1];
    }
}
