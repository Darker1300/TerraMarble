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

    [SerializeField] private Vector2[] fruitPoints;
    //divide our cover distance 

    
    private void Start()
    {
        ConfigureFruitPools();

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

    public void ConfigureFruitPools()
    {
        blueFruit.CreatePool(20);
        RedFruit.CreatePool(20);
        YellowFruit.CreatePool(20);
    }

    public GameObject FindFruitPrefab(FruitBase.FruitID id)
    {
        return fruitPrefabs.Find(
            go => go.GetComponent<FruitBase>().fruitID == id);
    }

    public void FertilizeNearby(Vector3 pos)
    {
        Collider2D[] allOverlappingColliders = Physics2D.OverlapCircleAll(pos, radius, 1 << 8)
            .Where(col => IsValidTree(col))
            .ToArray();

        foreach (var collider in allOverlappingColliders)
            //find out what id it has and spawn fruit
            collider.GetComponentInParent<ForestController>().SpawnFruit();
    }

    public bool IsValidTree(Collider2D col)
    {
        if (col.gameObject.CompareTag("Tree") && col.GetComponentInParent<Growable>().animGoalIndex > 5)
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