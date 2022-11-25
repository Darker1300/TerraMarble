using System;
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
    private List<GameObject> nearbyTargets = new List<GameObject>();
    void Start()
    {
        //collider = GetComponent<CircleCollider2D>();



    }

    // Update is called once per frame
    void Update()
    {

    }
    //real one
    //public GameObject FindClosestTarget()
    //{

    //    float minDst = float.MaxValue;
    //    GameObject minDstObj = null;
    //    foreach (var gObj in nearbyTargets)
    //    {
    //        if (!CheckLineOfSight(gObj)) continue;
    //        float dst = (gObj.transform.position - transform.position).sqrMagnitude;
    //        if (dst < minDst)
    //            minDstObj = gObj;
    //    }

    //}

    //private bool CheckLineOfSight(GameObject gObj)
    //{

    //}


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
        nearbyTargets.Add(collision.gameObject);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        nearbyTargets.Remove(collision.gameObject);
    }

}
