using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExploConfigure : MonoBehaviour
{
    public float explodeRadius = 5;
    public EnemyHealth enemyHealth;
    public UnityEvent ExplosionEvent;
   

    private void Start()
    {
        enemyHealth ??= GetComponent<EnemyHealth>();
    }

     
    /// <returns>Collection of recursively nearby Enemy layer colliders, includes target.</returns>
    public static void GetEnemyChain(ExploConfigure target, HashSet<ExploConfigure> collected)
    {
        ContactFilter2D filter = new ContactFilter2D();
        const string enemyLayerName = "Projectile";
        filter.SetLayerMask(LayerMask.NameToLayer(enemyLayerName));
        filter.useTriggers= true;
        GetEnemyChain(target, filter, collected);
    }


    /// <returns>Collection of recursively nearby Enemy layer colliders, includes target.</returns>
    public static void GetEnemyChain(ExploConfigure target, ContactFilter2D filter, HashSet<ExploConfigure> collected)
    {
        if (target == null) return;

        collected.Add(target);

        Collider2D[] nearbyEnemies;
        nearbyEnemies =  Physics2D.OverlapCircleAll(target.transform.position, target.explodeRadius, 1 << LayerMask.NameToLayer("Projectile"));
        //Physics2D.OverlapCircle(target.transform.position, target.explodeRadius, filter, nearbyEnemies);

        for (var index = 0; index < nearbyEnemies.Length; index++)
        {
            Collider2D enemyCollider = nearbyEnemies[index];
            ExploConfigure enemyExplode = enemyCollider.GetComponent<ExploConfigure>();

            Debug.Log(enemyExplode.name + " invoked Container of " + nearbyEnemies.Length);
            if (enemyExplode == null || collected.Contains(enemyExplode)) continue;
            GetEnemyChain(enemyExplode, filter, collected);
        }
    }

}
