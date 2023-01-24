using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Shapes;

public class TreeBend : MonoBehaviour
{

    [SerializeField]
    private string TargetTag;
    [SerializeField]
    private Vector2 direction;
  
    public List<GameObject> nearbyTargets = new List<GameObject>();
    [SerializeField]
    private Vector2 WheelPosition;
    [SerializeField]
    private Transform wheel;
    private float min = 13;
    private float max= 19f;
    // Start is called before the first frame update
    private float coliderRadius;
    [SerializeField]
    private AnimationCurve treeBend;
    private AimTreeLockUI aimUi;

    //
    
    void Start()
    {
        aimUi = GameObject.FindObjectOfType<AimTreeLockUI>();
        //wheel = FindObjectOfType<Wheel>();
        coliderRadius = GetComponent<CircleCollider2D>().radius;

        InputManager.LeftDragVectorEvent += UpdatePosition;

    }

    // Update is called once per frame
    void Update()
    {
        UpdateTrees();

       // GameObject[] ffgfg = Physics2D.OverlapCircle(WheelPosition, 5,,);
    }

    private void UpdateTrees()
    {


        if (nearbyTargets.Count > 0)
        {
            foreach (var target in nearbyTargets)
            {
                //coliderRadius
             

                float percent = treeBend.Evaluate( Mathf.Clamp01( (target.transform.position - transform.position).magnitude / coliderRadius));
                target.GetComponent<RotateToDirectionNoRb>().RotateToThis(  direction,percent, ( transform.position ));
                

            }
        }
        
    }
    public void UpdatePosition(Vector2 dir,Vector2 delta)
    {
        //get the difference and reverse it
        float diference = (max - Mathf.Clamp(dir.magnitude, min, max));
        diference = min + diference;

        //invert
        transform.position = -((Vector3)dir.normalized *  diference);
        //Mathf.Clamp(dir.magnitude, min, max)
       // aimUi.BarIncrease( Mathf.Clamp01((target.transform.position - transform.position).magnitude / coliderRadius));
    }
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("dddsjd");
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
            collision.gameObject.GetComponent<RotateToDirectionNoRb>()?.StartCoroutine("ReturnToDefaultAngle");
        }
    }

}
