using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class ExploConfigure : MonoBehaviour
{
    public float explodeRadius = 5;
    public EnemyHealth enemyHealth;
    [FormerlySerializedAs("ExplosionEvent")]
    public UnityEvent Exploded;
    public const string enemyLayerName = "Flying";
    
    private void Start()
    {
        enemyHealth ??= GetComponent<EnemyHealth>();
    }
     
    /// <returns>Collection of recursively nearby Enemy layer colliders, includes target.</returns>
    public static void GetEnemyChain(ExploConfigure target, HashSet<ExploConfigure> collected)
    {
        ContactFilter2D filter = new ContactFilter2D();
        int layerMask = 1 << LayerMask.NameToLayer(enemyLayerName);
        filter.SetLayerMask(layerMask);
        filter.useTriggers= true;
        GetEnemyChain(target, filter, collected);
    }


    /// <returns>Collection of recursively nearby Enemy layer colliders, includes target.</returns>
    public static void GetEnemyChain(ExploConfigure target, ContactFilter2D filter, HashSet<ExploConfigure> collected)
    {
        if (target == null) return;

        collected.Add(target);
        
        List<Collider2D> nearbyEnemies = new List<Collider2D>();
        Physics2D.OverlapCircle(target.transform.position, target.explodeRadius, filter, nearbyEnemies);

        for (var index = 0; index < nearbyEnemies.Count; index++)
        {
            Collider2D enemyCollider = nearbyEnemies[index];
            ExploConfigure enemyExplode = enemyCollider.GetComponent<ExploConfigure>();

            if (enemyExplode == null || collected.Contains(enemyExplode)) continue;
            GetEnemyChain(enemyExplode, filter, collected);
        }
    }

}
