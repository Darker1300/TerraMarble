using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// v1.2 - tempalte class for Player Controller saving and loading
public class PL2D_Template_PlayerController
{
    float bodySpeed;
    float bodyAcceleration;
    bool enableWallWalk;
    bool enableJump;
    float jumpForce;
    float horizontalVelocityMultiplierForce;
    float preLegBendTime;
    bool enableFlipToMouse;
    bool invertFlipToMouse;
    List<Vector3> pathPoints;
}
