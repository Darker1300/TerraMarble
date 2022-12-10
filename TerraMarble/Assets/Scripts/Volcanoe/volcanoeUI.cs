using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using System;
public class volcanoeUI : MonoBehaviour
{
    [SerializeField]
    private float [] Height;
    [SerializeField]
    public List<GameObject> volcanoes = new List<GameObject>(); 
    [SerializeField]
    private float moveUpAmount;
    //Start position 
    private Vector3 startPosition;
    //end position 
    private Vector3 endPosition;

    //move animation curve
    public AnimationCurve moveCurve;
    [SerializeField]
    float HitAmount;
    [SerializeField]
    int VolcanoStage;

    //start expand to(meaning how much it will expand to , to start with )
    [SerializeField]
    private float DefaultSize;

    [SerializeField]
    private float startExpandVal= 0.2f;
    //end size;
    [SerializeField]
    private float endExpandVal = 3;

    //expand animation curve
    public AnimationCurve expandEnd;

    [SerializeField]
    private float duration;
    [SerializeField]
    private float XploDuration;

    private Disc Circle;
    public bool Test;
   
    public delegate void ExploPercent(int stage,float ExploPercent);
    public event ExploPercent exploEvent;
    public delegate void ExploStartEnd(bool start,int stage);
    public event ExploStartEnd exploStartEndEvent;
    public GameObject InhaleExhale;
    private Region region = null;

    // Start is called before the first frame update
    void Start()
    {

        startPosition = new Vector3(0, -4, 0);
        Circle = GetComponent<Disc>();


        region = GetComponentInParent<Region>();
        region.BallHitEnter.AddListener(OnBallHitEnter);

    }
    void OnBallHitEnter(Region.RegionHitInfo info)
    {
        if (info.ballState.Stomp)
            OnBallStompEnter(info);
    }

    void OnBallStompEnter(Region.RegionHitInfo info)
    {
        if (info.surfaceObj != null)
        {
            Stomp();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void Stomp()
    {
        if ( HitAmount < 4)
        {
            HitAmount++;
            StartCoroutine(buildingUp());
           
        }
    }
    public IEnumerator buildingUp()
    {
        
        float Timer = 0f;
        //break time 

        Circle.enabled = true;
        while (Timer <= duration)
        {

            Timer = Timer + Time.deltaTime;
            float percent = Mathf.Clamp01(Timer / duration);

            Circle.Radius = Mathf.Lerp(DefaultSize, startExpandVal,percent);
             float change = Mathf.Lerp(0, 1, percent);
            transform.localPosition = new Vector3(0,  change, 0);


            yield return null;
        }
        Timer = 0;
        if (HitAmount != 3)
        {


            while (Timer <= duration)
            {

                Timer = Timer + Time.deltaTime;
                float percent = moveCurve.Evaluate(Mathf.Clamp01(Timer / duration));

                Circle.Radius = Mathf.Lerp(startExpandVal, DefaultSize, moveCurve.Evaluate(percent));
                float change = Mathf.Lerp(1, 0, percent);
                transform.localPosition = new Vector3(0, change, 0);


                yield return null;
            }
        }
        else // explode
        {
            VolcanoStage++;
            Circle.enabled = false;
            //check 
            SetVolcanoe(VolcanoStage );
            exploStartEndEvent?.Invoke(true,VolcanoStage);
            while (Timer <= XploDuration)
            {
                Timer = Timer + Time.deltaTime;
                float percent = moveCurve.Evaluate(Mathf.Clamp01(Timer / XploDuration));
                exploEvent?.Invoke(VolcanoStage, percent);
                yield return null;


            }
            exploStartEndEvent?.Invoke(false, VolcanoStage);


            HitAmount = 0;
        }
        InhaleExhale.SetActive(true);
        Circle.Radius = startExpandVal;
        //CHECK HERE IF YOU WANT TO DISABLE THIS VOLCANOE OR QUE ITS CHANGE TO rOCK
    }

    private void SetVolcanoe(int volcanoStage)
    {
        switch (volcanoStage)
        {
            case 1:
                //stage is indexer of height value
                moveUpAmount = Height[volcanoStage-1];
                //actually 2nd stage
                volcanoes[1].SetActive(true);
                volcanoes[0].SetActive(false);
                break;

            case 2:
                //stage is indexer of height value
                moveUpAmount = Height[volcanoStage-1];
                //actually 3rd stage
                volcanoes[2].SetActive(true);
                volcanoes[1].SetActive(false);
                break;
            case 3:
                //stage is indexer of height value
                moveUpAmount = Height[volcanoStage -1 ];
                //actually 1st stage
                volcanoes[0].SetActive(true);
                volcanoes[2].SetActive(false);
                break;
        }
        
    }
}
