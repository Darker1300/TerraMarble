using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using MathUtility;

public class AutoAim : MonoBehaviour
{
    // Start is called before the first frame update
    private float aimDistance;
    //CircleCollider2D collider;
    [SerializeField]
    private string TargetTag;
    [SerializeField]
    private List<GameObject> nearbyTargets = new List<GameObject>();
    RaycastHit2D hit;
    [SerializeField]
    private LayerMask hitlayer;
    CircleCollider2D col;

    void Start()
    {
        //collider = GetComponent<CircleCollider2D>();
        col = GetComponent<CircleCollider2D>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //real one
    public GameObject FindClosestTarget()
    {

        float minDst = float.MaxValue;
        GameObject minDstObj = null;
        foreach (var gObj in nearbyTargets)
        {
            if (!CheckLineOfSight(gObj)) continue;
            float dst = (gObj.transform.position - transform.position).sqrMagnitude;
            if (dst < minDst)
            {
                minDst = dst;
                minDstObj = gObj;
                
            
            }
        Debug.Log("foundOne");
            

        }
        return minDstObj;

    }

    private bool CheckLineOfSight(GameObject gObj)
    {
        if (gObj.activeInHierarchy == false) return false;

        hit = Physics2D.Raycast(transform.position,transform.Towards(gObj.transform).normalized,col.radius,hitlayer.value);
      
       // Debug.DrawLine(transform.position,transform.position + transform.Towards(gObj.transform));
        if (hit != null && hit.collider.gameObject == gObj)
        {
            return true;
        }
        else return false;


    }


    //pass in a tag Enemy ect
    //GameObject FindClosestTarget()
    //{
    //    GameObject foundObject = nearbyTargets.Min(o =>
    //    {
    //        if (true) return o.transform.position - transform.position;
    //        else return Vector3.positiveInfinity;
    //    });
    //    return foundObject;

    //}
    //    var foundObject = gameObjects.Min(o =>
    //    {
    //        if (true) return o.transform.position - transform.position;
    //        else return Vector3.positiveInfinity;
    //    });

    //    //returns list 
    //    //return GameObject.FindGameObjectsWithTag(trgt).OrderBy(o => (o.transform.position - position).sqrMagnitude)
    //    //.FirstOrDefault();

    //    Vector3 position = transform.position + (Vector3)Pos;
    //    return GameObject.FindGameObjectsWithTag(trgt).OrderBy(o =>

    //    {

    //    (o.transform.position - position).sqrMagnitude && CheckLineOfSight(o.transform)).FirstorDefault()


    //    }
    //}

    //GameObject FindClosestTargetB(string trgt, Vector2 Pos)
    //{
    //    Vector3 position = transform.position + (Vector3)Pos;
    //    return GameObject.FindGameObjectsWithTag(trgt).OrderBy(o =>

    //    {
    //    if (CheckLineOfSight(o.transform))
    //    {
    //        (o.transform.position - position).sqrMagnitude && CheckLineOfSight(o)).FirstorDefault()
    //}







    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TargetTag))
        {
        nearbyTargets.Add(collision.gameObject);

        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag(TargetTag))
        {
            nearbyTargets.Remove(collision.gameObject);

        }
    }

}
