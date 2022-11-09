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


       
        rb.AddForce(direction.normalized * (1.0f - direction.magnitude / maxGravDist) * maxGravity);
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
        float dot = Vector2.Dot(-collision.contacts[0].normal, rb.velocity);

        Debug.Log("coll" + dot);

        rb.velocity = collision.contacts[0].normal * 10.0f;
    }
}
