using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTreePosition : MonoBehaviour
{
    private TreeBend treeBender;

    void Start()
    {
        treeBender = GetComponent<TreeBend>();
        InputManager.LeftDragEvent += DragActivated;
    }

    public void DragActivated(bool activated)
    {
        if (activated)
        {
            treeBender.enabled= true;
        }
        else
        {
            treeBender.enabled = false;
        }
    }
}
