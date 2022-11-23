using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedDisperser : MonoBehaviour
{
    //prefab to spawn
    public GameObject spawnableObject;

    //where to shoot seeds out
    public Transform SpawnPoint;
    //how many seeds it has
    [SerializeField]
    int SeedAmount;
    //How Many seeds disperse from hit
    [SerializeField]
    int HitDisperseAmount;

    [SerializeField]
    int SeedsPerGrowth;
    //How Many seeds disperse from hit
    //[SerializeField]
    //int HitDisperseAmount;

    //seeds dispersed 
    
    public void GrownAddSeeds()
    {
        SeedAmount += SeedsPerGrowth;
    
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

        }
    }

}
