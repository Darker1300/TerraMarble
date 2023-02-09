using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragTreePosition : MonoBehaviour
{
    public TreeBend treeBender;

    void Start()
    {
        treeBender = GetComponent<TreeBend>();
        InputManager.LeftDragEvent += DragActivated;
        InputManager.RightDragEvent += DragActivated;
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
