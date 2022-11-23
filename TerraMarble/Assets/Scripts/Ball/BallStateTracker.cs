using UnityEngine;

public class BallStateTracker : MonoBehaviour
{
    public enum BallState
    {
        Fire,
        Goomba,
        NoEffector
    }

    public bool Goomba;

    public BallState thisBallState;

    private void Start()
    {
        GetComponent<UpdateGravityDirection>().HitSurface.AddListener(GoombaStompDisabled);
    }

    public void StateChange(BallState state)
    {
        thisBallState = state;
    }

    public void GoombaStompDisabled(Collision2D _collision2D)
    {
        Debug.Log("Gooomba");
        Goomba = false;
    }
}