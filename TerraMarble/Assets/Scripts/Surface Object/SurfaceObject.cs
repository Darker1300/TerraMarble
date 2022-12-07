using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SurfaceObject : MonoBehaviour
{
    public bool isDestroyed = false;

    [HideInInspector] public UnityEvent DestroyStart = new();
    [HideInInspector] public UnityEvent DestroyEnd = new();
    
    public void DoDestroy()
    {
        if (isDestroyed) return;

        isDestroyed = true;
        DestroyStart.Invoke();
    }
}
