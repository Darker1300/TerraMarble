using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityUtility;
using MathUtility;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private LayerMask projLayerName;
    [SerializeField] private LayerMask BallLayerName;
    public float CurrentDelayTime;
    [SerializeField] private float KnockBackDuration = 1.25f;
    public UnityEvent<Collider2D> OnProjectileHit;
    public UnityEvent OnStunEnd;
    private Rigidbody2D rb;
    [SerializeField]
    private float knockBackForce = 100;

    public int HitAmount;
    public bool CanFertilize = false;
    public SpawnRandomUnitCirclePos spawnRandomCircle;
    public bool canExplode = false;
    public float explosionRadius = 2f;
    

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update()
    {

        if (CurrentDelayTime > 0)
        {
            CurrentDelayTime -= Time.deltaTime;
        }
        else
        {
            OnStunEnd?.Invoke();
            
        }
    }
   

    public void OnHit()
    {
        CurrentDelayTime = KnockBackDuration;
    }
    private void OnDisable()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("KnockBack" + collision.gameObject.name);

        if (collision.gameObject.CompareLayerMask(projLayerName))
        {
            OnProjectileHit?.Invoke(collision);
        }
        //else if (collision.gameObject.CompareLayerMask(BallLayerName))
        //{
        //    collision.GetComponent<Rigidbody2D>()
        //        .AddForce(
        //            collision.GetComponent<Rigidbody2D>().velocity.normalized
        //            * (collision.GetComponent<Rigidbody2D>().velocity.magnitude * 20.2f));
        //    //im trying to increases velocity but its getting capped 
        //    //if player hits enemy player and explodes 
        //    collision.GetComponent<TimerSmoothSlowDown>().enabled = true;
        //    EnemyDead();


        //    //collision.GetComponent<BallBounce>().Bounce(-collision.transform.Towards(GameObject.FindObjectOfType<Wheel>().transform));


        //}

    }

    public void DammageTimer()
    { 

    //player is hiting through enemys the first hit does most damage and then and after that does less percentage
    //so the player doesnt die from one mistake

     //effect we are after is we want it to look like the player slowed by collision then powered by explosion
     //two options time slow cinematic effect aprouch 
    //when hit player banks its velocity and 
    
    }

    public void KnockBack(Collider2D colider)
    {

        if (rb == null)
            return;

        HitAmount++;
        rb.velocity = Vector2.zero;
        OnHit();
        rb.AddForce(colider.transform.Towards(transform).normalized * knockBackForce);




    }
    public void EnemyDead()
    {


        GameObject.FindObjectOfType<FruitManager>().FertilizeNearby(transform.position);
        //tell the spawner it has 
       spawnRandomCircle.ParticleSpawn(transform);
        //return to pool
        GetComponent<PoolObject>()?.Pool.ReturnToPool(this.gameObject);


    }


}
