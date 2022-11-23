using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class UpdateGravityDirection : MonoBehaviour
{
    // Start is called before the first frame update
    
    private Transform planetCenter;
    
    private Rigidbody2D rb;
    [SerializeField]
    private float maxGravDist = 4.0f;
    [SerializeField]
    private float maxGravity = 35.0f;
    public event EventHandler HitSurface;
    [SerializeField]
    private bool canBounce=true;
    public Vector3 direction;
    //how much influence of magnitude carries over
    [SerializeField]
    [Range(0,1)]
    private float SlideFactor=1;
    public Vector3 ExtraForceVector;
    [SerializeField]
    private float slideMax;

    void Start()
    {
       rb =  GetComponent<Rigidbody2D>();
        planetCenter = GameObject.FindObjectOfType<Wheel>().transform;
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
        else if (canBounce)
        
        {
           
            rb.AddForce(((direction.normalized * (2f - direction.magnitude / maxGravDist) * maxGravity) *2) * Time.fixedDeltaTime);
            rb.velocity = Vector2.ClampMagnitude(rb.velocity, maxGravity);


        }else
        {
            rb.AddForce(((direction.normalized * (2f - direction.magnitude / maxGravDist) * maxGravity) * 2) * Time.fixedDeltaTime);

            



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

        if (canBounce)
        {
            Bounce(collision.contacts[0].normal);
        }
        else
        {
            Vector2 SurfaceDirection = Vector3.Cross(collision.contacts[0].normal, Vector3.forward);
            float dot = Vector2.Dot(rb.velocity, SurfaceDirection);
            Debug.Log("dot" + dot);
            //if above zero facing relativly same direction
            if (dot > 0)
            {

                rb.velocity = (SurfaceDirection * Mathf.Clamp( rb.velocity.magnitude * SlideFactor, 0,slideMax));

            }
            else if (dot <0)
            {
                rb.velocity = (-SurfaceDirection * Mathf.Clamp(rb.velocity.magnitude * SlideFactor, 0, slideMax));
            }else Bounce(collision.contacts[0].normal);


            // we turn bounce and 
            //Bounce(collision.contacts[0].normal);

        }





        // Vector2 project = Vector2.ClampMagnitude( rb.velocity.normalized - 20 * (Vector2.Dot(rb.velocity , collision.contacts[0].normal) * collision.contacts[0].normal), maxGravity);
        //project =  project;


        //rb.velocity.Set(0,0);
        //Debug.Log("coll" + dot);
        //rb.AddForce(project);
        //rb.AddRelativeForce(project*10);

    }

    public void Bounce(Vector2 normal)
    {

        // ExtraForceVector = Vector2.zero;
        Vector2 project = Vector2.ClampMagnitude(rb.velocity.normalized - 20 * (Vector2.Dot(rb.velocity, normal) * normal), maxGravity);
        rb.AddForce(project * Time.fixedDeltaTime);

    }
    private void OnCollisionStay2D(Collision2D collision)
    {

      
    }
}
