/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using System.Collections;
using UnityEngine;


/// <summary>
/// Component that contains the Leg setting variables and have the methods to resolves its animation
/// </summary>
public class PL2D_Leg : MonoBehaviour
{
    public int legIndex;

    public bool isMoving;

    // Reference to the parent Axial Bone
    public PL2D_AxialBone pl2d_AxialBone;

    public Transform limbCenter;
    public Transform limbTarget;
    public UnityEngine.U2D.IK.Solver2D solver2D;
    public Transform restPosition;

    Vector3 footStartAngle;
    Vector3 stepEndPoint;

    Vector3 startL;
    RaycastHit2D endHit;
    RaycastHit2D startHit;
    public RaycastHit2D footHit;
    // v1.2 - added variable foot hit raycast distance 
    public float rayFootHitDistance = 0.5f;

    void Start()
    {
        isMoving = false;

        legIndex = PL2D_Animator.GetIndexFromName(name);

        footStartAngle = limbTarget.eulerAngles;

    }

    void Update()
    {
        // v1.2 - detection range of the ground adjusted;
        rayDistance = pl2d_AxialBone.mainBoneController.rayToGroundLength * 3f;

        footHit = Physics2D.Raycast(transform.position, -pl2d_AxialBone.transform.up, rayFootHitDistance, PL2D_Animator.layerMask);
        Debug.DrawRay(transform.position, -pl2d_AxialBone.transform.up * rayFootHitDistance, Color.red);

        if (!pl2d_AxialBone.isGrounded)
        {
            if (footHit.transform)
            {
                // v1.1 - Legs were lerping too fast, Changed speed to pl2d_PlayerController.bodyAcceleration
                limbTarget.position = Vector3.Lerp(limbTarget.position, footHit.point, Time.deltaTime * pl2d_AxialBone.pl2d_PlayerController.bodyAcceleration * 1);
            }
            else
            {
                // v1.1 - Legs were lerping too fast, Changed speed to pl2d_PlayerController.bodyAcceleration
                limbTarget.position = Vector3.Lerp(limbTarget.position, restPosition.position, Time.deltaTime * pl2d_AxialBone.pl2d_PlayerController.bodyAcceleration * 1);
            }

            Vector3 angle = limbTarget.eulerAngles;

            float footEndAngle = pl2d_AxialBone.transform.eulerAngles.z;
            // v1.1 - Legs were lerping too fast, Changed speed to pl2d_PlayerController.bodyAcceleration
            angle.z = Mathf.LerpAngle(angle.z, footEndAngle, Time.deltaTime * pl2d_AxialBone.pl2d_PlayerController.bodyAcceleration * 1);
            limbTarget.eulerAngles = angle;
        }
    }

    /// <summary>
    /// Adds keyframes to the Translation Curve based on the Height Curve
    /// </summary>
    /// <param name="heightCurve"></param>
    /// <returns></returns>
    public AnimationCurve CreateTranslationFromHeightCurve(AnimationCurve heightCurve)
    {
        AnimationCurve newTranslationCurve = new AnimationCurve();
        Keyframe[] hcKeys = heightCurve.keys;

        newTranslationCurve.AddKey(hcKeys[0].time, 0);
        newTranslationCurve.AddKey(hcKeys[hcKeys.Length - 1].time, 1);

        return newTranslationCurve;
    }

    public Vector2 footEndings;
    public float footAngleOffset;
    public float stepSpeed;
    public float additionalStepLength;

    // variables that are influenced by the step progression (step start to end, if sequencing equals after start, first step start to last step end)
    public AnimationCurve stepHeightCurveSI;

    /// <summary>
    /// To also apply changes use the property AutoSetTranslationCurve
    /// </summary>
    public bool autoSetTranslationCurve;
    public bool AutoSetTranslationCurve
    {
        get => autoSetTranslationCurve;
        set
        {
            autoSetTranslationCurve = value;
            if (stepHeightCurveSI.length > 0)
                stepTranslationCurveSI = CreateTranslationFromHeightCurve(stepHeightCurveSI);
        }
    }

    public AnimationCurve stepTranslationCurveSI;
    public AnimationCurve footAngleCurveSI;

    // next leg starts when previous step reaches phaseShiftFromPreviousStep
    // ex.: for starting the next step on the middle of the previous one, phaseShiftFromPreviousStep = 0.5 
    public float phaseShiftFromPreviousStep;

    // variables that are influenced by the body velocity progression (from velocity zero to up, 1 being the maximum velocity)
    public AnimationCurve multiplierSpeedCurveVI;
    public AnimationCurve additionalStepLengthCurveVI;
    public AnimationCurve multiplierHeightCurveVI;
    public AnimationCurve multiplierAngleCurveVI;

    float rayDistance = 200;

    /// <summary>
    /// Coroutine to lerps the leg to passed position 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public IEnumerator LerpLegToPosition(Vector3 pos)
    {
        float count = 0;
        while (count < 1)
        {
            // v1.1 - Legs were lerping too fast, Changed speed to pl2d_PlayerController.bodyAcceleration
            count += Time.deltaTime * pl2d_AxialBone.pl2d_PlayerController.bodyAcceleration * 2;

            limbTarget.position = Vector3.Lerp(limbTarget.position, pos, count);
            yield return null;
        }

        yield return null;
    }

    /// <summary>
    /// Resolves the leg step movement
    /// </summary>
    /// <returns></returns>
    public IEnumerator MoveLeg()
    {
        // Start of the leg step
        isMoving = true;

        Vector2 dir = -pl2d_AxialBone.transform.up;
        Vector3 moveDirection = pl2d_AxialBone.MoveDirection;

        startL = limbTarget.position;
        startHit = Physics2D.Raycast(startL, dir, rayDistance, PL2D_Animator.layerMask);

        stepEndPoint = limbCenter.position + (moveDirection * additionalStepLength) + (pl2d_AxialBone.transform.up * 1)
                        + (moveDirection * additionalStepLengthCurveVI.Evaluate(Mathf.Abs(pl2d_AxialBone.bodyVelocity) / pl2d_AxialBone.pl2d_PlayerController.bodySpeed));

        endHit = Physics2D.Raycast(stepEndPoint, dir, rayDistance, PL2D_Animator.layerMask);
        Debug.DrawRay(stepEndPoint, dir * rayDistance, Color.yellow, 1);

        Vector2 endHitPoint = limbTarget.position;

        if (endHit.transform)
            endHitPoint = endHit.point;

        float count = 0;

        if (endHit.transform != null)
        {
            bool canStartNextLeg = true;

            while (count < 1)
            {
                //--------------
                moveDirection = pl2d_AxialBone.MoveDirection;

                dir = -pl2d_AxialBone.transform.up;

                startL = limbTarget.position;

                stepEndPoint = limbCenter.position + (moveDirection * additionalStepLength) + (pl2d_AxialBone.transform.up * 1)
                                + (moveDirection * additionalStepLengthCurveVI.Evaluate(Mathf.Abs(pl2d_AxialBone.mainBoneController.bodyVelocity) / pl2d_AxialBone.pl2d_PlayerController.bodySpeed));
                endHit = Physics2D.Raycast(stepEndPoint, dir, rayDistance, PL2D_Animator.layerMask);

                if (endHit.transform)
                    endHitPoint = endHit.point;

                count += (Time.deltaTime * (stepSpeed * (multiplierSpeedCurveVI.Evaluate((Mathf.Abs(pl2d_AxialBone.mainBoneController.bodyVelocity) / pl2d_AxialBone.pl2d_PlayerController.bodySpeed)))));

                if (count > 1)
                    count = 1;

                if (count >= phaseShiftFromPreviousStep && canStartNextLeg)
                {
                    pl2d_AxialBone.actualLeg++;
                    if (pl2d_AxialBone.actualLeg >= pl2d_AxialBone.legs.Count)
                        pl2d_AxialBone.actualLeg = 0;

                    canStartNextLeg = false;
                }

                Vector3 limbPos = limbTarget.position;
                limbPos = Vector3.Lerp(startL, endHitPoint, stepTranslationCurveSI.Evaluate(count));
                limbPos += (Vector3)(-dir * ((stepHeightCurveSI.Evaluate(count) * (multiplierHeightCurveVI.Evaluate(Mathf.Abs(pl2d_AxialBone.bodyVelocity / pl2d_AxialBone.pl2d_PlayerController.bodySpeed))))));
                limbTarget.position = limbPos;

                // calculates the foot angle
                if (!float.IsNaN(CalculateFootAngle(stepEndPoint, dir, count).z))
                    limbTarget.eulerAngles = CalculateFootAngle(stepEndPoint, dir, count);

                yield return null;
            }
        }

        isMoving = false;
        // End of the leg step
    }

    // v1.2 - fix: avoids full rotation of the foot 
    public float footAngleRange = 360;

    /// <summary>
    /// Resolves the foot angling during the step
    /// </summary>
    /// <param name="endPos"></param>
    /// <param name="dir"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 CalculateFootAngle(Vector3 endPos, Vector3 dir, float t)
    {
        Vector3 angle = limbTarget.eulerAngles;

        RaycastHit2D hitFootLeftPart = Physics2D.Raycast(endPos + (pl2d_AxialBone.transform.right * -footEndings.x), dir, rayDistance, PL2D_Animator.layerMask);
        Debug.DrawRay(endPos + (pl2d_AxialBone.transform.right * -footEndings.x), dir * 1f, Color.magenta);
        RaycastHit2D hitFootRightPart = Physics2D.Raycast(endPos + (pl2d_AxialBone.transform.right * footEndings.y), dir, rayDistance, PL2D_Animator.layerMask);
        Debug.DrawRay(endPos + (pl2d_AxialBone.transform.right * footEndings.y), dir * 1f, Color.green);
        float footEndAngle = (Mathf.Atan2(hitFootRightPart.point.y - hitFootLeftPart.point.y, hitFootRightPart.point.x - hitFootLeftPart.point.x) * Mathf.Rad2Deg) + footAngleOffset;

        footEndAngle += footAngleCurveSI.Evaluate(t) * multiplierAngleCurveVI.Evaluate(Mathf.Abs(pl2d_AxialBone.bodyVelocity / pl2d_AxialBone.pl2d_PlayerController.bodySpeed))
                        * (pl2d_AxialBone.bodyVelocity / Mathf.Abs(pl2d_AxialBone.bodyVelocity));

        if (footEndAngle >= footAngleRange)
            return limbTarget.eulerAngles;

        angle.z = Mathf.LerpAngle(angle.z, footEndAngle, t);
        return angle;
    }
}
