using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathUtility;

public class Projectile : MonoBehaviour
{

    BallStateTracker.BallState effector;
    public ObjectPooler pooler;
    public Vector2 TargetDirection;
    [SerializeField]
    private float moveSpeed;
    private Transform planetCenter;



    // Start is called before the first frame update
    void Start()
    {
        moveSpeed =50;
        planetCenter = GameObject.FindGameObjectWithTag("Wheel").transform;
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
    void Update()
    {
        if (TargetDirection != null && Vector3.Distance(transform.position,planetCenter.position) < 500)
        {
            transform.Translate(transform.rotation* TargetDirection.normalized * moveSpeed * Time.deltaTime);
           
            //transform.position = Vector2.MoveTowards(transform.position, targetTrans, moveSpeed * Time.deltaTime);
        }
        


    }
    
    //public void ConfigureThisDirPos()
    //{
    //    transform.position - targetTrans.position;

    //}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
           
        }
        pooler.ReturnToPool(this.gameObject);
    }
    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (!collision.gameObject.CompareTag("Ball"))
    //    {
    //        pooler.ReturnToPool(this.gameObject);
    //    }
    //}
   
}
