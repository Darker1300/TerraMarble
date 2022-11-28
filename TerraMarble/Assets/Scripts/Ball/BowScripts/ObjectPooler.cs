using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [Header("Object type")]
    [SerializeField] public GameObject objectPrefab = null;
    [SerializeField] int objectAmount = 0;
    Queue<GameObject> activeObjects;
    Queue<GameObject> deactivatedObjects;
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

    public void CreatePool(int objAmount, GameObject objPrefab)
    {
        objectAmount = objAmount;
        objectPrefab = objPrefab;
        activeObjects = new Queue<GameObject>();
        deactivatedObjects = new Queue<GameObject>();

        for (int i = 0; i < objectAmount; i++)
        {
            GameObject temp = Instantiate(objectPrefab);
            temp.SetActive(false);
            deactivatedObjects.Enqueue(temp);
        }
    }


    public void CreatePool(int objAmount)
    {
        objectAmount = objAmount;
        //objectPrefab = objPrefab;
        activeObjects = new Queue<GameObject>();
        deactivatedObjects = new Queue<GameObject>();

        for (int i = 0; i < objectAmount; i++)
        {
            GameObject temp = Instantiate(objectPrefab);
            //temp.transform.SetParent(parent, worldPosStays);
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
