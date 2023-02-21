using MathUtility;
using UnityEngine;

public class EntitySeek : MonoBehaviour
{
    public bool doSeek = true;
    public Transform targetTransform = null;
    public Vector2 targetPosition = Vector2.zero;

    public float speed = 10f;
    //public float maxSpeed = 40f;

    private Rigidbody2D rb;
    private Wheel wheel;
    private Vector2 debugForceVector = Vector2.right;
    [SerializeField] private Color debugLineColor = Color.white;
    [SerializeField] private float debugLineSize = 0.01f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        wheel = FindObjectOfType<Wheel>();
    }

    void FixedUpdate()
    {
        if (!doSeek) return;

        Vector2 transformPos;
        if (targetTransform == null)
            transformPos = targetPosition;
        else
            transformPos = targetTransform.position;

        Vector2 forceVector = MoveTowardsAroundCircle(transform, transformPos,
            wheel.transform.position, wheel.regions.WheelRadius,
            speed * Time.fixedDeltaTime);

        debugForceVector = forceVector;
        // apply force
        rb.AddForce(forceVector);
    }

    private static Vector2 MoveTowardsAroundCircle(
        Transform _transform, Vector2 _target,
        Vector2 _circlePos, float _circleRadius, float _maxDelta)
    {

        return Vector2.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 pos = transform.position;
        Vector2 forceV = debugForceVector * debugLineSize;
        Gizmos.color = debugLineColor;
        Gizmos.DrawLine(pos, pos + forceV);
    }
}
