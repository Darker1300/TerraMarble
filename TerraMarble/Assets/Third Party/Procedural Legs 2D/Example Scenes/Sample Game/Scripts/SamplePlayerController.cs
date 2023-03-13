using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SamplePlayerController : MonoBehaviour
{
    PL2D_Animator playerAnimator;
    SampleGameController sampleGameController;

    public float life = 100;
    public float Life
    {
        get
        {
            return life;
        }
        set
        {
            life = value;
            sampleGameController.playerLifeBar.GetChild(0).localScale = new Vector3(life / 100, 1, 1);
        }
    }

    string state = "run";

    public void Initialize(SampleGameController sampleGameController, PL2D_Animator playerAnimator)
    {
        this.playerAnimator = playerAnimator;
        this.sampleGameController = sampleGameController;
    }

    void Start()
    {
        playerAnimator.LoadAnimation("run");
    }

    void Update()
    {
        if (life <= 0)
        {
            KillPlayer();
            life = 0;
        }

        PlayerStateChooser();
        AimPlayerGun();
        PlayerFireGun();
        CameraFollow();

        if (playerAnimator.axialBones[0].isJumping)
        {
            playerAnimator.axialBones[0].pl2d_PlayerController.bodyAcceleration = 20;
        }
        else
        {
            playerAnimator.axialBones[0].pl2d_PlayerController.bodyAcceleration = 5;
        }
    }

    void CameraFollow()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, transform.position + new Vector3(0, 0, -10), Time.deltaTime * 1);
    }

    void KillPlayer()
    {
        Destroy(transform.parent.gameObject);
    }

    void PlayerStateChooser()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            state = "crouch";
            playerAnimator.LoadAnimation("crouch");
            sampleGameController.gunAim.GetComponent<SpriteRenderer>().sprite = sampleGameController.gunAimSprites[1];
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            state = "run";
            playerAnimator.LoadAnimation("run");
            sampleGameController.gunAim.GetComponent<SpriteRenderer>().sprite = sampleGameController.gunAimSprites[0];
        }
    }

    Transform target;
    Vector3 mouseWorldPoint;

    void AimPlayerGun()
    {
        target = playerAnimator.iKManager.solvers[2].transform;

        mouseWorldPoint = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

        sampleGameController.gunAim.position = mouseWorldPoint;

        target.position = mouseWorldPoint;

        if (!playerAnimator.axialBones[0].pl2d_PlayerController.isFlipped)
            target.eulerAngles = new Vector3(0, 0, PL2D_AngleSolver.Atan2Angle(mouseWorldPoint, playerAnimator.axialBones[0].transform.position));
        else
            target.eulerAngles = new Vector3(0, 0, PL2D_AngleSolver.Atan2Angle(playerAnimator.axialBones[0].transform.position, mouseWorldPoint) + 40);
    }

    void PlayerFireGun()
    {
        if (Input.GetMouseButtonDown(0))
        {
            sampleGameController.statsTotalShots++;

            Quaternion rot = !playerAnimator.axialBones[0].pl2d_PlayerController.isFlipped ? sampleGameController.gunShotPos.rotation :
                                                    Quaternion.Euler(sampleGameController.gunShotPos.eulerAngles.x, sampleGameController.gunShotPos.eulerAngles.y, sampleGameController.gunShotPos.eulerAngles.z + 180);

            Transform bullet = Instantiate(sampleGameController.gunBullet, sampleGameController.gunShotPos.position, rot);
            Vector3 dir = new Vector3(0,0,0);

            if (state == "crouch")
            {
                dir = (mouseWorldPoint - sampleGameController.gunShotPos.position).normalized;
            }
            else if (state == "run")
            {
                dir = (mouseWorldPoint - (sampleGameController.gunShotPos.position + new Vector3(Random.Range(-7, 8), Random.Range(-7, 8), 0))).normalized;
            }

            bullet.name = "GunBullet";
            bullet.GetComponent<Rigidbody2D>().AddForce(dir * 5000);
            StartCoroutine(sampleGameController.KillBullet(bullet));
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "(a0) - Bug 1" || other.gameObject.name == "(a0) - Bug 2")
        {
            Life -= 10;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.name == "NoWallWalk")
        {
            playerAnimator.axialBones[0].pl2d_PlayerController.enableWallWalk = false;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name == "NoWallWalk")
        {
            playerAnimator.axialBones[0].pl2d_PlayerController.enableWallWalk = true;
        }
    }
}
