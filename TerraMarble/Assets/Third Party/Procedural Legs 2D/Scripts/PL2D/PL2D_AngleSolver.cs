/// Procedural Legs 2D Tool - Version 1.1
/// 
/// Daniel C Menezes
/// http://danielcmcg.github.io
/// 

using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component contains the methods that calculate and reangle the Axial Bone accordingly
/// </summary>
public class PL2D_AngleSolver : MonoBehaviour
{
    public List<PL2D_AxialBone> controllers;

    Vector3 boneAngle;
    float legsAngle;
    RaycastHit2D raycastNormal;

    /// <summary>
    /// Adds and initializes a PL2D_AngleSolver component in the Axial Bone
    /// </summary>
    /// <param name="pl2dAxialBone"></param>
    public static void Initialize(PL2D_AxialBone pl2dAxialBone)
    {
        pl2dAxialBone.pl2d_AngleSolver = pl2dAxialBone.GetComponent<PL2D_AngleSolver>() ? pl2dAxialBone.GetComponent<PL2D_AngleSolver>() : pl2dAxialBone.gameObject.AddComponent<PL2D_AngleSolver>();

        if (pl2dAxialBone.angleSolverEnabled)
        {
            // v1.1 - Fix: Angle solver type "Keep Vertical" not initializing correctly
            if (!pl2dAxialBone.pl2d_AngleSolver.isActiveAndEnabled)
                pl2dAxialBone.pl2d_AngleSolver.enabled = true;

            pl2dAxialBone.pl2d_AngleSolver.controllers = new List<PL2D_AxialBone>();
            pl2dAxialBone.pl2d_AngleSolver.controllers.Add(pl2dAxialBone);

            if (pl2dAxialBone.angleSolverType == PL2D_AxialBone.AngleSolverEnum.RelativeToLegs)
            {
                if (pl2dAxialBone.pL2dAnimator.axialBones.Count > pl2dAxialBone.axialBoneIndex + 1)
                {
                    pl2dAxialBone.pl2d_AngleSolver.controllers.Add(pl2dAxialBone.pL2dAnimator.axialBones[pl2dAxialBone.axialBoneIndex + 1]);

                    if (pl2dAxialBone.BottomFoot().transform.position.x > pl2dAxialBone.pl2d_AngleSolver.controllers[1].BottomFoot().transform.position.x)
                        pl2dAxialBone.pl2d_AngleSolver.legsAngle = Atan2Angle(pl2dAxialBone.pl2d_AngleSolver.controllers[0].BottomFoot().transform.position, pl2dAxialBone.pl2d_AngleSolver.controllers[1].BottomFoot().transform.position);
                    else
                        pl2dAxialBone.pl2d_AngleSolver.legsAngle = Atan2Angle(pl2dAxialBone.pl2d_AngleSolver.controllers[1].BottomFoot().transform.position, pl2dAxialBone.pl2d_AngleSolver.controllers[0].BottomFoot().transform.position);

                    pl2dAxialBone.pl2d_AngleSolver.boneAngle = new Vector3(0,0,0);
                    pl2dAxialBone.pl2d_AngleSolver.boneAngle.z = pl2dAxialBone.pl2d_AngleSolver.legsAngle;
                }
                else if (pl2dAxialBone.pL2dAnimator.axialBones.Count > 1)
                {
                    if (!pl2dAxialBone.pl2d_AngleSolver.isActiveAndEnabled)
                        pl2dAxialBone.pl2d_AngleSolver.enabled = true;

                    pl2dAxialBone.pl2d_AngleSolver.controllers = new List<PL2D_AxialBone>();
                    pl2dAxialBone.pl2d_AngleSolver.controllers.Add(pl2dAxialBone);
                    pl2dAxialBone.pl2d_AngleSolver.controllers.Add(pl2dAxialBone.pL2dAnimator.axialBones[pl2dAxialBone.axialBoneIndex - 1]);

                    if (pl2dAxialBone.BottomFoot().transform.position.x > pl2dAxialBone.pl2d_AngleSolver.controllers[1].BottomFoot().transform.position.x)
                        pl2dAxialBone.pl2d_AngleSolver.legsAngle = Atan2Angle(pl2dAxialBone.pl2d_AngleSolver.controllers[0].BottomFoot().transform.position, pl2dAxialBone.pl2d_AngleSolver.controllers[1].BottomFoot().transform.position);
                    else
                        pl2dAxialBone.pl2d_AngleSolver.legsAngle = Atan2Angle(pl2dAxialBone.pl2d_AngleSolver.controllers[1].BottomFoot().transform.position, pl2dAxialBone.pl2d_AngleSolver.controllers[0].BottomFoot().transform.position);

                    pl2dAxialBone.pl2d_AngleSolver.boneAngle = new Vector3(0,0,0);
                    pl2dAxialBone.pl2d_AngleSolver.boneAngle.z = pl2dAxialBone.pl2d_AngleSolver.legsAngle;
                }
                else
                {
                    if (pl2dAxialBone.pl2d_AngleSolver.isActiveAndEnabled)
                        pl2dAxialBone.pl2d_AngleSolver.enabled = false;
                }
            }

        }
        else
        {
            if (pl2dAxialBone.pl2d_AngleSolver.isActiveAndEnabled)
                pl2dAxialBone.pl2d_AngleSolver.enabled = false;
        }
    }

    void Update()
    {
        if (controllers[0].isGrounded)
        {
            if (controllers[0].angleSolverType == PL2D_AxialBone.AngleSolverEnum.Raycast)
            {
                raycastNormal = Physics2D.Raycast(transform.position, -controllers[0].axialBoneHit.normal, 200, PL2D_Animator.layerMask);
                Debug.DrawRay(transform.position, -controllers[0].axialBoneHit.normal * 200, Color.red);

                Vector3 axialBoneAngle = transform.eulerAngles;

                float angle = Mathf.LerpAngle(transform.eulerAngles.z,
                                                90 + Atan2Angle(raycastNormal.point, transform.position),
                                                Time.deltaTime * controllers[0].angleTrackSpeed);

                axialBoneAngle.z = angle;

                transform.eulerAngles = axialBoneAngle;
            }
            else if (controllers[0].angleSolverType == PL2D_AxialBone.AngleSolverEnum.RelativeToLegs)
            {
                // v1.1 - Fix: Breaking angle solving on flip due to negative scale
                if (!controllers[0].pl2d_PlayerController.isFlipped)
                {
                    if (controllers[0].RelativePoint(controllers[0].BottomFoot().transform.position).x > controllers[0].RelativePoint(controllers[1].BottomFoot().transform.position).x)
                        legsAngle = Mathf.LerpAngle(transform.eulerAngles.z,
                                                        Atan2Angle(controllers[0].BottomFoot().transform.position, controllers[1].BottomFoot().transform.position),
                                                        Time.deltaTime * controllers[0].angleTrackSpeed);
                    else
                        legsAngle = Mathf.LerpAngle(transform.eulerAngles.z,
                                                        Atan2Angle(controllers[1].BottomFoot().transform.position, controllers[0].BottomFoot().transform.position),
                                                        Time.deltaTime * controllers[0].angleTrackSpeed);
                }
                else
                {
                    if (controllers[0].RelativePoint(controllers[0].BottomFoot().transform.position).x < controllers[0].RelativePoint(controllers[1].BottomFoot().transform.position).x)
                        legsAngle = Mathf.LerpAngle(transform.eulerAngles.z,
                                                        Atan2Angle(controllers[0].BottomFoot().transform.position, controllers[1].BottomFoot().transform.position),
                                                        Time.deltaTime * controllers[0].angleTrackSpeed);
                    else
                        legsAngle = Mathf.LerpAngle(transform.eulerAngles.z,
                                                        Atan2Angle(controllers[1].BottomFoot().transform.position, controllers[0].BottomFoot().transform.position),
                                                        Time.deltaTime * controllers[0].angleTrackSpeed);
                }

                boneAngle.z = legsAngle;

                transform.eulerAngles = boneAngle;
            }
            else if (controllers[0].angleSolverType == PL2D_AxialBone.AngleSolverEnum.KeepVertical)
            {
                Vector3 axialBoneAngle = transform.eulerAngles;

                float angle = Mathf.LerpAngle(transform.eulerAngles.z,
                                                    0,
                                                    Time.deltaTime * controllers[0].angleTrackSpeed);

                axialBoneAngle.z = angle;
                transform.eulerAngles = axialBoneAngle;
            }

        }
        else
        {
            if (controllers[0].GetComponent<Rigidbody2D>())
            {
                Rigidbody2D rig = controllers[0].GetComponent<Rigidbody2D>();
                Debug.DrawRay(transform.position, new Vector3(rig.velocity.x * 0.5f, rig.velocity.y * 1f), Color.magenta);
            }

            // v1.1.1 - Refactored to a simpler code 
            Vector3 axialBoneAngle = transform.eulerAngles;
            axialBoneAngle.z = Mathf.LerpAngle(axialBoneAngle.z,
                                            0,
                                            Time.deltaTime * controllers[0].angleTrackSpeed);
            transform.eulerAngles = axialBoneAngle;

        }

    }

    /// <summary>
    /// Helper method that calculates the angle from two points
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    static public float Atan2Angle(Vector2 v1, Vector2 v2)
    {
        float _r = (Mathf.Atan2(v1.y - v2.y, v1.x - v2.x) * Mathf.Rad2Deg);
        return _r;
    }
}
