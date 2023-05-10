using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class AuraManager : MonoBehaviour
{
    public Disc Aura;
    public Disc pulseRing;
    public ChangeColorOverTime colorChange;
    public ChangeColorOverTime colorChangeFadein;
    public AuraScaler auraScaler;
    // Start is called before the first frame update
  

    void Start()
    {
        Aura = GetComponent<Disc>();
    }
    public void ShrinkAndFade()
    {
        Debug.Log("called Once");
        auraScaler.AuraToggle(false);
        auraScaler.ShrinkBackToNormal();
        colorChange.MakeTransparent();
        //Aura.Radius = 0;
        
    }
    public void FadeIn()
    {
       // Debug.Log("called Once");
        colorChange.MakeOpaque();
        //auraScaler.AuraToggle(true);



    }
    public void DisableAuraControl()
    {
        auraScaler.AuraToggle(false);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
