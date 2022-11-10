using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateGravityDirection : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private Transform planetCenter;
    [SerializeField]
    private Rigidbody2D rb;
    [SerializeField]
    private float maxGravDist = 4.0f;
    [SerializeField]
    private float maxGravity = 35.0f;
    
    private Vector3 direction;

    void Start()
    {
       rb =  GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateDirectionToCentre();


       
        rb.AddForce((direction.normalized * (2f - direction.magnitude / maxGravDist) * maxGravity)*Time.fixedDeltaTime);
        rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxGravity);
    }

    public void UpdateDirectionToCentre()
    {

        //if (rb.velocity.magnitude >= 0.0f)
        //{

        //}


        direction =  planetCenter.transform.position -  transform.position;
    
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        Vector2 project = Vector2.ClampMagnitude( rb.velocity.normalized - 2 * (Vector2.Dot(rb.velocity , collision.contacts[0].normal) * collision.contacts[0].normal), maxGravity);
        //project =  project;



        //rb.velocity.Set(0,0);
        //Debug.Log("coll" + dot);

        rb.AddForce(project);
        //rb.AddRelativeForce(project*10);

    }
}
