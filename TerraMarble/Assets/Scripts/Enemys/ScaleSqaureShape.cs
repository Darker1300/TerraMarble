using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class ScaleSqaureShape : MonoBehaviour
{
    [SerializeField]

    
    private Rectangle thisComponent;
    public bool running;
    [SerializeField]
    private float duration = 0.25f;
    [SerializeField]
    private float coolDownDuration = 0.25f;

    [SerializeField]
    private float endScale;
    [SerializeField]
    private float pinRegenTime = 1;

    public bool test;


    [SerializeField]
    private bool shouldScale;

    [SerializeField]
    AnimationCurve scaleCurve;

    // Start is called before the first frame update
    void Start()
    {
        thisComponent = GetComponent<Rectangle>();
    }
        
    IEnumerator ScaleChange()
    {
        running = true;
        
        float startScale = thisComponent.Width;
        float Timer = 0f;
        while (Timer <= duration)
        {

            Timer = Timer + Time.deltaTime;
            float percent = Mathf.Clamp01(Timer / duration);
            //thisComponent.Color = Color.Lerp(startColor, endcolor, percent);
            float scale = Mathf.Lerp(startScale, endScale, percent);
            thisComponent.Width = scale;
            thisComponent.Height = scale;


            yield return null;
        }


        Timer = 0f;
        while (Timer <= coolDownDuration)
        {
            Timer = Timer + Time.deltaTime;
            float percent = Mathf.Clamp01(Timer / coolDownDuration);
            //thisComponent.Color = Color.Lerp(endcolor, startColor, percent);
            float scale = Mathf.Lerp(startScale, endScale, percent);
            thisComponent.Width = scale;
            thisComponent.Height = scale;
            yield return null;
        }
        thisComponent.enabled = false;

        yield return new WaitForSeconds(pinRegenTime);
        running = false;
        //GetComponent<CircleCollider2D>().enabled = true;
        //thisComponent.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
            if (test)
            {
                StartCoroutine("ScaleChange");
                test = false;
            }
    }
   






}
