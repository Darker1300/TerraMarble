using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    [SerializeField] private ObjectPooler pooler;
  
    public GameObject currentProjectile;
    
    //object pooler 
    //arrow prefab will have abillity to change types , flame arrow , ect 

    //GET ARROW FROM POOL
    //pass in type 
    public void GetProjectile(BallStateTracker.BallState state,Vector2 pos)
    {
       currentProjectile =  pooler.SpawnFromPool(transform.position, null);
        //MAKE SURE PROJECTILE FROM POOL IS CONFIGURED
        Projectile projectile = currentProjectile.GetComponent<Projectile>();
        projectile.StateConfigure(state);
        projectile.TargetVector = pos - (Vector2)transform.position;
        projectile.pooler = pooler;
        
        //PASS IN TARGET 
    }
    //CONFIGURE AMMO TYPE

    //ASSIGN TARGET TO PROJECTILE
    
}
