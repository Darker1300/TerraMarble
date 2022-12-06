using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class BallStateTracker : MonoBehaviour
{
    public enum BallState
    {
        Fire,
        Stomp,
        NoEffector
    }

    public bool Stomp;

    public BallState ballState;

    [HideInInspector]
    public UnityEvent<Collision2D, BallStateTracker> BounceHit
        = new UnityEvent<Collision2D, BallStateTracker>();
    [HideInInspector]
    public UnityEvent<Collision2D, BallStateTracker> StompHit
        = new UnityEvent<Collision2D, BallStateTracker>();

    [SerializeField] private bool DoDebug = false;

    private void Start()
    {
        GetComponent<BallBounce>().HitSurface.AddListener(BallStompDisabled);
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
        if (ballState == BallState.Stomp)
            StompHit.Invoke(collision, this);
        else
            BounceHit.Invoke(collision, this);
    }

}