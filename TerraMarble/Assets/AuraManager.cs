using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class AuraManager : MonoBehaviour
{
    public Disc Aura;
    public Disc pulseRing;
    // Start is called before the first frame update
  

    void Start()
    {
        Aura = GetComponent<Disc>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
