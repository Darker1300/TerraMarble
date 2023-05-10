using MathUtility;
using System.Collections.Generic;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    private float aimDistance;
    [SerializeField] private string TargetTag;

    private RaycastHit2D hit;
    [SerializeField] private LayerMask hitlayer;
    [SerializeField] private float aimRadius = 20f;

    [SerializeField] private NearbySensor NearbySensor;
    [SerializeField] private List<string> SensorBufferNames = new();


    [SerializeField] private NearbySensor.ColliderSet NearbyTargetSet = new();


    void Awake()
    {
        NearbySensor = NearbySensor == null
            ? GetComponentInChildren<NearbySensor>()
            : NearbySensor;

        NearbySensor.Updated += OnSensorUpdate;
    }

    private void OnSensorUpdate()
    {
        // Update Set
        if (NearbySensor == null) return;

        foreach (var nearby in NearbyTargetSet)
            nearby.Clear();

        foreach (var bufferName in SensorBufferNames)
        {
            NearbySensor.ColliderBuffer colliderBuffer = NearbySensor.FindBuffer(bufferName);
            if (colliderBuffer == null)
                continue; // buffer not found;
            NearbyTargetSet.AddWhere(colliderBuffer.ColliderSet, targetCollider => targetCollider);
        }
    }


    public GameObject FindClosestTarget()
    {
        float minDst = float.MaxValue;
        GameObject minDstObj = null;
        foreach (var gObjCollider in NearbyTargetSet.Stay)
        {
            GameObject gObj = gObjCollider.gameObject;
            if (!CheckLineOfSight(gObj.gameObject)) continue;
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

        hit = Physics2D.Raycast(transform.position, transform.Towards(gObj.transform).normalized, aimRadius,
            hitlayer.value);
        
        if (hit.collider && hit.collider.gameObject == gObj) return true;
        return false;
    }
}