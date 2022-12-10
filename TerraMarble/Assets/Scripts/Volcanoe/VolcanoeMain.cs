using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoeMain : MonoBehaviour
{
    //LIST OF VOLCANOE STAGE VARIANTS
    public List<GameObject> VolcanoStagesObj = new List<GameObject>();
    public ObjectPooler pooler;
    volcanoeUI volcanoeUI;
    //color change 

    [SerializeField]
    private Color[] colors;

    [SerializeField]
    private float speed = 5f;

    void Start()
    {
        //INITIALIZE OBJECTS
        pooler = GetComponent<ObjectPooler>();
        pooler.CreatePool(20);
        volcanoeUI = GetComponentInChildren<volcanoeUI>();
        volcanoeUI.exploStartEndEvent += ShootVolcanicRock;
    }



    //COOLDOWN TIMER 


  
    public void ShootVolcanicRock(bool start,int stage)
    {
        GameObject Rock = pooler.SpawnFromPool(transform.position, null, true);
        //Rock.gameObject.SetActive(true);

        Rock.GetComponent<Rigidbody2D>().AddForce(transform.up * speed );
    }


    //IEnumerator ColorCool()
    //{ 
    ////lerp color 0-1 
    
    
    
    
    //}




    public void Stomped()
    {
        
    }

   
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }
}
