using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityUtility;

public class FruitManager : MonoBehaviour
{
    private CircleCollider2D collider;

    public List<GameObject> fruitPrefabs = new();
    [Header("Spawn Positions")] public Transform gizmoTreetopTransform;
    [SerializeField] private float CoverDistance = 6f;
    [SerializeField] private float MiddleOffset = 0f;
    [SerializeField] private float radius;

    [Header("Fruit Placement Quick Config X")] [SerializeField]
    private float xFruit = 4f;

    [SerializeField] private float gizmoFruitRadius = 0.1f;
    [SerializeField] private ObjectPooler blueFruit;
    [SerializeField] private ObjectPooler RedFruit;
    [SerializeField] private ObjectPooler YellowFruit;
    public int fruitCount = 6;
    public float worldSurfaceRadius;


    [SerializeField] private Vector2[] fruitPoints;
    //divide our cover distance 


    [Header("explosion to fertize delay and radius size")]

    //public float initialDistance;
    public float durationPerUnit;
    public float radiusPerUnit;
    [SerializeField] private float MaxDistance = 350;

    private float countdownTimer;
    private float currentRadius;
    private Vector3 GizmoSurfacePosition= Vector2.zero;

    [SerializeField] private AnimationCurve radiusCurve;
    [SerializeField] private AnimationCurve TimerCurve;
    private float TimerRadius;
    private float gismosInitRadius;


    private void Start()
    {
       worldSurfaceRadius = GameObject.FindObjectOfType<WheelRegionsManager>().WheelRadius;

       //fruitPositions = new Vector2[6];
        collider = GetComponent<CircleCollider2D>();
        
    }

    private void OnDrawGizmos()
    {
        if (gizmoTreetopTransform == null)
            return;
        Vector2[] gizmosFruits = GetFruitPositions();

        for (int i = 0; i < gizmosFruits.Length; i++)
        {
            Vector2 pos = gizmosFruits[i];
            pos = gizmoTreetopTransform.transform.TransformPoint(pos);
            Gizmos.color = Color.blue;
            // Gizmos.DrawLine(  rb.transform.position,transform.position + (transform.position.normalized  * alteredVelocity));
            GizmosExtensions.DrawWireCircle(pos, gizmoFruitRadius, 36,
                Quaternion.LookRotation(Vector3.up, Vector3.forward));
        }


        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GizmoSurfacePosition, gismosInitRadius);

        Gizmos.color = Color.blue;
       // Gizmos.DrawWireSphere(GizmoSurfacePosition, TimerRadius);
        
    }


    public void ConfigureFruit(ref Vector2[] v2FruitPoints)
    {
        float yDiference = CoverDistance / (v2FruitPoints.Length + 1);

        //Configure positions
        for (int i = 0; i < v2FruitPoints.Length; i++)
            //set the Y positions
            v2FruitPoints[i].y = yDiference * (i + 1) + MiddleOffset;
    }


    public void QuickWidthSet(ref Vector2[] v2FruitPoints)
    {
        for (int i = 0; i < v2FruitPoints.Length; i++)
            //left or right 1 left 2 right
            if ((i + 1) % 2 == 1)
                v2FruitPoints[i].x = -xFruit;
            else
                v2FruitPoints[i].x = xFruit;
    }

    [NaughtyAttributes.Button]
    public void GenerateFruitPoints()
    {
        fruitPoints = new Vector2[fruitCount];
        ConfigureFruit(ref fruitPoints);
        QuickWidthSet(ref fruitPoints);
    }

    public Vector2[] GetFruitPositions()
    {
        if (fruitPoints.Length == 0) GenerateFruitPoints();
        return fruitPoints;
    }

    public GameObject FindFruitPrefab(FruitBase.FruitID id)
    {
        return fruitPrefabs.Find(
            go => go.GetComponent<FruitBase>().fruitID == id);
    }

    public void FertilizeNearby(Vector3 pos)
    {
        StartCoroutine(FertilizerFallToWetTreesCountDown(pos));
    }
    public Vector3 ConvertToSurfacePos(Vector3 pos)
    {
         return  pos.normalized * worldSurfaceRadius;
        //we need to spawn

        //we have a tint layer for trees and regions (regions will have to spawn theirs,)
        ///when explosion happens it simply adds a red value to the layers which is based off distance (also probably increase transparency)
        ///////
        /// explosion happens tells the region , the region has an event that passes out a turn red amount ()objects that can be tinted will subscribe to this event in there on enable/disable
        ///the tint object script just trys to fade out to 0 transparancy which can be added to if explosions go off nearby.
        ///
        /// 
        ///jobs
        ///make the region tinted 
        ///
        ///


    }

    IEnumerator FertilizerFallToWetTreesCountDown(Vector3 initialExplosionPos)
    {
        GizmoSurfacePosition = ConvertToSurfacePos(initialExplosionPos);
        Vector2 surfacePos = ConvertToSurfacePos(initialExplosionPos);
        //get distance to surface of planet
       float initialDistance = Vector2.Distance(initialExplosionPos, surfacePos);
        //configure  timer and radius
        //countdownTimer =  TimerCurve.Evaluate( Mathf.Clamp((initialDistance * durationPerUnit),0,MaxDistance) / MaxDistance);
        //currentRadius = radiusCurve.Evaluate(Mathf.Clamp((initialDistance * radiusPerUnit), 0, MaxDistance) / MaxDistance);
        float countdownTimer = initialDistance * durationPerUnit;
        float currentRadius = initialDistance * radiusPerUnit;
        gismosInitRadius = currentRadius;
        //currentRadius = 2f;
        //Debug.Log("distance: " + initialDistance + " | time: " + countdownTimer);

        while (countdownTimer > 0)
        {
            yield return null;
            countdownTimer -= Time.deltaTime;
            TimerRadius = countdownTimer / durationPerUnit * radiusPerUnit;
        }
        // Timer has expired, trigger event here
        //Debug.Log("Timer expired!");

        Collider2D[] allOverlappingColliders = Physics2D.OverlapCircleAll(ConvertToSurfacePos(initialExplosionPos), currentRadius, 1 << LayerMask.NameToLayer("Wheel"))
           
           .ToArray();
        //Debug.Log("collection of regions " + allOverlappingColliders.Length);
        //Debug.Log("")
        // Debug.Log("count" + allOverlappingColliders.Length);
        int wet = 0;
        foreach (var collider in allOverlappingColliders)
            //find out what id it has and spawn fruit
            if (collider.GetComponentInParent<RegionWetController>())
            {
                collider.GetComponentInParent<RegionWetController>().AddWetness(surfacePos,currentRadius);
                ++wet;
                //collider.GetComponentInParent<RegionWetController>().AddWetness(initialDistance);
            }
        Debug.Log("wet trees :" + wet);
            //collider.GetComponentInParent<ForestController>().SpawnFruit();
            
        // You can add more code here to execute the event
        //Debug.Log("Timer expired!");
    }


    public bool IsValidTree(Collider2D col)
    {
        if (col.gameObject.CompareTag("Tree") && col.GetComponentInParent<Growable>().animGoalIndex > 5)
            return true;
        return false;
    }
    public bool IsRegion(Collider2D col)
    {
        if (GetComponentInParent<Region>())
            return true;
        return false;
    }

    public ObjectPooler GetFruitSpawner(FruitBase.FruitID id)
    {
        switch (id)
        {
            case FruitBase.FruitID.RED:
                return RedFruit;
                
            case FruitBase.FruitID.BLUE:
                return blueFruit;

            case FruitBase.FruitID.YELLOW:
                return YellowFruit;

            case FruitBase.FruitID.SIZE:
            default:
                break;
        }

        return null;
    }

   
}