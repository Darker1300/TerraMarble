using MathUtility;
using UnityEngine;

public class BallStateTracker : MonoBehaviour
{
    private WheelRegionsManager wheelRegions;
    private TreeBend treeBend;
    private Rigidbody2D ballRb;

    public enum BallState
    {
        NoEffector,
        Stomp,
        Fire
    }

    public bool Stomp;

    public BallState ballState;

    private void Awake()
    {
        wheelRegions = FindObjectOfType<WheelRegionsManager>();
        treeBend = FindObjectOfType<TreeBend>(true);
        ballRb = GetComponent<Rigidbody2D>();
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
        Region.RegionHitInfo info = wheelRegions.ProcessWheelHit(collision, this);
        info?.region.BallHitEnter.Invoke(info);

        StopStomp();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Region.RegionHitInfo info = wheelRegions.ProcessWheelHit(collision, this);
        info?.region.BallHitExit.Invoke(info);
    }

}