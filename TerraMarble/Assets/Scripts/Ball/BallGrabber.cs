using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MathUtility;
using Unity.VisualScripting;
using UnityEngine;
using static NearbySensor;

public class BallGrabber : MonoBehaviour
{
    [Serializable] public class GrabbableSet : CollisionSet<HashSet<BallGrabbable>, BallGrabbable> { }

    public NearbySensor nearbySensor;
    public List<string> bufferNames = new();

    public GrabbableSet NearbyGrabSet = new();

    [SerializeField] private TetherComponent tether;
    [SerializeField] private BallGrabbable grabbed = null;


    void Awake()
    {
        nearbySensor = nearbySensor == null
            ? GetComponentInParent<NearbySensor>()
            : nearbySensor;
        tether = tether == null
            ? GetComponentInChildren<TetherComponent>()
            : tether;

        nearbySensor.Updated += OnSensorUpdate;
    }

    private void OnSensorUpdate(object sender, System.EventArgs e)
    {
        if (nearbySensor == null) return;

        foreach (var nearby in NearbyGrabSet)
            nearby.Clear();

        for (int i = 0; i < bufferNames.Count; i++)
        {
            string bufferName = bufferNames[i];

            ColliderBuffer colliderBuffer = nearbySensor.buffers.Find(b => b.Name == bufferName);
            if (colliderBuffer == null)
                continue; // buffer not found;

            NearbyGrabSet.AddWhere(colliderBuffer.ColliderSet,
                targetCollider => targetCollider.GetComponent<BallGrabbable>());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (grabbed == null)
            {
                // Attach
                grabbed = FindClosest();
                if (grabbed != null)
                {
                    tether.AttachObjectToTether(grabbed.gameObject);
                    grabbed.GrabStart.Invoke();
                }
            }
            else
            {
                // Release
                tether.DetachObjectToTether();
                grabbed.GrabEnd.Invoke();
                grabbed = null;
            }
        }
    }

    private BallGrabbable FindClosest()
    {
        float closestDst = float.MaxValue;
        BallGrabbable closestGrabbable = null;

        foreach (BallGrabbable ballGrabbable in NearbyGrabSet.Stay)
        {
            float sqrDist = ((Vector2)ballGrabbable.transform.position).Towards(transform.position).sqrMagnitude;
            if (sqrDist < closestDst)
            {
                closestDst = sqrDist;
                closestGrabbable = ballGrabbable;
            }
        }

        return closestGrabbable;
    }
}
