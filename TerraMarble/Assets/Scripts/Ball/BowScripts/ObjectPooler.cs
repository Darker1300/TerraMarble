using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class ObjectPooler : MonoBehaviour
{
    [Header("Object type")]
    public GameObject objectPrefab = null;

    [FormerlySerializedAs("objectAmount")]
    public int defaultCapacity = 0;

    public int maxCapacity = 1000;

    public string poolName = "Pool";
    private Transform _poolTransform = null;
    public bool UsePrefabName = true;

    [Tooltip("DEPRECIATED")] [SerializeField] private bool createOnAwake = false;
    public ObjectPool<GameObject> objectPool;

    public Transform PoolTransform
    {
        get
        {
            if (_poolTransform == null)
                _poolTransform = new GameObject(
                            poolName + (UsePrefabName ? ": " + objectPrefab.name : ""))
                        .transform;
            return _poolTransform;
        }
        set => _poolTransform = value;
    }

    private void Awake()
    {
        objectPool = new ObjectPool<GameObject>(
            OnCreate,
            OnGet,
            OnRelease,
            Destroy,
            true, defaultCapacity, maxCapacity
        );
    }
    private GameObject OnCreate()
    {
        GameObject newGameObj = Instantiate(objectPrefab, PoolTransform, false);
        newGameObj.name = objectPrefab.name + " " + objectPool.CountAll;

        PoolObject poolObj = newGameObj.AddComponent<PoolObject>();
        poolObj.Pool = this;

        newGameObj.SetActive(false);
        return newGameObj;
    }

    private void OnGet(GameObject gObj)
    {
        gObj.SetActive(true);
    }

    private void OnRelease(GameObject gObj)
    {
        gObj.SetActive(false);
    }

    public GameObject SpawnFromPool()
    {
        GameObject newGameObj = objectPool.Get();
        newGameObj.transform.SetParent(PoolTransform, true);
        return newGameObj;
    }

    public GameObject SpawnFromPool(Vector3 position, Transform parent, Quaternion? rotation = null, Vector3? localScale = null)
    {
        GameObject newGameObj = objectPool.Get();
        Transform newTransform = newGameObj.transform;
        Transform prefabTransform = objectPrefab.transform;

        // Parent
        if (parent == null) 
            parent = PoolTransform;

        newTransform.SetParent(parent, false);
        // Position
        newTransform.position = position;
        // Rotation, defaults to local transform in prefab
        if (rotation != null) newTransform.rotation = rotation.Value;
        else newTransform.localRotation = prefabTransform.localRotation;
        // Scale, defaults to local transform in prefab
        if (localScale != null) newTransform.localScale = localScale.Value;
        else newTransform.localScale = prefabTransform.localScale;

        return newGameObj;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.transform.SetParent(PoolTransform, true);
        objectPool.Release(obj);
    }
}