using System.Collections;
using System.Collections.Generic;
using Shapes;
using UnityEngine;

public class Region : MonoBehaviour
{
    private Transform _anchor = null;
    public Wheel Wheel = null;
    public Disc Disc = null;

    public Transform Anchor => TryCreateAnchor();

    public Vector3 EndPosition => Anchor.position;

    private Transform TryCreateAnchor()
    {
        if (_anchor == null)
        {
            _anchor = new GameObject("Anchor").transform;
            _anchor.SetParent(transform, false);
            // _anchor.position
        }

        return _anchor;
    }
}