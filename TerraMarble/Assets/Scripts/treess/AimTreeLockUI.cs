using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class AimTreeLockUI : MonoBehaviour
{

    public Disc fillDisc;
    private float startRad;
    private float endRad;
    //private float DiffRad;
   [SerializeField]
   private float radius;
    [SerializeField]
    [Range(1,3)]
    private float startBarSize;

    // Start is called before the first frame update
    void Start()
    {
        //fillDisc = GetComponent<Disc>();
        fillDisc.AngRadiansStart = radius;
        fillDisc.AngRadiansEnd = -radius;

        startRad = radius;
       
        
        InputManager.LeftDragVectorEvent += LookRotation;
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    public void BarIncrease(float percent)
    {
        //float percent = Mathf.Clamp01((target.transform.position - transform.position).magnitude / coliderRadius));

    }

    public void LookRotation(Vector2 direction,Vector2 delta)
    {
        
        //transform.LookAt(target,Vector3.up);
        direction = direction.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //angle += offset;
        
        //float dot = Vector2.Dot(-transform.parent.up, Direction);

        //angle = Vector2.Dot(,)
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = rotation;
    }
}
