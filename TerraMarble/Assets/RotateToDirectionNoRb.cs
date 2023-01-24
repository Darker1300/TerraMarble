using System.Collections;
using UnityEngine;



public class RotateToDirectionNoRb : MonoBehaviour
{

    public float rotationSpeed = 25;
    public float moveSpeed = 25;
    public Vector2 direction;
    private Quaternion starting;
    //public float PaddleBackSpeed;

    //Coroutine stuff
    [SerializeField]
    private float duration;

    // Start is called before the first frame update
    void Start()
    {
        starting = transform.rotation;
        InputManager.LeftDragEvent += RotateBack;
    }

    // Update is called once per frame
    void Update()
    {
      
        

        // transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
    public void RotateToThis(Vector2 newDirection,float percentage,Vector3 pos)
    {
        var relativePoint = transform.InverseTransformPoint(pos);
        if (relativePoint.x <= 0.0)
        {
            newDirection.y = -1 ;
            Debug.Log("R");
            //Mathf.Abs (newDirection.x) ;
        }
        else if (relativePoint.x > 0.0)
        {
            Debug.Log("L");
            newDirection.y = 1;


        }

        float angle = Mathf.Atan2(newDirection.y, newDirection.x) * Mathf.Rad2Deg;
        //angle += offset;
        angle *= (1-percentage) ;

        //float dot = Vector2.Dot(-transform.parent.up, Direction);
       


        //angle = Vector2.Dot(,)
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward) * starting;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

    }
    public void RotateBack(bool keyDown)
    {
        if (!keyDown)
        { 
        StartCoroutine(ReturnToDefaultAngle());
        }

    }
    IEnumerator ReturnToDefaultAngle()
    {
        float Timer= 0;



        while (Timer <= duration )
        {

            Timer = Timer + Time.deltaTime;
            float percent = Mathf.Clamp01(Timer / duration);

            float angle = Mathf.Atan2(0, 1) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward) * starting;
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, percent);




            yield return null;
        }

    }
    }
