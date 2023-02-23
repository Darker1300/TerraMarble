using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{

    WaitForSeconds Firedelay = new WaitForSeconds(.1f);
    Coroutine coroutine;

    public GameObject Target;
    [SerializeField]
    private AutoAim aim;
    private AmmoController ammoController;
    public int AmmoAmount;
    public bool testShoot = false;

    public float fireRate = 0.5F;
    private float nextFire = 0.0F;
    // Start is called before the first frame update
    void Start()
    {
        ammoController = GetComponent<AmmoController>();
        aim = GetComponent<AutoAim>();


    }

    // Update is called once per frame
    void Update()
    {
        //if has ammo
        if (AmmoAmount != 0 && Time.time > nextFire)
        {
            Target = null;
            Target = aim.FindClosestTarget();
            if (Target != null)
            {
                StartCoroutine("FireRate");
                testShoot = false;
                nextFire = Time.time + fireRate;
            }

        }
        //if has target

        //shoot
    }


    IEnumerator FireRate()
    {
        int i = 4;

        while (i > 0)
        {
            ammoController.GetProjectile(BallStateTracker.BallState.NoEffector, Target.transform.position);
            ammoController.currentProjectile.SetActive(true);

            // Do something 4 times
            i--;
            yield return Firedelay;
        }
    }

}
