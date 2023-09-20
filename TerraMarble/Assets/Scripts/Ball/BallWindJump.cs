using MathUtility;
using System.Linq;
using UnityEngine;
using UnityUtility;

public class BallWindJump : MonoBehaviour
{
    [Header("Config Jump")]
    [SerializeField] private float glideSpeed = 40f;
    [SerializeField] private float minGlideSpeed = 1f;
    [SerializeField] private float upwardsSpeed = 70f;
    [SerializeField] private float slowDescentSpeed = 400f;

    [SerializeField] private float jumpDuration = 1f;
    [SerializeField] private float minUpDragInput = 0.1f;
    [SerializeField] private float upDragUISize = 0.15f;

    [SerializeField] private AnimationCurve forceCurve
        = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private bool doDebug = true;

    [Header("Config Particles")]
    [SerializeField] private float partDistance = 2.5f;
    [SerializeField] private AnimationCurve particleRateCurve
        = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [SerializeField] private int partInitialBurst = 10;
    [SerializeField] private float partMultiplier = 10f;

    [Header("Data")]
    public float upDragInput;
    public float horDragInput = 1f;

    public bool IsJumping = false;
    [SerializeField] private Rigidbody2D ballRb = null;
    [SerializeField] private Wheel wheel = null;
    [SerializeField] private ParticleSystem partSystem = null;
    [SerializeField] private float jumpTimer = 0f;
    [SerializeField] private float particleTimer = 0f;
    [SerializeField] private RaycastHit2D[] hits = new RaycastHit2D[5];
    private Vector3 downDir = Vector3.down;

    private FlyUI flyUI;
    void Start()
    {
        ballRb = gameObject.GetComponentInParent<Rigidbody2D>();
        partSystem = partSystem != null ? partSystem
            : GetComponentInChildren<ParticleSystem>();

        wheel = FindObjectOfType<Wheel>();
        flyUI = GetComponentInChildren<FlyUI>(true);
        InputManager.LeftDragEvent += ToggleDrag;
        InputManager.RightDragEvent += ToggleDrag;

        InputManager.LeftDragVectorEvent += (vector, delta, screenDragVector) => UpdateDragInput(screenDragVector);
        InputManager.RightDragVectorEvent += (vector, delta, screenDragVector) => UpdateDragInput(screenDragVector);

        flyUI.SetUI(false);
    }

    private void ToggleDrag(bool state)
    {
        upDragInput = 0f;
    }

    private void UpdateDragInput(Vector2 screenDragVector)
    {
        upDragInput = Mathf.Clamp(screenDragVector.y / upDragUISize, 0f, 1f);
        horDragInput = Mathf.Clamp(screenDragVector.x / upDragUISize, -1f, 1f);
    }

    void Update()
    {
        downDir = ballRb.transform.Towards(wheel.transform).normalized;
        UpdateHits();
        UpdateParticles();

        if (upDragInput > minUpDragInput)
        {
            // if (!IsJumping)
            {
                IsJumping = true;
                DoWindJump();


                flyUI.UpdateUI(upDragInput);
            }
        }
        else if (IsJumping)
        {
            IsJumping = false;
            OnWindJumpEnd();
            Camera.main.GetComponent<FollowBehavior>().cameraState = FollowBehavior.CameraState.FollowUp;
        }
    }

    void FixedUpdate()
    {
        OnWindJumpUpdate();
    }

    public void DoWindJump()
    {
        //IsJumping = true;
        UpdateParticles();
        partSystem.Emit(partInitialBurst);

        flyUI.SetUI(true);
        Camera.main.GetComponent<FollowBehavior>().cameraState = FollowBehavior.CameraState.FollowUp;
    }

    void OnWindJumpUpdate()
    {
        if (!IsJumping) return;

        Vector2 rbVLocal = transform
            .InverseTransformDirection(ballRb.velocity).To2DXY();

        float forwardDir = horDragInput;
        // if not dragging yet, keep moving forward
        if (Mathf.Approximately(0f, forwardDir))
            forwardDir = Mathf.Sign(rbVLocal.x);

        float upForce = upwardsSpeed * forceCurve.Evaluate(upDragInput) * Time.fixedDeltaTime;
        float forwardForce = Mathf.Max(glideSpeed - upForce, minGlideSpeed) * Time.fixedDeltaTime;

        // Up
        rbVLocal.y += upForce;
        if (rbVLocal.y < 0f)
            rbVLocal.y = Mathf.MoveTowards(
                rbVLocal.y, 0f, slowDescentSpeed * Time.fixedDeltaTime);
        // Forward
        rbVLocal.x += forwardForce * forwardDir;

        Vector2 rbVWorld = transform
            .TransformDirection(rbVLocal).To2DXY();
        ballRb.velocity = rbVWorld;

        // Timer
        jumpTimer += Time.fixedDeltaTime;
    }

    void UpdateParticles()
    {
        if (!partSystem) return;

        float hitDst = partDistance;
        if (hits.Length > 0)
        {
            RaycastHit2D hit = hits.First();
            hitDst = hit.distance;
        }

        partSystem.transform.position = ballRb.transform.position + (downDir * hitDst);
        partSystem.transform.rotation = Quaternion.AngleAxis((-(Vector2)downDir).ToDegrees(), Vector3.forward);

        if (!IsJumping) return;

        float t = jumpTimer / jumpDuration;
        float curveT = particleRateCurve.Evaluate(t);
        float deltaCount = curveT * partMultiplier * Time.deltaTime;
        particleTimer += deltaCount;

        int emitCount = Mathf.FloorToInt(particleTimer);
        particleTimer -= emitCount;

        partSystem.Emit(emitCount);
    }

    void OnWindJumpEnd()
    {
        jumpTimer = 0f;
        particleTimer = 0f;

        flyUI.SetUI(false);
    }

    private void UpdateHits()
    {
        ballRb.Cast(downDir, hits, partDistance);
    }

    private void OnDrawGizmosSelected()
    {
        if (!doDebug || hits.Length < 1) return;
        if (wheel == null) wheel = FindObjectOfType<Wheel>();
        CircleCollider2D collider = GetComponentInChildren<CircleCollider2D>();

        Gizmos.color = Color.gray;
        GizmosExtensions.DrawWireCircle(hits.First().point, collider.radius,
            72, Quaternion.LookRotation(Vector3.up, Vector3.forward));
    }
}
