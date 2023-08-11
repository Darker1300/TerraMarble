using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Bow : MonoBehaviour
{

    WaitForSeconds Firedelay = new WaitForSeconds(.5f);
    Coroutine coroutine;
    [SerializeField] TextMeshProUGUI AmmoText;

    public GameObject Target;
    [SerializeField]
    private AutoAim aim;
    private AmmoController ammoController;
    public int AmmoAmount;
    public bool testShoot = false;

    public float fireRate = 0.5F;
    private float nextFire = 0.0F;
    private Transform WheelTransform;
    private float wheelRadius;
    
    // Start is called before the first frame update
    void Start()
    {
        ammoController = GetComponent<AmmoController>();
        aim = GetComponent<AutoAim>();
        AmmoText = AmmoText != null ? AmmoText
            : GameObject.Find("AmmoText")?.GetComponent<TextMeshProUGUI>();
        
        UpdateAmmo();
        WheelTransform = GameObject.FindGameObjectWithTag("Wheel").transform;
        wheelRadius = WheelTransform.GetComponent<CircleCollider2D>().radius;
    }

    // Update is called once per frame
    void Update()
    {
        //if has ammo
        if (AmmoAmount != 0 && Time.time > nextFire && AmmoAmount > 0)
        {
            Target = null;
            Target = aim.FindClosestTarget();
            if (Target != null && Vector2.Distance(WheelTransform.position, transform.position) > wheelRadius +8)
            {
                StartCoroutine("FireRate");
                testShoot = false;
                nextFire = Time.time + fireRate;
            }

        }
        //if has target

        //shoot
    }

    public void UpdateAmmo()
    {
        if (AmmoText)
            AmmoText.text = "Ammo: " + AmmoAmount;
    }

    IEnumerator FireRate()
    {
        int i = 2;

        while (i > 0)
        {
            if (AmmoAmount > 0)
            {
                ammoController.GetProjectile(BallStateTracker.BallState.NoEffector, Target.transform.position);
                ammoController.currentProjectile.SetActive(true);
                AmmoAmount--;
                UpdateAmmo();
            }
            else yield break;

            // Do something 4 times
            i--;
            yield return Firedelay;
        }
    }

}
