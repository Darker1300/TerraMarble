using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using MathUtility;

public class ExploConfigure : MonoBehaviour
{
    public float explodeRadius = 5;
    public EnemyHealth enemyHealth;
    public UnityEvent Exploded;
    public const string enemyLayerName = "Flying";
    public CountDownTimer ExploTimer;
    public bool isExploding = false;
    private void Start()
    {
        enemyHealth ??= GetComponent<EnemyHealth>();
        ExploTimer = GetComponent<CountDownTimer>();
    }
    //public void SetCanExplode()
    //{
    //    canExplode = true;
    //}
    /// <returns>Collection of recursively nearby Enemy layer colliders, includes target.</returns>
    public static void GetEnemyChain(ExploConfigure target, HashSet<ExploConfigure> collected)
    {
        ContactFilter2D filter = new ContactFilter2D();
        int layerMask = 1 << LayerMask.NameToLayer(enemyLayerName);
        filter.SetLayerMask(layerMask);
        filter.useTriggers = true;
        GetEnemyChain(target, filter, collected);
    }


    /// <returns>Collection of recursively nearby Enemy layer colliders, includes target.</returns>
    public static void GetEnemyChain(ExploConfigure target, ContactFilter2D filter, HashSet<ExploConfigure> collected)
    {
        if (target == null || target.isExploding) return;

        collected.Add(target);

        List<Collider2D> nearbyEnemies = new List<Collider2D>();
        Physics2D.OverlapCircle(target.transform.position, target.explodeRadius, filter, nearbyEnemies);

        for (var index = 0; index < nearbyEnemies.Count; index++)
        {
            Collider2D enemyCollider = nearbyEnemies[index];
            ExploConfigure enemyExplode = enemyCollider.GetComponent<ExploConfigure>();

            if (enemyExplode == null || enemyExplode.isExploding || collected.Contains(enemyExplode)) continue;
            GetEnemyChain(enemyExplode, filter, collected);
        }
    }

    public void Explode()
    {
        Exploded.Invoke();
    }

    public void DoDistanceExplode(Vector3 pos)
    {
        isExploding = true;
        float time = (transform.position - pos).sqrMagnitude * GameObject.FindObjectOfType<ExplosionManager>().TimeToDistance;

        // Debug.Log("distance: " + (transform.position - pos).sqrMagnitude + " timeTO" + ((transform.position - pos).sqrMagnitude * GameObject.FindObjectOfType<ExplosionManager>().TimeToDistance));
        //caculate the distance and activate the timer component 
        ActivateExploCountdownTimer(time);

        //timer component will tell this when its done and explode 

    }
    //the countdownTimer will Trigger once finished will fire its countdown end event which in turn fires this scripts explo event
    public void ActivateExploCountdownTimer(float time)
    {
        ExploTimer.SetTimer(time);
        ExploTimer.enabled = true;

    }

    public void ConfigureOtherExplosion()
    {
        var enemyChain = new HashSet<ExploConfigure>();


        GetEnemyChain(this, enemyChain);

        foreach (ExploConfigure targetEnemy in enemyChain)
        {
          
            targetEnemy?.DoDistanceExplode(transform.position);
            // todo damage enemyHealth
            // targetEnemy.enemyHealth;
        }

    }
    private void OnEnable()
    {
        isExploding = false;
    }

}
