using UnityEngine;
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

    public BallState thisBallState;

    private void Start()
    {
        GetComponent<BallBounce>().HitSurface.AddListener(BallStompDisabled);
    }

    public void StateChange(BallState state)
    {
        thisBallState = state;
    }

    public void BallStompDisabled(Collision2D _collision2D)
    {
        Debug.Log("Gooomba");
        Stomp = false;
    }
}