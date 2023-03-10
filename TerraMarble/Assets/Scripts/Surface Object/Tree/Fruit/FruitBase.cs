using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;


public class FruitBase : MonoBehaviour
{

    public ForestController forestController;
    public GameObject renderObj;
    public enum FruitID
    {
        RED,BLUE,YELLOW,SIZE
    }

    public FruitID fruitID;

    public int treeIndex = -1;

    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (treeIndex < 0) return;
        var ballState = collider2D.gameObject.GetComponent<BallStateTracker>();

        if (ballState != null)
            forestController.PopFruit(treeIndex);
    }




}
