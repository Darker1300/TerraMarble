using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Shapes;

public class FlyUI : MonoBehaviour
{

    private Rigidbody2D rb;

    [Header("Flying How much the ball UI shrinks ")]

    [SerializeField]
    [Range(0, 5)]
    private float IncreaseSize = 0.5f;
    private float startDiscSize;
    public AnimationCurve DotUiScaleInfluence;
    
    [SerializeField]
    private float maxVelocity = 8;

    private Disc uiShape;

    private Vector2 uiVelocity;
    [SerializeField]
    private float smoothAimTime = 0.2f;




    // Start is called before the first frame update
    void Start()
    {
       rb = GetComponentInParent<Rigidbody2D>();
        
        uiShape = GetComponent<Disc>();
        startDiscSize = uiShape.Radius;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SetUI(bool isActive)
    {
        uiShape.gameObject.SetActive(isActive);
    }

    public void UpdateUI(float percBallWind)
    {
        //if (ballWindJump.upDragInput !=0)
        //{
            //0-1 factor

            uiShape.Radius = Mathf.Lerp(startDiscSize, startDiscSize+(startDiscSize * IncreaseSize),percBallWind);

        //}
        transform.right = Vector2.SmoothDamp(transform.right, rb.velocity.normalized,ref uiVelocity,smoothAimTime);

    }

    private void ResetUI()
    {
        
    }
}
