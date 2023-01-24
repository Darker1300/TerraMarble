using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// v1.2 - tempalte class for Axial Bones saving and loading 
public class PL2D_Template_AxialBone
{
    bool autosetBodyDistanceToGround;
    float bodyDistanceToGround;
    bool autosetRayToGroundLength;
    float RayToGroundLength;
    float distanceBeforeMoveLeg;
    float angleBeforeMoveLeg;
    float bodyStabilizationTopBottomFoot;
    Keyframe[] multiplierlegsDistanceOnVPosCurveVIKeys;
    PL2D_AxialBone.AngleSolverEnum angleSolverType;
    float angleTrackSpeed;
    bool angleSolverEnabled;
    bool playerControllerEnabled;
    PL2D_AxialBone.PlayerControllerEnum playerControllerType;
}
