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
    public string HavenBufferName = "Shelter";

    [SerializeField] private NearbySensor.ColliderBuffer havenColliders;
    public  int havenLaunchSpeed = 40;

    void Start()
    {
        InputManager.TapLeft += TapPickUp;
        InputManager.TapRight += TapPickUp;
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
        havenColliders = NearbySensor.FindBuffer(HavenBufferName);

    }



    private void OnSensorUpdate()
    {
        UpdateNearbyGrabSet();

        foreach (BallGrabbable grabbable in NearbyGrabSet.Enter)
            grabbable.NearbyEnterInvoke(this);

        foreach (BallGrabbable grabbable in NearbyGrabSet.Exit)
            grabbable.NearbyExitInvoke(this);
        if (grabbed = null)
            return;

        foreach (Collider2D Haven in havenColliders.ColliderSet.Stay)
        {
            // Calculate the dot product
            float dotProduct = Vector2.Dot(transform.up.normalized, (Haven.transform.position - transform.position).normalized);
            Debug.Log("DotProd : " + dotProduct);
            // Check if the dot product is close to 1, indicating ObjectB is directly above ObjectA
            if (dotProduct < -0.9f)
            {
               
                Rigidbody2D targetRb = grabbed.GetComponent<Rigidbody2D>();
                if (targetRb != null)
                {
                    tether.DetachObjectToTether();
                    grabbed.GrabEndInvoke(this);

                    grabbed = null;
                    // Apply an upward force to ObjectB
                    targetRb.AddForce(transform.up * havenLaunchSpeed, ForceMode2D.Impulse);

                }
            }
        }
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
                targetCollider =>
                {
                    if (targetCollider != null)
                        return targetCollider.GetComponent<BallGrabbable>();
                    return null;
                });
        }
    }

    public void TapPickUp()
    {

        if (grabbed == null)
        {

            // Attach
            grabbed = FindClosest();
            if (grabbed != null)
            {

                if (!grabbed.AutoPickUp)
                {
                    tether.AttachObjectToTether(grabbed.gameObject);
                    grabbed.GrabStartInvoke(this);
                }

            }
        }
        else
        {
            if (!grabbed.AutoDropOff)
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
