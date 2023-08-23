using MathUtility;
using Shapes;
using Unity.Mathematics;
using UnityEngine;

public class AimTreeLockUI : MonoBehaviour
{
    private DragTreePosition treeActive;

    [Header("Screen UI")] [SerializeField] private Transform screenAimTransform;
    [SerializeField] private Line screenAimLine;
    [SerializeField] private Disc screenAimDot;
    [SerializeField] private Rectangle screenEdgeRect;
    [SerializeField] private BallWindJump ballWindJumpScript;
    [SerializeField] private GameObject ball;
    [SerializeField] private PlayerInput playerInput;


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

    [Header("Telegraph")]
    public Disc telegraphDisc;

    [Header("Data")] [SerializeField] private bool isDragging = false;

    private void Start()
    {
        treeActive = FindObjectOfType<DragTreePosition>();
        ballWindJumpScript = FindObjectOfType<BallWindJump>();
        playerInput = FindObjectOfType<PlayerInput>();
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
        if (Vector3.Distance(Camera.main.GetComponent<FollowBehavior>().trackingTarget.position, Camera.main.GetComponent<FollowBehavior>().trackingTarget2.position) < 25f)
        {
            telegraphDisc.enabled = true;
           
            telegraphDisc.transform.rotation = Camera.main.transform.rotation;
        }
        else
            telegraphDisc.enabled = false;

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
        Vector2 worldSize = InputManager.ScreenWorldSize * playerInput.DragScreenSize * 2f;

        //
        screenAimTransform.rotation = Camera.main.transform.rotation;
        screenAimTransform.localScale = Vector3.forward + (Vector3)worldSize;

        screenAimTransform.position = new Vector3(center.x, center.y, screenAimTransform.position.z);

        // Aim Line
        SetScreenLine(playerInput.Side);

        Vector2 dragInput = playerInput.RawDrag;
        dragInput.y = Mathf.Clamp01(-dragInput.y);

        // Aim
        screenAimDot.transform.localPosition = new Vector3(0.5f, -0.5f, 0f)
                                               * dragInput; // todo fix


        UpdateScreenAimDotSize();
    }

    public void UpdateScreenAimDotSize()
    {
        screenAimDot.Radius = Mathf.Lerp(startDiscSize, ShrinkSize * startDiscSize,
            DotUiScaleInfluence.Evaluate(ballWindJumpScript.upDragInput));
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
        Vector2 dir = ((Vector2)wheelAimTransform.Towards(treeActive.transform)).normalized;
        float ang = MathU.Vector2ToDegree(dir);
        wheelAimTransform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

        UpdatePowerBar();
    }

    public void UpdatePowerBar()
    {
        Vector2 dragInput = playerInput.RawDrag;
        dragInput.y = Mathf.Clamp01(-dragInput.y);

        float rangeExtent = treeActive.treeBender.dragMoveRange;
        float powerPercent = dragInput.y;

        float powerFull = rangeExtent * 2f * Mathf.Deg2Rad;
        powerBackDisc.AngRadiansStart = powerFull;
        powerBackDisc.AngRadiansEnd = -powerFull;

        float powerCurrent = powerFull * powerPercent;
        powerFillDisc.AngRadiansStart = powerCurrent;
        powerFillDisc.AngRadiansEnd = -powerCurrent;

        float rangeSize = rangeExtent * 2f;
        float shift = math.remap(-1, 1, 0, 1, dragInput.x) * rangeSize;
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