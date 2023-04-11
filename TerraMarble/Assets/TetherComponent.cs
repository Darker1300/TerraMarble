using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetherComponent : MonoBehaviour
{
    private TenticleRetainSize tenticleScr;
    public GameObject tetheredObj;

    public LineRenderer tetherLine;

    public void AttachObjectToTether(GameObject ObjToTethered)
    {
        tetheredObj = ObjToTethered;
        tenticleScr.followObject = ObjToTethered.transform;
        //turn on tether
        tenticleScr.enabled = true;
        tetherLine.enabled = true;
        tetheredObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
        
    }

    public void DetachObjectToTether()
    {
        tetheredObj.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        tetheredObj.GetComponent<Rigidbody2D>().AddForce(tenticleScr.GetPullDirection()*20f);
        tetheredObj = null;
        tenticleScr.followObject = null;
        //turn on tether
        tenticleScr.enabled = false;
        tetherLine.enabled = false;
       
        
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
