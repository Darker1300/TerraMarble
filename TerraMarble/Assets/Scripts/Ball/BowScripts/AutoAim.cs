using MathUtility;
using System.Collections.Generic;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    private float aimDistance;
    [SerializeField]
    private string TargetTag;
    [SerializeField]
    private List<GameObject> nearbyTargets = new List<GameObject>();
    RaycastHit2D hit;
    [SerializeField]
    private LayerMask hitlayer;
    CircleCollider2D col;

    void Start()
    {
        //collider = GetComponent<CircleCollider2D>();
        col = GetComponent<CircleCollider2D>();


    }

    // Update is called once per frame
    void Update()
    {

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

        hit = Physics2D.Raycast(transform.position, transform.Towards(gObj.transform).normalized, col.radius, hitlayer.value);

        // Debug.DrawLine(transform.position,transform.position + transform.Towards(gObj.transform));
        if (hit != null && hit.collider.gameObject == gObj)
        {
            return true;
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TargetTag))
        {
            nearbyTargets.Add(collision.gameObject);
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TargetTag))
        {
            nearbyTargets.Remove(collision.gameObject);
        }
    }

}
