using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{

    //public void GetExplodableObjectsNearby(Vector3 pos, float radius)
    //{
    //    Collider2D[] allOverlappingColliders = Physics2D.OverlapCircleAll(pos, radius, 1 << LayerMask.NameToLayer("Flying") )
    //        .Where(col => IsExplodable(col))
    //        .ToArray();

    //    foreach (var collider in allOverlappingColliders)

    //    { 
    //        //IF OUR EXPLODABLE HAS HEALTH
    //        if (collider.gameObject.GetComponent<EnemyHealth>() != null)
    //        {

    //        }
    //        collider.GetComponent<EnemyHealth>().EnemyDead();


    //    }

    //}

    //public bool IsExplodable(Collider2D col)
    //{
    //    //if it has a explosion component and has health script
    //    if (col.gameObject.GetComponent<ExploConfigure>()!= null  )
    //        return true;
    //    return false;
    //}
   
}
