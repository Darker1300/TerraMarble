using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AutoAim : MonoBehaviour
{
    // Start is called before the first frame update
    private float aimDistance;
    //CircleCollider2D collider;
    [SerializeField]
    private string TargetTag;

    void Start()
    {
        //collider = GetComponent<CircleCollider2D>();
        


    }

    // Update is called once per frame
    void Update()
    {
        
    }
 
    //pass in a tag Enemy ect
    GameObject FindClosestTarget(string trgt, Vector2 Pos)
    {
        Vector3 position = transform.position + (Vector3)Pos;
        return GameObject.FindGameObjectsWithTag(trgt).OrderBy(o => (o.transform.position - position).sqrMagnitude)
            .FirstOrDefault();
    }
   
}
