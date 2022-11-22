using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedDisperser : MonoBehaviour
{
 
    //how many seeds it has
    [SerializeField]
    int SeedAmount;
    //How Many seeds disperse from hit
    [SerializeField]
    int HitDisperseAmount;

    //How Many seeds disperse from hit
    //[SerializeField]
    //int HitDisperseAmount;

    public void GrownAddSeeds(int Seeds)
    { 
        
    
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //FindWheelSection(collision.contacts[0].point);
        if (collision.gameObject.name == "Ball")
        {
            //if (collision.gameObject.GetComponent<>)
            //{

            //}

        }
    }

}
