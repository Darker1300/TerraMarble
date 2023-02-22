using MathUtility;
using Shapes;
using UnityEngine;

public class AimTreeLockUI : MonoBehaviour
{
    private DragTreePosition treeActive;

    [Header("Screen UI")] [SerializeField] private Transform screenAimTransform;
    [SerializeField] private Line screenAimLine;
    [SerializeField] private Disc screenAimDot;
    [SerializeField] private Rectangle screenEdgeRect;
    [SerializeField] private BallWindJump ballWindJumpScript;

    [Header("Wheel UI")] [SerializeField] private Transform wheelAimTransform;

    [SerializeField] private Disc powerFillDisc;
    [SerializeField] private Disc powerBackDisc;

    [SerializeField] private Disc rangeBackDisc;
    [Header("Flying How much the ball UI shrinks ")]

    [SerializeField]
    [Range(0, 1)]
    private float ShrinkSize = 0.5f;
    private float startDiscSize;
    public AnimationCurve DotUiScaleInfluence;


    [Header("Data")] [SerializeField] private bool isDragging = false;

    private void Start()
    {

        treeActive = FindObjectOfType<DragTreePosition>();
        ballWindJumpScript = FindObjectOfType<BallWindJump>();
        startDiscSize = screenAimDot.Radius;
        InputManager.LeftDragEvent += OnDragToggle;
        InputManager.RightDragEvent += OnDragToggle;
    }

    private void FixedUpdate()
    {
        if (isDragging)
        {
            UpdateWheelAim();
            UpdateScreenAim();
        }
    }

    public void OnDragToggle(bool inputDown)
    {
        isDragging = inputDown;
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

        //screenAimDot.transform.localPosition = Vector3.zero;
    }

    private void UpdateScreenAim()
    {
        if (!treeActive.enabled) return;

        //Vector3 center = Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos);
        Vector3 center = Camera.main.ScreenToWorldPoint(InputManager.ScreenSize * new Vector2(.5f, 1f - .1f));
        Vector2 worldSize = InputManager.ScreenWorldSize * treeActive.treeBender.dragSize * 2f;

        //
        screenAimTransform.rotation = Camera.main.transform.rotation;
        screenAimTransform.localScale = Vector3.forward + (Vector3) worldSize;

        screenAimTransform.position = new Vector3(center.x, center.y, screenAimTransform.position.z);

        // Aim Line
        SetScreenLine(treeActive.treeBender.dragOffsetDir);

        // Aim
        screenAimDot.transform.localPosition = new Vector3(0.5f, -0.5f, 0f)
                                               * treeActive.treeBender.dragInput;

        screenAimDot.Radius = Mathf.Lerp(startDiscSize, ShrinkSize * startDiscSize, DotUiScaleInfluence.Evaluate( ballWindJumpScript.upDragInput));
       

    }

    private void SetScreenLine(float dir)
    {
        screenAimLine.transform.localPosition = new Vector3(-0.5f -
                                                            (screenEdgeRect.Thickness * 0.5f -
                                                             screenAimLine.Thickness * 0.5f),
                                                    0f, 0f)
                                                * dir;
    }

    private void UpdateWheelAim()
    {
        Vector2 dir = ((Vector2) wheelAimTransform.Towards(treeActive.transform)).normalized;
        float ang = MathU.Vector2ToDegree(dir);
        wheelAimTransform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

        UpdatePowerBar();
    }

    public void UpdatePowerBar()
    {
        float rangeExtent = treeActive.treeBender.dragRange;
        float powerPercent = treeActive.treeBender.dragInput.y;

        float powerFull = rangeExtent * 2f * Mathf.Deg2Rad;
        powerBackDisc.AngRadiansStart = powerFull;
        powerBackDisc.AngRadiansEnd = -powerFull;

        float powerCurrent = powerFull * powerPercent;
        powerFillDisc.AngRadiansStart = powerCurrent;
        powerFillDisc.AngRadiansEnd = -powerCurrent;

        float shift = (treeActive.treeBender.dragInput.x + 1f) * 0.5f * (rangeExtent * 2f);
        float rangeSize = rangeExtent * 2f;
        rangeBackDisc.AngRadiansStart = (rangeSize * 2f - shift) * Mathf.Deg2Rad;
        rangeBackDisc.AngRadiansEnd = (-rangeSize - shift) * Mathf.Deg2Rad;
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