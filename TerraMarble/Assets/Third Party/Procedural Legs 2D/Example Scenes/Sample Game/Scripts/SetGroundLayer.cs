using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGroundLayer : MonoBehaviour
{
    void OnValidate()
    {
        SetLayer();
    }

    void Start()
    {
        SetLayer();
    }

    void SetLayer()
    {
        gameObject.layer = LayerMask.NameToLayer("Ground");
    }
}
