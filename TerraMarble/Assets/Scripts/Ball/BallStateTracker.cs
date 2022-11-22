using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallStateTracker : MonoBehaviour
{
 
    public enum BallState {Fire,Goomba,NoEffector};

    public BallState thisBallState;


    public bool Goomba = false;
    private void Start()
    {
        GetComponent<UpdateGravityDirection>().HitSurface += GoombaStompDisabled;
    }
    public void StateChange(BallState state)
    {

        thisBallState = state;
    }

    public void GoombaStompDisabled(object sender,EventArgs e)
    {
        Debug.Log("Gooomba");
        Goomba = false;
    }
}
