using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnRandomUnitCirclePos : MonoBehaviour
{


    [SerializeField]
    private ObjectPooler pooler;

   

    // Start is called before the first frame update
    void Start()
    {

        pooler = GetComponent<ObjectPooler>();
        pooler.CreatePool(20);
    }




    // Update is called once per frame
    void Update()
    {
            
    }

    public void Test()
    {
        Spawn(Vector2.zero,3, 10);
    
    }
    //Pass in 
    private void Spawn(Vector2 centreToSpawnAround, int SpawnQauntity,float radius)
    {
        Vector3 center = centreToSpawnAround;
        for (int i = 0; i < SpawnQauntity; i++)
        {
            Vector3 pos = RandomCircle(center, radius);
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center - pos);

            GameObject spawnedObj = pooler.SpawnFromPool(pos, null, true);
            spawnedObj.transform.rotation = rot;


           
        }

    }
    //Get Pos on unit circle 
    Vector3 RandomCircle(Vector3 center, float radius)
    {
        float ang = Random.value * 360;
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        pos.z = center.z;
        return pos;
    }



}
