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
        //pooler.CreatePool(20);
        volcanoeUI = GetComponentInChildren<volcanoeUI>();
        volcanoeUI.exploStartEndEvent += ShootVolcanicRock;
    }



    //COOLDOWN TIMER 


  
    public void ShootVolcanicRock(bool start,int stage,float explosionYheightStart)
    {
        if (start)
        {
            GameObject Rock = pooler.SpawnFromPool(transform.position + transform.up * explosionYheightStart , null);
            Rock.transform.up = transform.up;
            
            //Rock.transform.position = new Vector3(Rock.transform.position.x,Rock.transform.position.y + explosionYheightStart+10,Rock.transform.position.z);
            //Rock.gameObject.SetActive(true);
            //Rock.transform.Translate(,Space.World );
            Rock.GetComponent<Rigidbody2D>().AddForce( Vector2.up * speed);
        }
        
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
