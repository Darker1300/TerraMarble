using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnCollisionEnter2D(Collision2D collision)
    {

        //FindWheelSection(collision.contacts[0].point);
        if (collision.gameObject.name == "Wheel")
        {
            collision.gameObject.GetComponent<Wheel>().GetRegion(transform.position).MakeForest();

        }


    }
}
