using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;
using UnityEngine.Events;

public class DiscSizBounce : MonoBehaviour
{
    public Disc m_Disc = null;

    public UnityEvent<float> Lerping;

    public UnityEvent<bool> OnStartBounce;

    public float tValue = 0.0f;
    public bool lerping = false;
    public float MaxSize;
    public bool shrinkLerping = false;



    public float currentRadius;
    public float startRadius;


    [Header("Overshoot Variables")]
    public float _OverShootAmount;
    public float _DesiredAmount;
    public float _NormalDuration;

    public float _BounceBackDur;
    public float _OveralDuration;
    public float time;
    //0 is nothing || 1 lerpto overshoot ||  2 lerp back 
    public int State = 0;


    CircleCollider2D[] circleColiders;
    private bool Growing = true;

    private void Start()
    {
        circleColiders = transform.GetComponents<CircleCollider2D>();
        startRadius = m_Disc.Radius;
        currentRadius = startRadius;
        Debug.Log("starttyStart");
        OnStartBounce?.Invoke(true);
        _OveralDuration = calculateDuration();

    }

    private void Update()
    {








        if (lerping && Growing)
        {

            LerpToSize(_DesiredAmount * 1.2f, +_OverShootAmount, _NormalDuration, _BounceBackDur);



            State = 1;

            lerping = false;
        }






        if (shrinkLerping && !Growing)
        {

            LerpToSize(_DesiredAmount * 0.8f, -_OverShootAmount, _NormalDuration, _BounceBackDur);


            State = 1;

            shrinkLerping = false;
        }





        if (State != 0)
        {
            Lerping?.Invoke(_OveralDuration / time);

            if (State == 1)
            {

                if (LerpCircle(_DesiredAmount + _OverShootAmount, _NormalDuration))
                {

                    State = 2;
                    //Lerping.Invoke(tValue);
                    //
                    startRadius = m_Disc.Radius;
                    // Debug.Log("done");
                }
                //overshootamount,desiredAmount,toovershootduration,fromOverShootDuration
                //overshootAmount = desiredAmount + overshootAmount
                //lerp once to overshoot by Overshootdur
                //lerp 2nd time to desiredAmount
                // Lerping.Invoke(tValue);
            }
            else if (State == 2)
            {
                //Debug.Log("second");
                if (LerpCircle(_DesiredAmount, _BounceBackDur))
                {

                    State = 0;
                    OnStartBounce?.Invoke(true);
                    CheckToSwitch();
                    time = 0f;
                }
            }

        }
    }

    public void LerpToSize(float DesiredRadius, float overShoot, float normalSizeDuration, float popBackDur)
    {
        //set desired amount
        _DesiredAmount = DesiredRadius;
        //set overshoot amount
        _OverShootAmount = overShoot;
        //set start Radius
        startRadius = m_Disc.Radius;
        //set current Radius
        //currentRadius = startRadius;
        //set to size duration
        _NormalDuration = normalSizeDuration;
        //set the bounce back duration
        _BounceBackDur = popBackDur;

        //START THE LERP
        State = 1;






    }
    public void CheckToSwitch()
    {
        if (_DesiredAmount < MaxSize)
        {
            OnStartBounce?.Invoke(true);

            //OnStartBounce?.Invoke(true);
            Growing = true;
            lerping = true;
        }
        else if (_DesiredAmount > 0)
        {
            Growing = false;
            shrinkLerping = true;

        }
    }
    public bool LerpCircle(float sizeto, float duration)
    {


        time += Time.deltaTime;
        if (tValue < duration)
        {

            Lerping?.Invoke(tValue);


            //increment timer once per frame
            tValue += Time.deltaTime;
            if (tValue > duration)
            {
                tValue = duration;
            }

            _OveralDuration -= tValue;
            float t = tValue / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);


            m_Disc.Radius = Mathf.Lerp(startRadius, sizeto, t);
            circleColiders[1].radius = m_Disc.Radius;
            circleColiders[0].radius = m_Disc.Radius * 3;


            return false;
        }
        else
        {
            m_Disc.Radius = sizeto;
            State = 2;
            tValue = 0.0f;
            m_Disc.Radius = sizeto;
            return true;
        }

    }
    public float calculateDuration()
    {

        return  _NormalDuration + _BounceBackDur;
    }

    ///size in inspector 
}
