using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;


public class ChangePinColer : MonoBehaviour
{
    [SerializeField]

    private Color startColor;
    [SerializeField]
    private Color endcolor;
    private Disc thisComponent;
    public bool running;
    [SerializeField]
    private float duration = 0.25f;
    [SerializeField]
    private float coolDownDuration = 0.25f;

    [SerializeField]
    private float endScale;
    [SerializeField]
    private float pinRegenTime = 1;




    [SerializeField]
    private bool shouldScale;

    [SerializeField]
    AnimationCurve scaleCurve;

    // Start is called before the first frame update
    void Start()
    {
        thisComponent = GetComponent<Disc>();
        startColor = thisComponent.Color;
    }
    IEnumerator ColorChange()
    {
        running = true;
        GetComponent<CircleCollider2D>().enabled = false;
        float startScale = thisComponent.Radius;
        float Timer = 0f;
        while (Timer <= duration)
        {

            Timer = Timer + Time.deltaTime;
            float percent = Mathf.Clamp01(Timer / duration);
            thisComponent.Color = Color.Lerp(startColor, endcolor, percent);
            thisComponent.Radius = Mathf.Lerp(startScale, endScale, percent);


            yield return null;
        }


        Timer = 0f;
        while (Timer <= coolDownDuration)
        {
            Timer = Timer + Time.deltaTime;
            float percent = Mathf.Clamp01(Timer / coolDownDuration);
            thisComponent.Color = Color.Lerp(endcolor, startColor, percent);
            thisComponent.Radius = Mathf.Lerp(endScale, startScale, percent);
            yield return null;
        }
        thisComponent.enabled = false;
        
        yield return new WaitForSeconds(pinRegenTime);
        running = false;
        GetComponent<CircleCollider2D>().enabled = true;
        thisComponent.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Worm"))
        {


            if (!running)
            {

                StartCoroutine(ColorChange());



            }
            collision.gameObject.GetComponent<WormMain>().WormAte(0.01f);
        }
    }
}
