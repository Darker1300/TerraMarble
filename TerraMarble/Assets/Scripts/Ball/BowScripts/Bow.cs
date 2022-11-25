using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{

   
    public GameObject Target;
    [SerializeField]
    private AutoAim aim;

    // Start is called before the first frame update
    void Start()
    {
        aim = GetComponent<AutoAim>();   
       

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
