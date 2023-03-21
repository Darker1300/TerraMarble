using System.Linq;
using UnityEngine;

public class ForestController : MonoBehaviour
{
    private SurfaceObject surfaceObject = null;
    private Region region = null;
    private Growable growable = null;
    public Collider2D treeCollider;

    private FruitManager fruitManager;
    //public List<Transform>()

    private FruitBase[] fruits;
    public FruitBase.FruitID fruitID;
    

    #region Events

    private void Start()
    {
        fruitID = (FruitBase.FruitID) Random.Range(0, (int) FruitBase.FruitID.SIZE);

        fruitManager = FindObjectOfType<FruitManager>();
        fruits = new FruitBase[fruitManager.fruitCount];

        treeCollider = GetComponentInChildren<Collider2D>();
        surfaceObject = GetComponent<SurfaceObject>();
        region = GetComponentInParent<Region>();
        growable = GetComponent<Growable>();

        surfaceObject.DestroyStart.AddListener(OnDestroyStart);
        region.BallHitEnter.AddListener(OnBallHitEnter);
    }

    private void Update()
    {
        if (!Application.isPlaying) return;

        if (surfaceObject.isDestroyed
            & growable.IsInEmptyState())
        {
            surfaceObject.DestroyEnd.Invoke();
            region.surfaceObjects.Remove(surfaceObject);
            Destroy(gameObject);
        }
    }

    public void SpawnFruit()
    {
        Vector2[] fruitPoints = fruitManager.GetFruitPositions();
        for (int i = 0; i < fruitPoints.Length; i++) SpawnFruit(i, fruitPoints[i]);
    }

    public void SpawnFruit(int index, Vector3 fruitPos)
    {
        if (fruits[index] != null) return;
        ObjectPooler fruitPooler = fruitManager.GetFruitSpawner(fruitID);

        if (fruitPooler == null)
            return;

        fruitPos.z = -0.1f;

        GameObject prefabFruit = fruitPooler.objectPrefab;
        GameObject newFruit = fruitPooler.SpawnFromPool();
        
        // Set tranform
        Vector3 localScale = prefabFruit.transform.localScale;
        newFruit.transform.SetParent(treeCollider.transform, false);
        newFruit.transform.localPosition = fruitPos;
        newFruit.transform.localRotation = Quaternion.identity;
        newFruit.transform.localScale = localScale;

        // Fruit info
        FruitBase fruitBase = newFruit.GetComponent<FruitBase>();
        fruitBase.treeIndex = index;
        fruitBase.forestController = this;

        fruits[index] = fruitBase;
    }

    public void PopFruit(int index)
    {
        if (index >= fruits.Length || index < 0) return;
        FruitBase fruit = fruits[index];
        if (fruit == null) return;

        fruit.GetComponent<PoolObject>()?.Pool?.ReturnToPool(fruit.gameObject);
    }


    private void OnBallHitEnter(Region.RegionHitInfo info)
    {
        if (info.ballState is null) return;

        if (info.ballState.Stomp)
            OnBallStompEnter(info);

        //if (fruits.Any(fruit => fruit != null))
        //{ // There is Fruits
        //    for (int i = 0; i < fruits.Length; i++)
        //    {
        //        FruitBase fruit = fruits[i];
        //        if (fruit == null) continue;
                
        //        // Collect Fruit
        //        fruit.GetComponent<PoolObject>().Pool.ReturnToPool(fruit.gameObject);
        //    }
        //}
    }

    private void OnBallStompEnter(Region.RegionHitInfo info)
    {
        if (info.surfaceObj != null)
            // hit tree
            ShrinkDestroy();
    }

    #endregion

    public void TryGrow()
    {
        if (surfaceObject.isDestroyed) return;
        growable.TryGrowState();
    }

    public void TryShrink()
    {
        if (surfaceObject.isDestroyed) return;
        growable.TryShrinkState();
    }

    public void Reset()
    {
        if (surfaceObject.isDestroyed) return;
        growable.ResetState();
    }

    public void ShrinkDestroy()
    {
        bool success = growable.TryShrinkState();
        if (!success && !surfaceObject.isDestroyed)
            surfaceObject.DoDestroy();
    }

    private void OnDestroyStart()
    {
        growable.ResetState();
    }
}