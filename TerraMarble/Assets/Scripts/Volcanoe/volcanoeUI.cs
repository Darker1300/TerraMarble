using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using System;
public class volcanoeUI : MonoBehaviour
{
    [SerializeField]
    private float[] Height;
    [SerializeField]
    private float[] RockSpawnOffset;

    [SerializeField]
    public List<GameObject> volcanoes = new List<GameObject>();
    [SerializeField]
    private float moveUpAmount;

    [SerializeField]
    private float startVolcanoYPos = -0.35f;
    //Start position 
    private Vector3 startPosition;
    //end position 
    private Vector3 endPosition;

    //move animation curve
    public AnimationCurve moveCurve;
    [SerializeField]
    int HitAmount;
    [SerializeField]
    int VolcanoStage;
    private bool isErupting;

    //start expand to(meaning how much it will expand to , to start with )
    [SerializeField]
    private float DefaultSize;

    [SerializeField]
    private float startExpandVal = 0.2f;
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
    public bool debug;
    public delegate void ExploPercent(int stage, float ExploPercent);
    public event ExploPercent exploEvent;
    public delegate void ExploStartEnd(bool start, int stage, float volcanoExplosionHeightOffset);
    public event ExploStartEnd exploStartEndEvent;
    public GameObject InhaleExhale;
    private Region region = null;
    public ObjectPooler explosion;
    public ObjectPooler explosionSmoke;


    [Header("Space Inbetween FireBalls")]
    [SerializeField]
    private float FireBallDelay = 0.4f;
    [SerializeField]
    private int FireballsPerEruption = 3;
    //
    [SerializeField]
    private ObjectPooler fireBallPool;


    // Start is called before the first frame update
    void Start()
    {
       // fireBallPool = GetComponent<ObjectPooler>();
        //fireBallPool.CreatePool(18);
        //explosion.CreatePool(10);
        //explosionSmoke.CreatePool(10);

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
        if (debug)
        {
            if (Test)
            {
                StartCoroutine(buildingUp());
                Test = false;
            }
        }

    }
    public void Stomp()
    {
        if (HitAmount <= 3 && !isErupting)
        {
            // HitAmount++;
            StartCoroutine(buildingUp());

        }
    }

    public IEnumerator buildingUp()
    {

        float Timer = 0f;
        //break time 
        HitAmount++;
        Circle.enabled = true;
        while (Timer <= duration)
        {

            Timer = Timer + Time.deltaTime;
            float percent = Mathf.Clamp01(Timer / duration);

            Circle.Radius = Mathf.Lerp(DefaultSize, startExpandVal, percent);
            float change = Mathf.Lerp(startVolcanoYPos, Height[VolcanoStage], percent);
            transform.localPosition = new Vector3(0, change, 0);




            yield return null;
        }
        Timer = 0;
        if (HitAmount < 3)
        {


            while (Timer <= duration)
            {

                Timer = Timer + Time.deltaTime;
                float percent = moveCurve.Evaluate(Mathf.Clamp01(Timer / duration));

                Circle.Radius = Mathf.Lerp(startExpandVal, DefaultSize, moveCurve.Evaluate(percent));
                float change = Mathf.Lerp(Height[VolcanoStage], startVolcanoYPos, percent);
                transform.localPosition = new Vector3(0, change, 0);


                yield return null;
            }

        }
        else // explode
        {
            VolcanoStage++;
            Circle.enabled = false;
            //check 

            isErupting = true;

            //fireBallSpawn
            for (int i = 0; i < FireballsPerEruption; i++)
            {
                Explosion(Vector3.up * RockSpawnOffset[VolcanoStage - 1]);
                GameObject fireball = fireBallPool.SpawnFromPool(transform.position + transform.up * RockSpawnOffset[VolcanoStage - 1], null);

                yield return new WaitForSeconds(FireBallDelay);
            }
            Explosion(Vector3.up * RockSpawnOffset[VolcanoStage - 1]);
            exploStartEndEvent?.Invoke(true, VolcanoStage, RockSpawnOffset[VolcanoStage - 1]);
            SetVolcanoe(VolcanoStage);

            while (Timer <= XploDuration)
            {
                Timer = Timer + Time.deltaTime;
                float percent = moveCurve.Evaluate(Mathf.Clamp01(Timer / XploDuration));
                exploEvent?.Invoke(VolcanoStage, percent);

                Debug.Log("fireball delay result " + (Mathf.Abs(percent % FireBallDelay)));





                yield return null;


            }
           
            isErupting = false;
            exploStartEndEvent?.Invoke(false, VolcanoStage, RockSpawnOffset[VolcanoStage - 1]);

            HitAmount = 0;
            InhaleExhale.SetActive(true);
        }


        Circle.Radius = startExpandVal;
        Circle.transform.position = new Vector3(0, startVolcanoYPos, 0);
        //CHECK HERE IF YOU WANT TO DISABLE THIS VOLCANOE OR QUE ITS CHANGE TO rOCK
    }
    public void Explosion(Vector3 height)
    {

        GameObject gg = explosion.SpawnFromPool(transform.position + transform.up * RockSpawnOffset[VolcanoStage - 1], null);
        GameObject smoke = explosionSmoke.SpawnFromPool(transform.position + transform.up * RockSpawnOffset[VolcanoStage - 1], null);

        gg.transform.localScale = new Vector3(2, 2, 2);
        smoke.transform.localScale = new Vector3(2,2,2);
      
        

        //explosion.GetComponent<ParticleSystem>().Play();
    }
    private void SetVolcanoe(int volcanoStage)
    {
        switch (volcanoStage)
        {
            case 1:
                //stage is indexer of height value
                moveUpAmount = Height[volcanoStage - 1];
                //actually 2nd stage
                volcanoes[1].SetActive(true);
                volcanoes[0].SetActive(false);
                break;

            case 2:
                //stage is indexer of height value
                moveUpAmount = Height[volcanoStage - 1];
                //actually 3rd stage
                volcanoes[2].SetActive(true);
                volcanoes[1].SetActive(false);
                break;
            case 3:
                //stage is indexer of height value
                moveUpAmount = Height[volcanoStage - 1];
                //actually 1st stage
                volcanoes[0].SetActive(true);
                volcanoes[2].SetActive(false);
                break;
        }

    }
}
