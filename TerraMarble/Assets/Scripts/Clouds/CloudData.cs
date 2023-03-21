using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CloudData : MonoBehaviour
{
    public float timeRemaining = 0f;
    public float lifeTime = 0f;
    public float scale = 1f;
    public float speed = 1f;
    public float height = 1f;
    [HideInInspector] public MeshRenderer meshRenderer;
}
