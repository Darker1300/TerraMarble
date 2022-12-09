using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
public class volcanoeUI : MonoBehaviour
{

    [SerializeField]
    private float moveUpAmount;
    //Start position 
    private Vector3 startPosition;
    //end position 
    private Vector3 endPosition;

    //move animation curve
    public AnimationCurve moveCurve;



    //start expand to(meaning how much it will expand to , to start with )
    [SerializeField]
    private float startExpandVal= 0.2f;
    //end size;
    [SerializeField]
    private float endExpandVal = 3;

    //expand animation curve
    public AnimationCurve expandEnd;

    [SerializeField]
    private float duration;

    private Disc Circle;
    public bool Test;

    // Start is called before the first frame update
    void Start()
    {

        startPosition = new Vector3(0, -4, 0);
        Circle = GetComponent<Disc>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Test)
        {
            StartCoroutine(buildingUp());
            Test = false;
        }
    }

    public IEnumerator buildingUp()
    {
        
        float Timer = 0f;
        //break time 
        float start = Circle.Radius;

        while (Timer <= duration)
        {

            Timer = Timer + Time.deltaTime;
            float percent = moveCurve.Evaluate(Mathf.Clamp01(Timer / duration));

            Circle.Radius = Mathf.Lerp(start, startExpandVal,percent);
             float change = Mathf.Lerp(0, 1, percent);
            transform.localPosition = new Vector3(0,  change, 0);


            yield return null;
        }
        Timer = 0;
        while (Timer <= duration)
        {

            Timer = Timer + Time.deltaTime;
            float percent = moveCurve.Evaluate( Mathf.Clamp01(Timer / duration));

            Circle.Radius = Mathf.Lerp(startExpandVal, start, percent);
            float change = Mathf.Lerp(1, 0, percent);
            transform.localPosition = new Vector3(0, change, 0);


            yield return null;
        }

    }

}
