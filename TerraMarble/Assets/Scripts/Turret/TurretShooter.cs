using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Shapes;
public class TurretShooter : MonoBehaviour
{
    WaitForSeconds Firedelay = new WaitForSeconds(.25f);
    Coroutine coroutine;

    public Line AmmoLineIndictor;
    private float DefaultAmmoLineDist = 6;
    public GameObject Target;
    [SerializeField]
    private AutoAim aim;
    private AmmoController ammoController;
    public int MaxAmmo;
    public int AmmoAmount;
    public bool testShoot = false;

    public float fireRate = 0.5F;
    private float nextFire = 0.0F;
    private Transform WheelTransform;
    private float wheelRadius;
    public bool StartWithAmmo = true;
    

    // Start is called before the first frame update
    void Start()
    {
        ammoController = GetComponent<AmmoController>();
        aim = GetComponent<AutoAim>();
        
        //UpdateAmmo();
        WheelTransform = GameObject.FindGameObjectWithTag("Wheel").transform;
        wheelRadius = WheelTransform.GetComponent<CircleCollider2D>().radius;
    }
    private void OnEnable()
    {
        if (StartWithAmmo)
        {
            AmmoAmount = MaxAmmo;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //if has ammo
        if (AmmoAmount != 0 && Time.time > nextFire && AmmoAmount > 0)
        {

            AmmoLineIndictor.End = new Vector3(0,-(((float)AmmoAmount / MaxAmmo) * 6), 0);
            //Debug.Log(AmmoAmount);
            //Debug.Log((AmmoAmount / MaxAmmo) * 6);
            Target = null;
            Target = aim.FindClosestTarget();
            if (Target != null && Vector2.Distance(WheelTransform.position, transform.position) > wheelRadius + 8)
            {
                StartCoroutine("FireRate");
                testShoot = false;
                nextFire = Time.time + fireRate;
            }
            //scales the line depending on ammo to max ammo
        }
        //if has target

        //shoot
    }

    public void UpdateAmmo()
    {
    }

    IEnumerator FireRate()
    {
        int i = 4;

        while (i > 0)
        {
            if (AmmoAmount > 0)
            {
                ammoController.GetProjectile(BallStateTracker.BallState.NoEffector, Target.transform.position);
                ammoController.currentProjectile.SetActive(true);
                AmmoAmount--;
                //UpdateAmmo();
            }
            else yield break;

            // Do something 4 times
            i--;
            yield return Firedelay;
        }
    }

}

