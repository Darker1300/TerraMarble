using Shapes;
using UnityEngine;

public class FlyUI : MonoBehaviour
{
    private Rigidbody2D rb;

    [Header("Flying How much the ball UI shrinks ")]

    [SerializeField]
    [Range(0, 5)]
    private float IncreaseSize = 0.5f;
    private float startDiscSize;
    public AnimationCurve UiDistanceAniCurve;
    public AnimationCurve UiWidthAniCurve;

    [SerializeField]
    private float maxVelocity = 8;

    private Disc uiShape;

    private Vector2 uiVelocity;
    [SerializeField]
    private float smoothAimTime = 0.2f;

    public Line left;
    public Line right;
    [SerializeField]
    private float uiMin = 10;
    [SerializeField]
    private float uiMax = 25;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInParent<Rigidbody2D>();

        uiShape = GetComponent<Disc>();
        startDiscSize = uiShape.Radius;
    }

    public void SetUI(bool isActive)
    {
        uiShape?.gameObject.SetActive(isActive);
    }

    public void UpdateUI(float percBallWind)
    {
        //if (ballWindJump.upDragInput !=0)
        //{
        //0-1 factor

        uiShape.Radius = Mathf.LerpUnclamped(startDiscSize, startDiscSize + (startDiscSize * IncreaseSize), UiDistanceAniCurve.Evaluate(percBallWind));
        float angle = Mathf.LerpUnclamped(uiMin, uiMax, UiWidthAniCurve.Evaluate(percBallWind));
        uiShape.AngRadiansStart = -angle * Mathf.Deg2Rad;
        uiShape.AngRadiansEnd = angle * Mathf.Deg2Rad;
        //}
        transform.right = Vector2.SmoothDamp(transform.right, rb.velocity.normalized, ref uiVelocity, smoothAimTime);

    }
}
