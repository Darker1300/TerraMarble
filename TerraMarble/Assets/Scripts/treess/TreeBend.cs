using MathUtility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TreeBend : MonoBehaviour
{
    public List<RotateToDirectionNoRb> nearbyTrees = new();

    // Positioning
    [SerializeField] private float min = 10;
    [SerializeField] private float max = 10f;
    private float colliderRadius;

    public float rotationSpeed = 60f;

    [FormerlySerializedAs("treeBend")]
    [SerializeField] private AnimationCurve treeBendCurve;
    private AimTreeLockUI aimUi;

    private void Start()
    {
        aimUi = FindObjectOfType<AimTreeLockUI>(true);
        colliderRadius = GetComponent<CircleCollider2D>().radius;

        InputManager.LeftDragVectorEvent += UpdatePosition;
    }

    private void Update()
    {
        UpdateTrees();
    }

    private void OnDisable()
    {
        foreach (var target in nearbyTrees)
            target.RotateToThis(1f, transform.position);
    }

    private void UpdateTrees()
    {
        if (nearbyTrees.Count > 0)
            foreach (var target in nearbyTrees)
            {
                float percent =
                    treeBendCurve.Evaluate(
                        Mathf.Clamp01(
                            transform.Towards(target.transform)
                                .magnitude
                            / colliderRadius));
                target.RotateToThis(percent, transform.position);
            }
    }

    public void UpdatePosition(Vector2 dir, Vector2 delta)
    {
        //get the difference and reverse it
        float diference = max - Mathf.Clamp(dir.magnitude, min, max);
        diference = min + diference;

        //invert
        transform.position = -((Vector3)dir.normalized * diference);
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
        rotToDir.RotateToThis(1f, transform.position);
    }
}