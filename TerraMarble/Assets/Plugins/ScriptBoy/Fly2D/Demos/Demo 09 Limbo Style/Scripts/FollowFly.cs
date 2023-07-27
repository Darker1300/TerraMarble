using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptBoy.Fly2D;

[DefaultExecutionOrder(10000)]
public class FollowFly : MonoBehaviour
{
    public F2DFlyZone flyZone;
    public int flyIndex;
    public Vector3 offset;

    private void Update()
    {
        transform.position = flyZone.flies[flyIndex].Position + offset;
    }
}