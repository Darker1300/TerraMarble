using MathUtility;
using UnityEngine;

public class EntitySeek : MonoBehaviour
{
    public bool doSeek = true;
    public Transform targetTransform = null;
    public Vector2 targetPosition = Vector2.zero;

    public float speed = 10f;
    public float maxSpeed = 40f;

    private Rigidbody2D rb;
    private Vector2 debugForceVector = Vector2.right;
    [SerializeField] private Color debugLineColor = Color.red;
    [SerializeField] private float debugLineSize = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (!doSeek) return;

        Vector2 towardsVector;
        if (targetTransform == null)
            towardsVector = transform.position.Towards(targetPosition);
        else
            towardsVector = transform.Towards(targetTransform);
        // apply speed factor
        towardsVector = towardsVector * speed;
        // clamp to max speed
        towardsVector = Vector2.ClampMagnitude(towardsVector, maxSpeed) * Time.fixedDeltaTime;

        debugForceVector = towardsVector;
        // apply force
        rb.AddForce(towardsVector);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 pos = transform.position;
        Vector2 forceV = (debugForceVector / maxSpeed) * debugLineSize;
        Gizmos.color = debugLineColor;
        Gizmos.DrawLine(pos, pos + forceV);
    }
}
