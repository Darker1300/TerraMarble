using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtility;
using Unity.VisualScripting;
using Sirenix;

public class Projectile : MonoBehaviour
{
    private BallStateTracker.BallState effector;
    public ObjectPooler pooler;
    public Vector2 TargetVector;
    [SerializeField] private float moveSpeed;
    private Transform planetCenter;
    public int projectileDammage;

   
   [Tag]
    public  string enemyTagName ;
   [Tag]
    public string wheelTagName ;

    private bool isEnemyProjectile = false;


    // Start is called before the first frame update
    private void Start()
    {
        moveSpeed = 50;
        planetCenter = GameObject.FindGameObjectWithTag(wheelTagName).transform;
        if (enemyTagName == "Ball") isEnemyProjectile = true;
    }

    public void StateConfigure(BallStateTracker.BallState state)
    {
        effector = state;
        switch (effector)
        {
            case BallStateTracker.BallState.Fire:
                //configure to Fire
                break;
            case BallStateTracker.BallState.Stomp:
                //configure to Stomp
                break;
            case BallStateTracker.BallState.NoEffector:
                //configure to NoEffector
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (((Vector2) planetCenter.position).Towards(transform.position)
            .sqrMagnitude > (float) (500 * 500)
            && gameObject.activeInHierarchy)
        {
            pooler.ReturnToPool(gameObject);
        }
    }

    private void FixedUpdate()
    {
        if (TargetVector.sqrMagnitude > 0f)
            transform.Translate(transform.rotation * TargetVector.normalized * (moveSpeed * Time.fixedDeltaTime));

        //transform.position = Vector2.MoveTowards(transform.position, targetTrans, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(enemyTagName))
        {
            //// example of collecting chain of enemies to explode; todo

            //var enemyChain = new HashSet<ExploConfigure>();
            //ExploConfigure targetExplode = collision.gameObject.GetComponent<ExploConfigure>();
           
            //    ExploConfigure.GetEnemyChain(targetExplode, enemyChain);

            //    foreach (ExploConfigure targetEnemy in enemyChain)
            //    {
            //        targetEnemy?.DoDistanceExplode(transform.position);
            //        // todo damage enemyHealth
            //        // targetEnemy.enemyHealth;

            //    } 

            if (isEnemyProjectile)
            {
                FindObjectOfType<PlayerHealth>().Damage(2);
            }

            if (gameObject.activeInHierarchy)
                pooler.ReturnToPool(gameObject);
        }
    }
    
}

//  public class ExplodeConfigure : MonoBehaviour
//  {
//      
//  }