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

    private const string enemyLayerName = "Enemy";
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
            //  // example of collecting chain of enemies to explode; todo

            //  var enemyChain = new HashSet<ExplodeConfigure>();
            //  ExplodeConfigure targetExplode = collision.gameObject.GetComponent<ExplodeConfigure>();
            //  ExplodeConfigure.GetEnemyChain(targetExplode, enemyChain);

            //  foreach (ExplodeConfigure targetEnemy in enemyChain)
            //  {
            //      // todo damage enemyHealth
            //          // targetEnemy.enemyHealth;
            //  }

            pooler.ReturnToPool(gameObject);
        }
    }
    
}

//  public class ExplodeConfigure : MonoBehaviour
//  {
//      public float explodeRadius = 5;
//      public EnemyHealth enemyHealth;

//      private void Start()
//      {
//          enemyHealth ??= GetComponent<EnemyHealth>();
//      }

//      /// <returns>Collection of recursively nearby Enemy layer colliders, includes target.</returns>
//      public static void GetEnemyChain(ExplodeConfigure target, HashSet<ExplodeConfigure> collected)
//      {
//          ContactFilter2D filter = new ContactFilter2D();
//          const string enemyLayerName = "Enemy";
//          filter.SetLayerMask(LayerMask.NameToLayer(enemyLayerName));
//          GetEnemyChain(target, filter, collected);
//      }


//      /// <returns>Collection of recursively nearby Enemy layer colliders, includes target.</returns>
//      public static void GetEnemyChain(ExplodeConfigure target, ContactFilter2D filter, HashSet<ExplodeConfigure> collected)
//      {
//          if (target == null) return;

//          collected.Add(target);

//          Collider2D[] nearbyEnemies = new Collider2D[0];
//          Physics2D.OverlapCircle(target.transform.position, target.explodeRadius, filter, nearbyEnemies);

//          for (var index = 0; index < nearbyEnemies.Length; index++)
//          {
//              Collider2D enemyCollider = nearbyEnemies[index];
//              ExplodeConfigure enemyExplode = enemyCollider.GetComponent<ExplodeConfigure>();
//              if (enemyExplode == null || collected.Contains(target)) continue;
//              GetEnemyChain(enemyExplode, filter, collected);
//          }
//      }
//  }