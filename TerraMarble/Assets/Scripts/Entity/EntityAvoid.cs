using System.Collections;
using System.Collections.Generic;
using MathUtility;
using UnityEngine;
using UnityUtility;

public class EntityAvoid : MonoBehaviour
{
    public bool doCollisionAvoid = true;
    [SerializeField] private float pushForce = 400f;
    [SerializeField] private float distanceCheck = 2f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Color debugAvoidColor = Color.cyan;
    private Collider2D collider2d;
    private Rigidbody2D rb;
    private Wheel wheel;
    private Vector2 castDir = Vector2.right;

    //[SerializeField] private bool doHeightAvoid = false;
    //[SerializeField] private Vector2 minMaxHeight = new(16f, 25.5f);
    //[SerializeField] private Color debugHeightColor = Color.white;


    void Awake()
    {
        collider2d = GetComponentInChildren<Collider2D>();
        rb = GetComponentInChildren<Rigidbody2D>();
        wheel = FindObjectOfType<Wheel>();
    }

    void Update()
    {
        if (doCollisionAvoid)
        {
            Vector2 upDir = wheel.transform.Towards(transform).normalized;
            ContactFilter2D contactFilter2D = new ContactFilter2D();
            contactFilter2D.SetLayerMask(layerMask);

            RaycastHit2D[] raycastHit2Ds = new RaycastHit2D[1];
            castDir = (rb.velocity.normalized - upDir).normalized;
            int count = collider2d.Cast(castDir,
                contactFilter2D, raycastHit2Ds, distanceCheck, true);

            if (count > 0)
            {
                float strength = (1f - raycastHit2Ds[0].fraction) * pushForce * Time.deltaTime;
                rb.AddForce(upDir * strength);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (doCollisionAvoid)
        {
            Gizmos.color = debugAvoidColor;
            GizmosExtensions.DrawWireCircle(transform.position + (Vector3)(castDir * distanceCheck),
                0.1f, 36, Quaternion.LookRotation(Vector3.up, Vector3.forward));
        }

        //if (doHeightAvoid)
        //{
        //    Gizmos.color = debugHeightColor;
        //    if (wheel == null) wheel = FindObjectOfType<Wheel>();
        //    GizmosExtensions.DrawWireCircle(wheel.transform.position, minMaxHeight.x,
        //        36, Quaternion.LookRotation(Vector3.up, Vector3.forward));
        //    GizmosExtensions.DrawWireCircle(wheel.transform.position, minMaxHeight.y,
        //        36, Quaternion.LookRotation(Vector3.up, Vector3.forward));
        //}
    }
}
