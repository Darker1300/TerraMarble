using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleEnemyController : MonoBehaviour
{
    PL2D_Animator enemyAnimator;
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
            sampleGameController.SetLifeBar(life, transform);
        }
    }

    public void Initialize(SampleGameController sampleGameController, PL2D_Animator enemyAnimator)
    {
        this.enemyAnimator = enemyAnimator;
        this.sampleGameController = sampleGameController;

        enemyAnimator.axialBones[0].pl2d_PlayerController.PlayPathFollow();
    }

    void Start()
    {

    }

    void Update()
    {
        if (life <= 0)
        {
            KillEnemy();
        }
    }

    void KillEnemy()
    {
        sampleGameController.statsNumberOfKills++;
        sampleGameController.lifeBar.SetParent(null);
        sampleGameController.lifeBar.gameObject.SetActive(false);
        sampleGameController.RemoveEnemy(this);
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.name == "GunBullet")
        {
            sampleGameController.statsHitShots++;
            Destroy(other.gameObject);
            Life -= 10;
        }
    }
}
