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

    private void Awake()
    {
        regionsMan = FindObjectOfType<WheelRegionsManager>();
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
        Region.RegionHitInfo info = regionsMan.ProcessWheelHit(collision, this);
        info?.region.BallHitEnter.Invoke(info);

        StopStomp();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Region.RegionHitInfo info = regionsMan.ProcessWheelHit(collision, this);
        info?.region.BallHitExit.Invoke(info);
    }

}