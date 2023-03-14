using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SampleGameController : MonoBehaviour
{
    public PL2D_Animator playerAnimator;

    public Sprite[] gunAimSprites = new Sprite[2];

    public Transform gunAim;
    public Transform gunBullet;
    public Transform gunShotPos;

    public Transform playerLifeBar;
    public Transform lifeBar;

    SamplePlayerController samplePlayerController;
    //list of enemies in scene
    List<SampleEnemyController> enemyList;

    public Transform[] enemyPrefabs = new Transform[2];

    void Start()
    {
        // add and initialize a player controller (related to the sample game, not to the asset)
        samplePlayerController = playerAnimator.axialBones[0].gameObject.AddComponent<SamplePlayerController>();
        samplePlayerController.Initialize(this, playerAnimator);

        enemyList = new List<SampleEnemyController>();

        foreach (Transform enemyPrefab in enemyPrefabs)
        {
            InstantiateEnemy(enemyPrefab);
        }

        panelImage.gameObject.SetActive(false);

        Physics2D.gravity = new Vector2(0, -90);
    }

    public void InstantiateEnemy(Transform enemy)
    {
        // Usage tip: Sharing the same animation to many instances  
        // The enemy prefab contains the enemy Body and its PL2D Animator already initialized.
        // To make all instances share the same Animations, make sure their PL2D Animator's GameObject name is the same.
        Transform enemyTransform = Instantiate(enemy);

        // reference to the instance PL2D Animator
        PL2D_Animator enemyAnimator = enemyTransform.GetChild(0).GetComponent<PL2D_Animator>();

        // load the "walk" animation of the enemyAnimator
        enemyAnimator.LoadAnimation("walk");

        // add and initialize an enemy controller (related to the sample game, not to the asset)
        SampleEnemyController enemyController = enemyAnimator.axialBones[0].gameObject.AddComponent<SampleEnemyController>();
        enemyController.Initialize(this, enemyAnimator);
        enemyList.Add(enemyController);
    }

    public void RemoveEnemy(SampleEnemyController enemyController)
    {
        // remove enemy from the list and instantiates a new random one
        enemyList.Remove(enemyController);
        InstantiateEnemy(Random.Range(0, 100) < 60 ? enemyPrefabs[0] : enemyPrefabs[1]);
    }

    float hideLifeBarCounter = 0;

    float fadePanelTimer = 0;

    void Update()
    {
        if (lifeBar.gameObject.activeSelf)
        {
            hideLifeBarCounter -= Time.deltaTime;
            lifeBar.rotation = Quaternion.identity;
        }

        if (hideLifeBarCounter <= 0)
        {
            lifeBar.gameObject.SetActive(false);
        }

        if (playerAnimator == null)
        {
            if (fadePanelTimer < 3)
            {
                panelImage.gameObject.SetActive(true);
                fadePanelTimer += Time.deltaTime;
                Color c = panelImage.color;
                c.a += fadePanelTimer;
                panelImage.color = c;

                infoText.text = "You died!\n\n" +
                "Number of kills: " + statsNumberOfKills + "\n" +
                "Hit Rate: " + (statsHitShots / statsTotalShots) * 100 + "%\n\n" +
                "Restart game.";

                Color c2 = infoText.color;
                c2.g -= fadePanelTimer;
                c2.b -= fadePanelTimer;
                infoText.color = c2;

            }
        }
        else
        {
            if (samplePlayerController.transform.position.y < -80)
            {
                samplePlayerController.Life = 0;
            }
        }
    }

    public Image panelImage;

    public Text infoText;
    public Text gravityInfo;

    public int statsNumberOfKills = 0;
    public float statsTotalShots = 0;
    public float statsHitShots = 0;

    public void SetLifeBar(float life, Transform target)
    {
        lifeBar.position = target.position + new Vector3(0, 10, 0);
        lifeBar.GetChild(0).localScale = new Vector3(life / 100, 1, 1);
        lifeBar.SetParent(target);
        lifeBar.gameObject.SetActive(true);
        hideLifeBarCounter = 3;
    }

    public IEnumerator KillBullet(Transform bullet)
    {
        yield return new WaitForSeconds(1f);
        if (bullet)
            Destroy(bullet.gameObject);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
