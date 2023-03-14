using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// v1.2 - char class for the Mecha scene
public class MechaSampleGame_Char : MonoBehaviour
{
    public PL2D_Animator animator;
    public PL2D_AxialBone mainAxialBone;
    public PL2D_PlayerController playerController;

    public float indicatorDistance;
    public virtual string DescriptionText { get; }
    public virtual Color ColorTextImage { get; }

    void Awake()
    {
        animator = transform.GetChild(0).GetComponent<PL2D_Animator>();
        mainAxialBone = animator.axialBones[0].mainBoneController;
        playerController = mainAxialBone.pl2d_PlayerController;
    }

}
