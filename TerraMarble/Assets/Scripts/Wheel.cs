using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using NaughtyAttributes;


public class Wheel : MonoBehaviour
{
    [Header("Config")]
    public float frictionFactor = 0.99f;
    public float idleVelocity = 3f;
    public float dragFactor = 0.1f;
    public float maxVelocity = 100f;

    [Header("Debug")]
    public Transform regionsParent;
    public Region[] regions;

    public new Rigidbody2D rigidbody2D;

    public float velocity = 1f;

    public bool dragging = false;
    public Vector2 dragTarget = Vector2.zero;

    private void Start()
    {
        rigidbody2D ??= GetComponent<Rigidbody2D>();

        InputManager.LeftDragEvent += OnLeftDrag;
        InputManager.LeftDragVectorEvent += OnLeftDragUpdate;

        velocity = idleVelocity;
    }

    private void FixedUpdate()
    {
        // Direction
        float angle = 0f;

        if (dragging)
            angle = Mathf.Atan2(
                        dragTarget.y - transform.position.y,
                        dragTarget.x - transform.position.x)
                    * Mathf.Rad2Deg;
        else angle = transform.eulerAngles.z - 10f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        var newRot = Quaternion.RotateTowards(
            transform.rotation, 
            targetRotation, 
            velocity * Time.fixedDeltaTime);

        // Apply direction
        rigidbody2D.MoveRotation(newRot);

        // Friction
        if (velocity > idleVelocity)
            velocity = Mathf.MoveTowards(
                velocity,
                idleVelocity,
                frictionFactor * Time.fixedDeltaTime);
    }

    private void OnLeftDrag(bool state)
    {
        dragging = state;

    }

    private void OnLeftDragUpdate(Vector2 currentWorldPosition, Vector2 delta)
    {
        // Debug.Log(delta);
        Camera gameCamera = Camera.main;
        dragTarget = currentWorldPosition;
        velocity = Mathf.Clamp(delta.magnitude * dragFactor, idleVelocity, maxVelocity);
    }

    private void UpdateVelocity()
    {

    }
}
