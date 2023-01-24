using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragTreePosition : MonoBehaviour
{
    private TreeBend treeBend;
    // Start is called before the first frame update
    void Start()
    {
        treeBend = GetComponent<TreeBend>();
        InputManager.LeftDragEvent += DragActivated;
        
    }

    public void DragActivated(bool activated)
    {
        if (activated)
        {
            treeBend.enabled= true;
        }
        else
        {
            treeBend.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
