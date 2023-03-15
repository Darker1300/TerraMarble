using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtility;
using Unity.VisualScripting;

public class Projectile : MonoBehaviour
{
    private BallStateTracker.BallState effector;
    public ObjectPooler pooler;
    public Vector2 TargetDirection;
    [SerializeField] private float moveSpeed;
    private Transform planetCenter;

   
    private const string enemyTagName = "Enemy";
    private const string wheelTagName = "Enemy";


    // Start is called before the first frame update
    private void Start()
    {
        moveSpeed = 50;
        planetCenter = GameObject.FindGameObjectWithTag(wheelTagName).transform;
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

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (TargetDirection != null && Vector3.Distance(transform.position, planetCenter.position) < 500)
            transform.Translate(transform.rotation * TargetDirection.normalized * moveSpeed * Time.fixedDeltaTime);

        //transform.position = Vector2.MoveTowards(transform.position, targetTrans, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(enemyTagName))
        {
            // example of collecting chain of enemies to explode; todo

            var enemyChain = new HashSet<ExploConfigure>();
            ExploConfigure targetExplode = collision.gameObject.GetComponent<ExploConfigure>();
            ExploConfigure.GetEnemyChain(targetExplode, enemyChain);

            foreach (ExploConfigure targetEnemy in enemyChain)
            {
                targetEnemy?.Exploded.Invoke();
                // todo damage enemyHealth
                // targetEnemy.enemyHealth;
                
            }

            pooler.ReturnToPool(gameObject);
        }
    }
    
}

//  public class ExplodeConfigure : MonoBehaviour
//  {
//      
//  }