using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class ChangeLayerInner : MonoBehaviour
{
    [SerializeField]
    private float HuffMaxSpeed =3;
    [SerializeField]
    private float HuffSpeed = 3;
    [SerializeField]
   // private float OutSpeed = 5;
    private bool huffing;

    private Vector2 force;
    [SerializeField]
    private Polygon polygon;
    [SerializeField]
    private Color endColor;
    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(HuffPuff(120,30f));
       // polygon = GetComponent<Polygon>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    IEnumerator HuffPuff(float Overallduration,float inbetween)
    {
        bool blowing = false;
       
        // float startScale = thisComponent.Radius;
        float overallTimer = 0;
        Color start = polygon.Color;
        while (overallTimer <= Overallduration)
        {
       
            blowing = !blowing;
            huffing = blowing;
            float Timer = 0f;
            while (Timer <= inbetween)
            {
                Timer = Timer + Time.deltaTime;
                float percent = Mathf.Clamp01(Timer / inbetween);
                //thisComponent.Color = Color.Lerp(startColor, endcolor, percent);
                HuffSpeed = blowing ? Mathf.Lerp(1.0f, HuffMaxSpeed, percent) : Mathf.Lerp(HuffMaxSpeed, 1.0f, percent);
                force =  blowing?    1200f * HuffMaxSpeed * transform.up : -transform.up *HuffMaxSpeed;

                polygon.Color = Color.Lerp(start,endColor,percent);

                yield return null;
            }
            overallTimer = overallTimer + inbetween;
        }
    }

        private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Inner") )
        {
            if (!huffing )
            {

            other.gameObject.layer = LayerMask.NameToLayer("Inner");
            }
            //if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
            //{
            // other.transform.up = transform.up;
           // other.GetComponent<Rigidbody2D>().velocity = Vector2.Reflect(other.GetComponent<Rigidbody2D>().velocity,transform.up);

            //}
        }
        else
            other.gameObject.layer = LayerMask.NameToLayer("Ball");
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Inner"))
        {
            //other.transform.up = transform.up;
            other.GetComponent<Rigidbody2D>().AddForce(force * HuffSpeed * Time.deltaTime);
            //if (other.gameObject.layer == LayerMask.NameToLayer("Ball"))
            //{


            //}
        }
        //else
        //    other.transform.up = transform.up;
        //    other.GetComponent<Rigidbody2D>().AddForce(transform.up * OutSpeed * Time.deltaTime);

    }
}

