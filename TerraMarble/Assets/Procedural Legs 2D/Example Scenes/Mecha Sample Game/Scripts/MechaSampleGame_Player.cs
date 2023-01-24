using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// v1.2 - player class for the Mecha scene
public class MechaSampleGame_Player : MechaSampleGame_Char
{
    public override string DescriptionText
    {
        get => "-- Mecha --\n" +
                "Move: A, S, D\n" +
                "Jump: Space\n" +
                "Switch Character: Tab";
    }
    public override Color ColorTextImage
    {
        get => new Color(0.6603774f, 0.6603774f, 0.6603774f);
    }

    void Start()
    {
        indicatorDistance = 3;
        
        animator.LoadAnimation("Walk");
        mainAxialBone.playerControllerType = PL2D_AxialBone.PlayerControllerEnum.KeyboardInput;
        playerController.PlayPathFollow();
    }

    IEnumerator ApplyVelocityForSeconds(float seconds)
    {
        float timer = 0;
        while (timer < seconds)
        {
            playerController.rig2d.velocity = (new Vector2(2, playerController.rig2d.velocity.y));
            timer += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    void Update()
    {
        if (mainAxialBone.axialBoneHit.transform && mainAxialBone.axialBoneHit.transform.name == "Limit")
        {
            if (mainAxialBone.MoveDirection.x > 0 && Input.GetKey(KeyCode.D))
            {
                if (animator.animationName == "Walk")
                {
                    mainAxialBone.bodyVelocity = 10;
                    if (!mainAxialBone.isJumping)
                    {
                        playerController.pressedJump = true;
                    }
                    //playerController.rig2d.AddForce(new Vector2(500, 0));
                    StartCoroutine(ApplyVelocityForSeconds(0.5f));
                }
                else
                {
                    playerController.positionTarget.position = mainAxialBone.transform.position + new Vector3(-0.05f, 0, 0);
                }
            }
        }

        if (mainAxialBone.playerControllerEnabled)
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                animator.LoadAnimation("Crouch");
            }

            if (Input.GetKeyUp(KeyCode.S))
            {
                animator.LoadAnimation("Walk");
            }
        }
    }

    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    Debug.Log(other);
    //    //Debug.Log(mainAxialBone.MoveDirection);
    //}
}
