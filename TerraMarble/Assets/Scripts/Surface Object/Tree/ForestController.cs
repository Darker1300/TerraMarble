using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

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

    void Start()
    {
        fruitID = (FruitBase.FruitID)Random.Range(0, (int)(FruitBase.FruitID.SIZE));

        fruitManager = FindObjectOfType<FruitManager>();
        fruits = new FruitBase[fruitManager.fruitCount];

        treeCollider = GetComponentInChildren<Collider2D>();
        surfaceObject = GetComponent<SurfaceObject>();
        region = GetComponentInParent<Region>();
        growable = GetComponent<Growable>();

        surfaceObject.DestroyStart.AddListener(OnDestroyStart);
        region.BallHitEnter.AddListener(OnBallHitEnter);
    }

    void Update()
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

    public void SpawnFruit(int index, Vector3 fruitPos)
    {
        Debug.Log(index);
        if (fruits[index] != null)
        {
            return;
        }
        ObjectPooler fruitPooler = fruitManager.GetFruitSpawner(fruitID);

        if (fruitPooler == null)
            return;
        //foreach (var item in fruitManager.fruitPositions)
        //{
        //    Debug.Log(item);
        //}
       
        fruitPos.z = -0.1f;
        GameObject newFruit = fruitPooler.SpawnFromPool();
        Vector3 localScale = newFruit.transform.localScale;
        newFruit.transform.SetParent(treeCollider.transform, false);
        newFruit.transform.localPosition = fruitPos;
        newFruit.transform.localRotation = Quaternion.identity;
        newFruit.transform.localScale = localScale;
        fruits[index] = newFruit.GetComponent<FruitBase>();

    }
    public void SpawnFruit()
    {
        Vector2[] fruitPoints = fruitManager.GetFruitPositions();
        for (int i = 0; i < fruitPoints.Length; i++)
        {
            SpawnFruit(i, fruitPoints[i]);
        }
    }
   // [NaughtyAttributes.Button]

    //public void TestSpawnFruit()
    //{
    //    Vector2[] fruitPoints = fruitManager.GetFruitPositions();
    //    SpawnFruit(Random.Range(0, fruits.Length - 1), fruitPoints[Random.Range(0, fruitPoints.Length - 1)]);
    //}
    void OnBallHitEnter(Region.RegionHitInfo info)
    {
        if (info.ballState is not null && info.ballState.Stomp)
            OnBallStompEnter(info);
    }

    void OnBallStompEnter(Region.RegionHitInfo info)
    {
        if (info.surfaceObj != null)
        {   // hit tree
            ShrinkDestroy();
        }
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

    void OnDestroyStart()
    {
        growable.ResetState();
    }
}
