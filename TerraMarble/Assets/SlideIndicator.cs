using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class SlideIndicator : MonoBehaviour
{

    seadWeirdGravity seedGravity;
    [SerializeField]
    float distanceChangeColor = 18f;
    public Disc uiDisc;
    public Color startColor;
    public Color endColor;


    // Start is called before the first frame update
    void Start()
    {
      seedGravity =  GetComponent<seadWeirdGravity>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log( "mag" + seedGravity.wheelDir.magnitude);
        if (seedGravity.wheelDir.magnitude <= distanceChangeColor)
        {
            uiDisc.Color = endColor;
        }
        else
            uiDisc.Color = startColor;


    }
}
