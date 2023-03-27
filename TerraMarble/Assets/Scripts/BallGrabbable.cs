using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BallGrabbable : MonoBehaviour
{
    [HideInInspector] public UnityEvent GrabStart;
    [HideInInspector] public UnityEvent GrabEnd;
}
