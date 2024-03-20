using Shapes;
using UnityEngine;
using UnityUtility;

public class BallGrabbable : MonoBehaviour
{
    public delegate void GrabberEvent(BallGrabber grabber);

    public float GrabRadius = 5f;
    public float NearbyRadius = 7.5f;
    public float FadeTime = 0.1f;
    public float MaxAlpha = 1f;
    public float CooldownDuration = 0.75f;
    public bool showUI = true;
    public bool showGizmos = true;

    private float fadeVelocity;
    private float cooldownTime = 0.0f;

    [Header("Data")] 
    public bool BallIsNearby = false;
    public bool IsGrabbed = false;
    public BallGrabber Grabber = null;

    public event GrabberEvent NearbyEnter;
    public event GrabberEvent NearbyExit;

    public event GrabberEvent GrabStart;
    public event GrabberEvent GrabEnd;
    public bool AutoPickUp= false;
    public bool AutoDropOff= false;
    [SerializeField] private Disc discUI = null;
    public bool IsCoolingDown => CooldownDuration > 0f;

    public float DiscAlpha
    {
        get => discUI.ColorInner.a;
        set => discUI.ColorInner = discUI.ColorInner.AsAlpha(value);
    }

    public float DiscRadius
    {
        get => discUI.Radius;
        set => discUI.Radius = value;
    }

    private void Start()
    {
        discUI = discUI != null
            ? discUI
            : transform.GetComponentInChildren<Disc>();
        DiscAlpha = 0f;
        DiscRadius = GrabRadius;
    }

    private void Update()
    {
        if (!discUI || !Grabber) return;

        if (!BallIsNearby && DiscAlpha < Mathf.Epsilon) return;

        float distance = Vector2.Distance(transform.position, Grabber.transform.position);
        float goalAlpha = 0f;

        if (showUI && distance < NearbyRadius)
            goalAlpha = MaxAlpha;

        DiscAlpha = Mathf.SmoothDamp(DiscAlpha, goalAlpha, ref fadeVelocity, FadeTime);

        if (cooldownTime > 0f)
            cooldownTime = Mathf.Max(0f, cooldownTime - Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.white;
        GizmosExtensions.DrawWireCircle(transform.position, GrabRadius,
            72, Quaternion.LookRotation(Vector3.up, Vector3.forward));

        Gizmos.color = Color.gray;
        GizmosExtensions.DrawWireCircle(transform.position, NearbyRadius,
            72, Quaternion.LookRotation(Vector3.up, Vector3.forward));
    }


    public void NearbyEnterInvoke(BallGrabber grabber)
    {
        NearbyEnter?.Invoke(grabber);
        BallIsNearby = true;
        if (Grabber == null) Grabber = grabber;

    }

    public void NearbyExitInvoke(BallGrabber grabber)
    {
       
        NearbyExit?.Invoke(grabber);
        BallIsNearby = false;
    }

    public void GrabStartInvoke(BallGrabber grabber)
    {
        GrabStart?.Invoke(grabber);
        IsGrabbed = true;
        Grabber = grabber;
    }

    public void GrabEndInvoke(BallGrabber grabber)
    {
        GrabEnd?.Invoke(grabber);
        IsGrabbed = false;
        cooldownTime = CooldownDuration;
    }
}