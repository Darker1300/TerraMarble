using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherComponent : MonoBehaviour
{
    private TenticleRetainSize tenticleScr;
    public GameObject tetheredObj;
    

    public void AttachObjectToTether(GameObject ObjToTethered)
    {
        tetheredObj = ObjToTethered;
        tenticleScr.followObject = ObjToTethered.transform;
        //turn on tether
        tenticleScr.enabled = true;
    }

    public void DetachObjectToTether()
    {
        tetheredObj = null;
        tenticleScr.followObject = null;
        //turn on tether
        tenticleScr.enabled = false;
    }


    // Start is called before the first frame update
    void Start()
    {
        tenticleScr = GetComponent<TenticleRetainSize>();


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
