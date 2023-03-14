using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// v1.2 - tempalte class for Legs saving and loading 
public class PL2D_Template_Leg : MonoBehaviour
{
    float stepSpeed;
    float additionalStepLength;
    Keyframe[] stepHeightCurveSIKeys;
    bool autoSetTranslationCurve;
    Keyframe[] stepTranslationCurveSIKeys;
    float phaseShiftFromPreviousStep;
    Keyframe[] multiplierSpeedCurveVIKeys;
    Keyframe[] additionalStepLengthCurveVIKeys;
    Keyframe[] multiplierHeightCurveVIKeys;
    Vector2 footEndings;
    float footAngleOffset;
    Keyframe[] footAngleCurveSIKeys;
    Keyframe[] multiplierAngleCurveVIKeys;
}
