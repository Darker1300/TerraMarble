using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ScaleToAndBack : MonoBehaviour
{
    [HideLabel]
    [SerializeField]
    [TextArea(1, 2)]
    private string InfoString2 = "";
    public UnityEvent FinishOverallShrink;
    [SerializeField]
    private float duration = 0.25f;
    [SerializeField]
    int maxShrink = 3;
    public bool test;

    //[SerializeField]
    //AnimationCurve scaleCurve;

    [SerializeField]
    int currentShrinkIndex = 3;

    private Vector3 endScale;
    [SerializeField] private Vector3 startScale;
    // Start is called before the first frame update
    void Start()
    {
        endScale = transform.localScale;
         startScale = transform.localScale;
        
        currentShrinkIndex = maxShrink;
        
    }


    // Update is called once per frame
    void Update()
    {

        if (test)
        {
            currentShrinkIndex--;
            if (currentShrinkIndex <= 0)
                currentShrinkIndex = maxShrink;

            ScaleDown(null);
            test = false;
        }
    }


    public void ScaleDown(Collider2D col)

    {
        currentShrinkIndex--;
        if (currentShrinkIndex <= 0)
        {
            currentShrinkIndex = maxShrink;
            FinishOverallShrink?.Invoke();
            


        }
        float step = 1f / maxShrink;
        float st = step * currentShrinkIndex;
        
        endScale = startScale * st;

        this.AnimateComponent<Transform>(duration, (t, time) =>
        {
            t.localScale = Vector3.Lerp(startScale, endScale, time);

        });

    }



}
