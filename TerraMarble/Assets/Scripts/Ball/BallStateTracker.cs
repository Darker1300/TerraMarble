using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityUtility;

public class BallStateTracker : MonoBehaviour
{
    private WheelRegionsManager regionsMan;

    public enum BallState
    {
        NoEffector,
        Stomp,
        Fire
    }

    public bool Stomp;

    public BallState ballState;

    private LayerMask surfaceLayer;
    private LayerMask wheelLayer;

    private void Awake()
    {
        regionsMan = FindObjectOfType<WheelRegionsManager>();

        surfaceLayer = LayerMask.NameToLayer("Surface");
        wheelLayer = LayerMask.NameToLayer("Wheel");
    }

    public void StateChange(BallState state)
    {
        ballState = state;
    }

    public void StartStomp()
    {
        Stomp = true;
    }

    public void StopStomp()
    {
        Stomp = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Region.RegionHitInfo info = ProcessHit(collision);
        info?.region.BallHitEnter.Invoke(info);
        StopStomp();
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

            // Find Region from Surface Object
            Region regionSearch = collision.collider.GetComponentInParent<Region>();

            // Alternative Find region
            if (regionSearch == null) // if failed, find Region from contact point
            {
                Vector2 p = collision.contactCount > 0  // use Contact.point if possible
                    ? collision.GetContact(0).point // contact position
                    : transform.position; // Ball position
                regionSearch = regionsMan.GetClosestRegion(p);
            }

            if (hitSurfaceObj)
                info.surfaceObj = collision.collider.GetComponentInParent<SurfaceObject>();
            else info.surfaceObj = null;

            info.ballState = this;
            info.collision = collision;
            info.region = regionSearch;
            return info;
        }

        return null;
    }

    private bool IsHitSurfaceObj(Collision2D collision)
    {
        return collision.collider.gameObject.layer == surfaceLayer;
    }

    private bool IsHitRegion(Collision2D collision)
    {
        return collision.collider.gameObject.layer == wheelLayer;
    }
}