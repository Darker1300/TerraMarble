using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormMain : MonoBehaviour
{
    [SerializeField ]
    private TenticleRetainSize tenti;
    [SerializeField]
    private float startSize = 0.02f;
    [SerializeField]
    private float endSize = 0.18f;
    private float currentSize;


    // Start is called before the first frame update
    void Start()
    {
       //tenti = GetComponent<TenticleRetainSize>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void WormAte(float val)
    {
        currentSize += Mathf.Clamp01( val); 
        tenti.GrowSize = Mathf.Lerp(startSize,endSize, currentSize);
    

    }
}
