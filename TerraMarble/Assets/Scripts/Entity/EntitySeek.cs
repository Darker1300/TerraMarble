using MathUtility;
using UnityEngine;
using UnityUtility;

public class EntitySeek : MonoBehaviour
{

    private EnemyHealth enemyHealth;
    public bool doSeek = true;
    public Transform targetTransform = null;
    public Vector2 targetPosition = Vector2.zero;
    public Vector2 forceMultiplier = new(2f, 2f);
    public Vector2 seekSpeed = new(5f, 5f);
    

    //public float maxSpeed = 40f;

    private Rigidbody2D rb;
    private Wheel wheel;
    private Vector2 debugForceVector = Vector2.right;
    [SerializeField] private Color debugLineColor = Color.white;
    [SerializeField] private float debugLineSize = 0.01f;
    private void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.OnProjectileHit.AddListener(OnHit);
        enemyHealth.OnStunEnd.AddListener(StartSeek);

    }
    void Awake()
    {
       // enemyhealth = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        wheel = FindObjectOfType<Wheel>();
    }
    private void OnEnable()
    {
        targetTransform = GameObject.FindGameObjectWithTag("Ball").transform;
    }

    void FixedUpdate()
    {
        if (!doSeek) return;

        Vector2 targetPos = targetTransform == null
            ? targetPosition
            : targetTransform.position;

        Vector2 newPosition = MoveTowardsAroundCircle(
            transform.position, targetPos,
            wheel.transform.position,
            seekSpeed * Time.fixedDeltaTime);

        Vector2 forceVector = newPosition - (Vector2)transform.position;
        debugForceVector = forceVector;

        forceVector *= forceMultiplier;

        // apply force
        rb.AddForce(forceVector);
    }

    private static Vector2 MoveTowardsAroundCircle(
        Vector2 _currentPos, Vector2 _targetPos,
        Vector2 _circlePos, Vector2 _maxDelta)
    {
        // Get new Radius
        float newRadius = MoveTowardsAroundRadius(_currentPos, _targetPos, _circlePos, _maxDelta.y);

        // Find new Angle, along new Radius
        float newAngle = MoveTowardsAroundAngle(_currentPos, _targetPos, _circlePos, newRadius, _maxDelta.x);

        // convert radius and angle to position
        Vector2 newPos = MathU.DegreeToVector2(newAngle) * newRadius;

        return newPos;
    }

    private static float MoveTowardsAroundRadius(
        Vector2 _currentPos, Vector2 _targetPos,
        Vector2 _circlePos, float _maxDelta)
    {
        float currentRadius = _circlePos.Towards(_currentPos).magnitude;
        float targetRadius = _circlePos.Towards(_targetPos).magnitude;
        float newRadius = Mathf.MoveTowards(currentRadius, targetRadius, _maxDelta);
        return newRadius;
    }

    private static float MoveTowardsAroundAngle(
        Vector2 _currentPos, Vector2 _targetPos,
        Vector2 _circlePos, float _circleRadius, float _maxDelta)
    {
        float currentAngle = MathU.Vector2ToDegree(
            _circlePos.Towards(_currentPos).normalized);
        float targetAngle = MathU.Vector2ToDegree(
            _circlePos.Towards(_targetPos).normalized);

        // Move along New Radius to get new angle
        float circleLength = _circleRadius * Mathf.PI * 2.0f;
        float maxAngleDelta = (360f / circleLength) * 360f * _maxDelta;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, maxAngleDelta);
        return newAngle;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 pos = transform.position;
        Vector2 forceV = debugForceVector * debugLineSize;
        Gizmos.color = debugLineColor;
        Gizmos.DrawLine(pos, pos + forceV);
    }
    public void OnHit(Collider2D col)
    {
        doSeek = false;
    }

    public void StartSeek()
    {
        doSeek = true;
    }


}
