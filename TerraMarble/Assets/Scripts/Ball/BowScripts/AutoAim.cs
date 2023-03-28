using MathUtility;
using System.Collections.Generic;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    private float aimDistance;
    [SerializeField] private string TargetTag;
    [SerializeField] private List<GameObject> nearbyTargets = new();

    private RaycastHit2D hit;
    [SerializeField] private LayerMask hitlayer;

    private CircleCollider2D col;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();
    }

    public GameObject FindClosestTarget()
    {
        float minDst = float.MaxValue;
        GameObject minDstObj = null;
        foreach (var gObj in nearbyTargets)
        {
            if (!CheckLineOfSight(gObj)) continue;
            float dst = (gObj.transform.position - transform.position).sqrMagnitude;
            if (dst < minDst)
            {
                minDst = dst;
                minDstObj = gObj;
            }
        }

        return minDstObj;
    }

    private bool CheckLineOfSight(GameObject gObj)
    {
        if (gObj.activeInHierarchy == false) return false;

        hit = Physics2D.Raycast(transform.position, transform.Towards(gObj.transform).normalized, col.radius,
            hitlayer.value);
        
        if (hit.collider && hit.collider.gameObject == gObj) return true;
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TargetTag)) nearbyTargets.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TargetTag)) nearbyTargets.Remove(collision.gameObject);
    }
}