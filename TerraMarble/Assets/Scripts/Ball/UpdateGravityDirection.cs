using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

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
    public event EventHandler HitSurface;

    public Vector3 direction;
    //
    
    public Vector3 ExtraForceVector;

    void Start()
    {
       rb =  GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        UpdateDirectionToCentre();


       
       
        if (ExtraForceVector != Vector3.zero)
        {
            //rb.velocity = Vector2.zero;
            rb.AddRelativeForce(ExtraForceVector.normalized * 10000 * Time.fixedDeltaTime);
           
        }
        else
        {
           
            rb.AddForce(((direction.normalized * (2f - direction.magnitude / maxGravDist) * maxGravity) *5) * Time.fixedDeltaTime);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxGravity);


        }
    }

    public void UpdateDirectionToCentre()
    {

        //if (rb.velocity.magnitude >= 0.0f)
        //{

        //}


        direction =  planetCenter.transform.position -  transform.position;
    
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        ExtraForceVector = Vector2.zero;
        //Vector2 project = -2 * (Vector2.Dot(rb.velocity, collision.contacts[0].normal) * collision.contacts[0].normal + rb.velocity);
        //rb.velocity = Vector2.zero;
        //rb.velocity = project;
        HitSurface?.Invoke(this, EventArgs.Empty);








        // Vector2 project = Vector2.ClampMagnitude( rb.velocity.normalized - 20 * (Vector2.Dot(rb.velocity , collision.contacts[0].normal) * collision.contacts[0].normal), maxGravity);
        //project =  project;


        //rb.velocity.Set(0,0);
        //Debug.Log("coll" + dot);
        //rb.AddForce(project);
        //rb.AddRelativeForce(project*10);

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
       // ExtraForceVector = Vector2.zero;
        Vector2 project = Vector2.ClampMagnitude(rb.velocity.normalized - 20 * (Vector2.Dot(rb.velocity, collision.contacts[0].normal) * collision.contacts[0].normal), maxGravity);
        //project =  project;
        


    //rb.velocity.Set(0,0);
    //Debug.Log("coll" + dot);

        rb.AddForce(project* Time.fixedDeltaTime);
        //rb.AddRelativeForce(project*10);

    }
}
