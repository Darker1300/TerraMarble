using System.Collections;
using System.Collections.Generic;
using MathUtility;
using UnityEngine;
using Shapes;

public class AimTreeLockUI : MonoBehaviour
{

    private DragTreePosition treeActive;

    public Disc fillDisc;
    public Disc outerDisc;
    private float startRad;
    private float endSize;
    //private float DiffRad;
   [SerializeField]
   private float radius;
    [SerializeField]
    [Range(1,3)]
    private float startBarSize;

    // Start is called before the first frame update
    void Start()
    {
        treeActive = FindObjectOfType<DragTreePosition>();

        //fillDisc = GetComponent<Disc>();
        fillDisc.AngRadiansStart = radius;
        fillDisc.AngRadiansEnd = -radius;

        //startRad = radius;
        //outerDisc = GetComponentInChildren<Disc>();
        endSize = outerDisc.AngRadiansStart;

        InputManager.LeftDragEvent += EnableDiscs;
    }

    // Update is called once per frame
    void Update()
    {
        if (treeActive.enabled)
        {
            Vector2 dir = ((Vector2)transform.Towards(treeActive.transform)).normalized;
            float ang = MathU.Vector2ToDegree(dir);
            transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

            BarIncrease(treeActive.treeBender.dragInput.y);
        }
    }

    public void EnableDiscs(bool inputDown)
    {
        if (inputDown)
        {
            fillDisc.enabled = true;
            outerDisc.enabled = true;

        }
        else
        {
            fillDisc.enabled = false;
            outerDisc.enabled = false;
        }

    }
    public void BarIncrease(float percent)
    {
        fillDisc.AngRadiansStart = Mathf.Lerp(0, endSize, percent);
        fillDisc.AngRadiansEnd = -Mathf.Lerp(0, endSize, percent);
        //float percent = Mathf.Clamp01((target.transform.position - transform.position).magnitude / coliderRadius));

    }

    //public void LookRotation(Vector2 direction,Vector2 delta)
    //{
        
    //    //transform.LookAt(target,Vector3.up);
    //    direction = -direction.normalized;

    //    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    //    //angle += offset;
        
    //    //float dot = Vector2.Dot(-transform.parent.up, Direction);

    //    //angle = Vector2.Dot(,)
    //    Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    //    transform.rotation = rotation;
    //}
}
