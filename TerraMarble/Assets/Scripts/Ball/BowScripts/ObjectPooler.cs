using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [Header("Object type")]
    [SerializeField] GameObject objectPrefab = null;
    [SerializeField] int objectAmount = 0;
    public string poolName = "Pool";
    Queue<GameObject> activeObjects;
    Queue<GameObject> deactivatedObjects;
    private Transform _poolTransform = null;
    public bool UsePrefabName = true;

    public Transform PoolTransform
    {
        get
        {
            if (_poolTransform is null)
                _poolTransform
                    = new GameObject(
                            poolName + (UsePrefabName ? (": " + objectPrefab.name) : ""))
                    .transform;
            return _poolTransform;
        }
        set => _poolTransform = value;
    }

    private void Awake()
    {
        //activeObjects = new Queue<GameObject>();
        //deactivatedObjects = new Queue<GameObject>();

        //for (int i = 0; i < objectAmount; i++)
        //{
        //    GameObject temp = Instantiate(objectPrefab);
        //    temp.SetActive(false);
        //    deactivatedObjects.Enqueue(temp);
        //}
    }

    public void CreatePool(int objAmount, GameObject objPrefab = null)
    {
        objectAmount = objAmount;
        if (objPrefab != null) objectPrefab = objPrefab;
        activeObjects = new Queue<GameObject>();
        deactivatedObjects = new Queue<GameObject>();

        for (int i = 0; i < objectAmount; i++)
        {
            GameObject temp = Instantiate(objectPrefab, PoolTransform);
            temp.name = objectPrefab.name + " " + i;
            PoolObject po = temp.AddComponent<PoolObject>(); po.Pool = this;
            temp.SetActive(false);
            deactivatedObjects.Enqueue(temp);
        }
    }

    public GameObject SpawnFromPool(Vector3 position, Transform parent, bool worldPosStays)
    {
        GameObject temp;
        if (deactivatedObjects.Count != 0)
        {
            temp = deactivatedObjects.Dequeue();
        }
        else
        {
            temp = Instantiate(objectPrefab);
            temp.name = objectPrefab.name + " Spawn";
            PoolObject po = temp.AddComponent<PoolObject>(); po.Pool = this;
            objectAmount += 1;
        }

        temp.transform.position = position;
        if (parent != null)
        {
            temp.transform.SetParent(parent, worldPosStays);

        }
        temp.SetActive(true);
        activeObjects.Enqueue(temp);
        return temp;
    }

    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(null, true);
        deactivatedObjects.Enqueue(obj);
    }


}
