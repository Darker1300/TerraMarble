using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    [SerializeField]
    private ObjectPooler pooler;
  
    public GameObject currentProjectile;
    
    //
    
    //object pooler 
    //arrow prefab will have abillity to change types , flame arrow , ect 

    ///GET ARROW FROM POOL
    ///pass in type 
    public void GetProjectile(BallStateTracker.BallState state,Vector2 pos)
    {
       currentProjectile =  pooler.SpawnFromPool(transform.position, null, true);
        //MAKE SURE PROJECTILE FROM POOL IS CONFIGURED
        currentProjectile.GetComponent<Projectile>().StateConfigure(state);
        currentProjectile.GetComponent<Projectile>().TargetDirection = pos - (Vector2)transform.position;
        currentProjectile.GetComponent<Projectile>().pooler = pooler;


        //PASS IN TARGET 


    }
    ///CONFIGURE AMMO TYPE
    ///

    ///ASSIGN TARGET TO PROJECTILE
    
    

    // Start is called before the first frame update
    void Start()
    {

        pooler = GetComponent<ObjectPooler>();
        pooler.CreatePool(20);   
    }



    // Update is called once per frame
   
}
