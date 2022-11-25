using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoController : MonoBehaviour
{
    [SerializeField]
    private ObjectPooler pooler;
    [SerializeField]
    private GameObject currentProjectile;
    //

    //object pooler 
    //arrow prefab will have abillity to change types , flame arrow , ect 

    ///GET ARROW FROM POOL
    ///pass in type 
    public void GetProjectile(BallStateTracker.BallState state)
    {
       currentProjectile =  pooler.SpawnFromPool(transform.position, this.gameObject.transform, false);
        //MAKE SURE PROJECTILE FROM POOL IS CONFIGURED
        currentProjectile.GetComponent<Projectile>().StateConfigure(state);
        //PASS IN TARGET 

    
    }
    ///CONFIGURE AMMO TYPE
    ///

    ///ASSIGN TARGET TO PROJECTILE
    
    

    // Start is called before the first frame update
    void Start()
    {
        pooler = GetComponent<ObjectPooler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
