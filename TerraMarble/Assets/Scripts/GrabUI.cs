using System;
using Shapes;
using UnityEngine;
using UnityUtility;

public class GrabUI : MonoBehaviour
{
    [SerializeField] private Disc discUI = null;
    [SerializeField] private BallGrabbable grabbable = null;
    public float GrabRadius = 5f;
    public float NearbyRadius = 7.5f;
    public float FadeoutSpeed = .1f;
    public float FlashSpeed = 2f;
    public float GrabDuration = 1f;
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private float FlashTimer = 1f;
    [SerializeField] private float GrabTimer = -1f;
    [SerializeField] private float grabVelocity;

    public float DiscAlpha
    { get => discUI.ColorInner.a; set => discUI.ColorInner = discUI.ColorInner.AsAlpha(value); }
    public float DiscRadius
    { get => discUI.Radius; set => discUI.Radius = value; }

    public event Action<BallGrabbable> GrabReady;

    void Start()
    {
        grabbable = grabbable != null ? grabbable
            : transform.parent.GetComponentInChildren<BallGrabbable>();

        DiscRadius = GrabRadius;
    }

    void Update()
    {
        if (grabbable == null ||
            grabbable.Grabber == null ||
            !grabbable.BallIsNearby && Mathf.Approximately(DiscAlpha, 0f)) // Ball is far away and we've finished animating to zero alpha
            return;

        float distance = Vector2.Distance(grabbable.transform.position, grabbable.Grabber.transform.position);
        float deltaSpeed = Time.deltaTime * FlashSpeed;

        if (distance < GrabRadius)
        {
            // animate grabbing
            FlashTimer = Mathf.Repeat(FlashTimer + deltaSpeed, 1f);
            DiscAlpha = Mathf.PingPong(FlashTimer * 2f, 1f);

            DiscRadius = Mathf.SmoothDamp(DiscRadius, 0f, ref grabVelocity, GrabDuration);
        }
        else if (distance < NearbyRadius)
        {
            DiscAlpha = Mathf.Clamp((distance - DiscRadius) / (NearbyRadius - DiscRadius), 0f, 1f);
        }
        else
        {
            DiscAlpha = Mathf.MoveTowards(DiscAlpha, 0f, deltaSpeed);

            if (Mathf.Approximately(DiscAlpha, 0f))
            {
                GrabReady.Invoke(grabbable);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.gray;
        GizmosExtensions.DrawWireCircle(transform.position, NearbyRadius,
            72, Quaternion.LookRotation(Vector3.up, Vector3.forward));
    }
}