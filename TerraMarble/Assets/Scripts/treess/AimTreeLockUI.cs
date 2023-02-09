using MathUtility;
using Shapes;
using UnityEngine;
using UnityEngine.Serialization;

public class AimTreeLockUI : MonoBehaviour
{
    private DragTreePosition treeActive;

    [Header("Screen UI")]
    [SerializeField] private Transform screenAimTransform;
    [SerializeField] private Line screenAimLine;
    [SerializeField] private Disc screenAimDot;
    [SerializeField] private Rectangle screenEdgeRect;


    [Header("Wheel UI")]
    [SerializeField] private Transform wheelAimTransform;

    [SerializeField] private Disc powerFillDisc;
    [SerializeField] private Disc powerBackDisc;

    [SerializeField] private Disc rangeBackDisc;

    [SerializeField]
    private float radius;

    private float endSize;

    void Start()
    {
        treeActive = FindObjectOfType<DragTreePosition>();

        powerFillDisc.AngRadiansStart = radius;
        powerFillDisc.AngRadiansEnd = -radius;

        //endSize = powerBackDisc.AngRadiansStart;

        InputManager.LeftDragEvent += EnableDiscs;
    }

    void FixedUpdate()
    {
        UpdateWheelAim();
        UpdateScreenAim();
    }

    public void EnableDiscs(bool inputDown)
    {
        if (inputDown)
        {
            wheelAimTransform.gameObject.SetActive(true);
            screenAimTransform.gameObject.SetActive(true);
            UpdateScreenAim();
            UpdateWheelAim();
        }
        else
        {
            wheelAimTransform.gameObject.SetActive(false);
            screenAimTransform.gameObject.SetActive(false);
        }
    }

    private void UpdateScreenAim()
    {
        if (!treeActive.enabled) return;

        Vector3 center = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
        Vector2 worldSize = InputManager.ScreenWorldSize * treeActive.treeBender.dragSize * 2f;

        //
        screenAimTransform.rotation = Camera.main.transform.rotation;
        screenAimTransform.localScale = Vector3.forward + (Vector3)worldSize;

        screenAimTransform.position = new Vector3(center.x, center.y, screenAimTransform.position.z);

        // Aim Line
        screenAimLine.transform.localPosition = new Vector3(-0.5f -
                                                            ((screenEdgeRect.Thickness * 0.5f) -
                                                             (screenAimLine.Thickness * 0.5f)),
                                                    0f, 0f)
                                                * treeActive.treeBender.dragOffsetDir;

        // Aim
        screenAimDot.transform.localPosition = new Vector3(0.5f, -0.5f, 0f)
                                               * treeActive.treeBender.dragInput;
    }

    private void UpdateWheelAim()
    {
        Vector2 dir = ((Vector2)wheelAimTransform.Towards(treeActive.transform)).normalized;
        float ang = MathU.Vector2ToDegree(dir);
        wheelAimTransform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

        UpdatePowerBar();

    }

    public void UpdatePowerBar()
    {
        //float size = treeActive.treeBender.dragRange
        //             + treeActive.treeBender.dragInitalOffset * treeActive.treeBender.dragOffsetDir;

        float rangeExtent = treeActive.treeBender.dragRange;

        float powerPercent = treeActive.treeBender.dragInput.y;

        //float rangeOffset = treeActive.treeBender.dragInitalOffset * treeActive.treeBender.dragOffsetDir;

        float powerFull = rangeExtent * 2f * Mathf.Deg2Rad;
        powerFillDisc.AngRadiansStart = Mathf.Lerp(0, powerFull, powerPercent);
        powerFillDisc.AngRadiansEnd = -Mathf.Lerp(0, powerFull, powerPercent);

        powerBackDisc.AngRadiansStart = powerFull;
        powerBackDisc.AngRadiansEnd = -powerFull;

        //float dragDir = treeActive.treeBender.dragOffsetDir;
        //float rangeEdgeOffset = treeActive.treeBender.dragInput.x * rangeExtent;
        //float dirMin = Mathf.Min(0f, dragDir);
        //float dirMax = Mathf.Max(0f, dragDir);

        //float rangeOffsetMin = (rangeExtent + treeActive.treeBender.dragInitalOffset) * dirMin - rangeEdgeOffset * dirMin;
        //float rangeOffsetMax = (rangeExtent + treeActive.treeBender.dragInitalOffset) * dirMax + rangeEdgeOffset * dirMax;

        //rangeBackDisc.AngRadiansStart = rangeOffsetMin * 2f * Mathf.Deg2Rad;
        //rangeBackDisc.AngRadiansEnd = rangeOffsetMax * 2f * Mathf.Deg2Rad;

        ////float rangeBackFull = (rangeExtent + rangeOffset + rangeOffsetMin) * Mathf.Deg2Rad;
    }

    //public void LookRotation(Vector2 direction,Vector2 delta)
    //{

    //    //transform.LookAt(target,Vector3.up);
    //    direction = -direction.normalized;

    //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //    //angle += offset;

    //    //float dot = Vector2.Dot(-transform.parent.up, Direction);

    //    //angle = Vector2.Dot(,)
    //    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    //    transform.rotation = rotation;
    //}
}
