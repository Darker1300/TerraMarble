using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlanetApplier : MonoBehaviour
{
    public GameObject thisPlanet;
    public Rigidbody2D rb;
    public delegate void PlanetInfluence(Vector2 Direction);
    public event PlanetInfluence OnPlanetInfluence;

    public bool shouldRepel = true;
    public bool beAffectedByProjectile = false;
    // Start is called before the first frame update
    void Start()
    {
        thisPlanet = this.gameObject;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //be affected by same kind of projectiles
        if (beAffectedByProjectile)
        {
            if (collision.gameObject.layer == 6 && collision.gameObject.tag == this.gameObject.tag && collision.gameObject != thisPlanet.gameObject)
            {


                

                    Quaternion _rotation = transform.rotation;


                    Vector2 force = (collision.gameObject.GetComponent<GravitationalAtractor>().GetObjectToPlanet(transform.position, ref _rotation) );
                    //if (shouldRepel)
                    //    rb.AddForce(force);
                    //else
                    rb.AddForce(-force);
                    transform.rotation = _rotation;
                    OnPlanetInfluence?.Invoke(force);

                    //planetGravity.planet = collision.gameObject;
                    //planetGravity.applyGravity = true;
                    //transform.rotation = rotation;
                    //Quaternion.Slerp(  transform.rotation, rotation, 0.01f * Time.deltaTime);
                    //transform.Rotate(rotation.eulerAngles);
                    //transform.Rotate()= Quaternion.Euler(rotation.x) rotation;

                    //CurrentState = playerStates.DEFAULT;

                
            }
        }
        else
        {
            if (collision.gameObject.layer == 6 && collision.gameObject.tag != this.gameObject.tag && collision.gameObject != thisPlanet.gameObject)
            {




                Quaternion _rotation = transform.rotation;


                Vector2 force = (collision.gameObject.GetComponent<GravitationalAtractor>().GetObjectToPlanet(transform.position, ref _rotation));
                if (shouldRepel)
                {
                    rb.velocity = rb.velocity * 0.5f;
                    rb.AddForce(-force *4);
                    
                }
                else
                {
                    rb.velocity = rb.velocity * 0.5f;
                    rb.AddForce(force * 4);
                } 
                transform.rotation = _rotation;
                OnPlanetInfluence?.Invoke(force);

                //planetGravity.planet = collision.gameObject;
                //planetGravity.applyGravity = true;
                //transform.rotation = rotation;
                //Quaternion.Slerp(  transform.rotation, rotation, 0.01f * Time.deltaTime);
                //transform.Rotate(rotation.eulerAngles);
                //transform.Rotate()= Quaternion.Euler(rotation.x) rotation;

                //CurrentState = playerStates.DEFAULT;


            }
        }
    }
            private void OnTriggerEnter2D(Collider2D collision)
            {
                //intialOrbitEnter = true;


               // if (collision.gameObject.layer == 6 && collision.gameObject.tag != //planet)
                {
                    //OnEnteredAtmosphere?.Invoke(collision.GetComponent<Planet>().data);
                    //if (Vector2.Dot(rb.velocity, transform.right) <= 0)
                    //{
                    //    moveDirection = -Vector2.right;

                    //}
                    //else moveDirection = Vector2.right;

                    // Grounded = true;
                    //planetGravity.planet = collision.gameObject;
                    //planetGravity.applyGravity = true;
                    // var a = Quaternion.Angle(transform.rotation, rotation2);
                    //StartCoroutine(RotateOverTime(rotation1, rotation2, a / speed));
                    //StartCoroutine( RotateOverTime(transform.rotation, rotation2));
                    //Quaternion.Slerp(  transform.rotation, rotation, 0.01f * Time.deltaTime);
                    //transform.Rotate(rotation.eulerAngles);
                    //transform.Rotate()= Quaternion.Euler(rotation.x) rotation;
                    //CurrentState = playerStates.DEFAULT;

                }
            }
            public void ChangeAssignRepel(bool repel)
            {
                if (repel)
                {
                    shouldRepel = true;
                }
                else
                    shouldRepel = false;
            }
            public void SwapRepel()
            {
                if (shouldRepel)
                {
                    shouldRepel = false;
                }
                else
                    shouldRepel = true;
            } public void SwapBeingAffectedByOtherProjectiles()
            {
                if (beAffectedByProjectile)
                {
            beAffectedByProjectile = false;
                }
                else
            beAffectedByProjectile = true;
            }
        }
