using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BallStateTracker : MonoBehaviour
{
    private WheelRegionsManager regionsMan;

    public enum BallState
    {
        Fire,
        Stomp,
        NoEffector
    }

    public bool Stomp;

    public BallState ballState;

    [SerializeField] private bool DoDebug = false;

    private void Awake()
    {
        regionsMan = FindObjectOfType<WheelRegionsManager>();
    }

    public void StateChange(BallState state)
    {
        ballState = state;
    }

    public void BallStompDisabled(Collision2D _collision2D)
    {
        if (DoDebug) Debug.Log("Gooomba");
        Stomp = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Region.RegionHitInfo info = ProcessHit(collision);
        info?.region.BallHitEnter.Invoke(info);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Region.RegionHitInfo info = ProcessHit(collision);
        info?.region.BallHitExit.Invoke(info);
    }

    private Region.RegionHitInfo ProcessHit(Collision2D collision)
    {
        bool hitSurfaceObj = IsHitSurfaceObj(collision);
        bool hitRegion = IsHitRegion(collision);

        if (hitSurfaceObj || hitRegion)
        {
            Region.RegionHitInfo info = new();
            Region r = regionsMan.GetClosestRegion(collision.GetContact(0).point);

            if (hitSurfaceObj)
                info.surfaceObj = info.collision.gameObject;
            else if (hitRegion)
                info.surfaceObj = null;

            info.ballState = this;
            info.collision = collision;
            info.region = r;
            return info;
        }
        return null;
    }

    private bool IsHitSurfaceObj(Collision2D collision)
        => collision.collider.gameObject.layer == LayerMask.NameToLayer("Surface");

    private bool IsHitRegion(Collision2D collision)
        => collision.collider.gameObject.layer == LayerMask.NameToLayer("Wheel");
}