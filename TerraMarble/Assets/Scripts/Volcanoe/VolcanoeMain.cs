using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolcanoeMain : MonoBehaviour
{
    //LIST OF VOLCANOE STAGE VARIANTS
    public List<GameObject> VolcanoStagesObj = new List<GameObject>();
    public ObjectPooler pooler;

    [SerializeField]
    private float speed = 5f;

    void Start()
    {
        //INITIALIZE OBJECTS
        pooler = GetComponent<ObjectPooler>();
        pooler.CreatePool(20);
    }



    //COOLDOWN TIMER 


  
    public void ShootVolcanicRock(int amount)
    {
        GameObject Rock = pooler.SpawnFromPool(transform.position, null, true);
        //Rock.gameObject.SetActive(true);

        Rock.GetComponent<Rigidbody2D>().AddForce(Vector2.up * speed * Time.deltaTime);
    }
    





    public void Stomped()
    {
        
    }

    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        
    }
}
