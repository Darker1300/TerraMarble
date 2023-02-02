using MathUtility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TreeBend : MonoBehaviour
{
    // Positioning
    [SerializeField] private float wheelDst = 10;
    [SerializeField] private float maxDragDst = 10;
    private float colliderRadius;

    public float bendTime = 0.05f;
    public float maxSpeed = 1000f;

    private float dragPercent = 0f;

    [FormerlySerializedAs("treeBend")]
    [SerializeField] private AnimationCurve treeBendCurve;
    private AimTreeLockUI aimUi;

    public List<RotateToDirectionNoRb> nearbyTrees = new();

    private void Start()
    {
        aimUi = FindObjectOfType<AimTreeLockUI>(true);
        colliderRadius = GetComponent<CircleCollider2D>().radius;

        InputManager.LeftDragVectorEvent += OnDragUpdate;
    }

    private void Update()
    {
        UpdateTrees();
    }

    private void OnDisable()
    {
        foreach (var target in nearbyTrees)
            target.RotateToThis(0f, transform.position);
    }

    private void UpdateTrees()
    {
        if (nearbyTrees.Count > 0)
            foreach (var target in nearbyTrees)
            {
                float fallOffPercent =
                   1f - treeBendCurve.Evaluate(
                        Mathf.Clamp01(
                            transform.Towards(target.transform).sqrMagnitude
                            / (colliderRadius * colliderRadius)));
                fallOffPercent = dragPercent * fallOffPercent;
                Debug.Log("Drag: " + fallOffPercent);
                target.RotateToThis(fallOffPercent, transform.position);
            }
    }

    public void OnDragUpdate(Vector2 dragVector, Vector2 dragDelta)
    {
        // Update Position
        transform.position = -((Vector3)dragVector.normalized * wheelDst);

        dragPercent = Mathf.Clamp01(dragVector.magnitude / maxDragDst);
        Debug.Log("Drag: " + dragPercent);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var rotToDir = collision.gameObject.GetComponentInParent<RotateToDirectionNoRb>();
        if (rotToDir && !nearbyTrees.Contains(rotToDir)) nearbyTrees.Add(rotToDir);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var rotToDir = collision.gameObject.GetComponentInParent<RotateToDirectionNoRb>();
        if (!rotToDir) return;

        nearbyTrees.Remove(rotToDir);
        rotToDir.RotateToThis(0f, transform.position);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            Camera.main.ScreenToWorldPoint(InputManager.DragLeftStartScreenPos),
            maxDragDst);
    }

}