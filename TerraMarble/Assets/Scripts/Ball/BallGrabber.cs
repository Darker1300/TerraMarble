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

    public NearbySensor NearbySensor;
    public List<string> BufferNames = new();

    public GrabbableSet NearbyGrabSet = new();

    [SerializeField] private TetherComponent tether;
    [SerializeField] private BallGrabbable grabbed = null;


    void Start()
    {
        InputManager.TapLeft += BombPickUp;
        InputManager.TapRight += BombPickUp;
    }


    void Awake()
    {
        NearbySensor = NearbySensor == null
            ? GetComponentInChildren<NearbySensor>()
            : NearbySensor;
        tether = tether == null
            ? GetComponentInChildren<TetherComponent>()
            : tether;

        NearbySensor.Updated += OnSensorUpdate;
    }
    

    private void OnSensorUpdate()
    {
        UpdateNearbyGrabSet();

        foreach (BallGrabbable grabbable in NearbyGrabSet.Enter)
            grabbable.NearbyEnterInvoke(this);

        foreach (BallGrabbable grabbable in NearbyGrabSet.Exit)
            grabbable.NearbyExitInvoke(this);
    }

    private void UpdateNearbyGrabSet()
    {
        if (NearbySensor == null) return;

        foreach (var nearby in NearbyGrabSet)
            nearby.Clear();

        foreach (var bufferName in BufferNames)
        {
            ColliderBuffer colliderBuffer = NearbySensor.Buffers.Find(b => b.Name == bufferName);
            if (colliderBuffer == null)
                continue; // buffer not found;

            NearbyGrabSet.AddWhere(colliderBuffer.ColliderSet,
                targetCollider => targetCollider.GetComponent<BallGrabbable>());
        }
    }

    public void BombPickUp(bool pickUp)
    {
        if (pickUp)
        {
            if (grabbed == null)
            {
                
                // Attach
                grabbed = FindClosest();
                if (grabbed != null)
                {
                    tether.AttachObjectToTether(grabbed.gameObject);
                    grabbed.GrabStartInvoke(this);
                }
            }
            else
            {
                // Release
                tether.DetachObjectToTether();
                grabbed.GrabEndInvoke(this);
                
                grabbed = null;
            }
        }
       
    }

    private void Update()
    {
        //  if (Input.GetKeyDown(KeyCode.Space))
        //  {
        //      if (grabbed == null)
        //      {
        //          // Attach
        //          grabbed = FindClosest();
        //          if (grabbed != null)
        //          {
        //              tether.AttachObjectToTether(grabbed.gameObject);
        //              grabbed.GrabStartInvoke(this);
        //          }
        //      }
        //      else
        //      {
        //          // Release
        //          tether.DetachObjectToTether();
        //          grabbed.GrabEndInvoke(this);
        //          grabbed = null;
        //      }
        //  }
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