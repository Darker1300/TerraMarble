using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class SpawnRandomUnitCirclePos : MonoBehaviour
{


    [SerializeField]
    private ObjectPooler pooler;

    [SerializeField]
    private ObjectPooler particlePooler;
    [SerializeField]
    private float minRad = 15;
    [SerializeField]
    private float maxRad = 25;
    public bool SpawnInBunches = false;

    public bool aroundPlayer;
    [SerializeField]
    public float distanceAroundPlanet;
    [SerializeField]
    private int amount;
    [SerializeField]
    private int bunchSpacing;
    [SerializeField]
    private float distanceAroundPlayer;

    // Start is called before the first frame update
    void Start()
    {

        pooler = GetComponent<ObjectPooler>();
        //  pooler.CreatePool(20);
        //particlePooler.CreatePool(20);

    }




    // Update is called once per frame
    void Update()
    {

    }

    public void Test()
    {
        SpawnController();
        //Spawn(Vector2.zero, 3, Random.Range(minRad, maxRad));

    }
    public void SpawnController()
    {
        //around player
        if (aroundPlayer)
        {
            //bunches
            SpawnAroundPlayer(amount);

        }//around planet
        else SpawnAroundPlanet(amount);
    }
    public void SpawnAroundPlayer(int Amount)
    {
        if (!SpawnInBunches)
        {

            Spawn(Camera.main.GetComponent<FollowBehavior>().trackingTarget.transform.position, Amount, distanceAroundPlayer);
        }
        else Spawn(RandomCircle(Vector2.zero, distanceAroundPlayer), amount, bunchSpacing);

    }
    public void SpawnAroundPlanet(int Amount)
    {
        if (!SpawnInBunches)
        {

            Spawn(Vector2.zero, Amount, distanceAroundPlanet);
        }
        else//spawn a bunch of enemies around a random point
            Spawn(RandomCircle(Vector2.zero, distanceAroundPlanet), amount, bunchSpacing);

    }
    //Pass in 
    private void Spawn(Vector2 centreToSpawnAround, int SpawnQauntity, float radius)
    {

        //spaced around world





        Vector3 center = centreToSpawnAround;


        for (int i = 0; i < SpawnQauntity; i++)
        {
            Vector3 pos = RandomCircle(center, radius);
            Quaternion rot = Quaternion.FromToRotation(Vector3.right, center - pos);

            GameObject spawnedObj = pooler.SpawnFromPool(pos, null);
            spawnedObj.transform.rotation = rot;
            spawnedObj.GetComponent<EnemyHealth>().spawnRandomCircle = this;

        }

    }
    public void ParticleSpawn(Transform transform)
    {
        particlePooler.SpawnFromPool(transform.position, null);

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

    private void OnDrawGizmosSelected()
    {
        Wheel wheel = FindObjectOfType<Wheel>();
        Gizmos.color = Color.red;

        GizmosExtensions.DrawWireCircle(wheel.transform.position, minRad, 36, Quaternion.LookRotation(Vector3.up, Vector3.forward));
        GizmosExtensions.DrawWireCircle(wheel.transform.position, maxRad, 36, Quaternion.LookRotation(Vector3.up, Vector3.forward));

    }

}
