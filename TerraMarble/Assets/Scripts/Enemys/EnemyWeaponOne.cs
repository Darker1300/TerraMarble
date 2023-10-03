using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponOne : MonoBehaviour
{
    WaitForSeconds Firedelay = new WaitForSeconds(.5f);
    Coroutine coroutine;
    //[SerializeField] TextMeshProUGUI AmmoText;
    public GameObject Target;
    [SerializeField]
    private AutoAim aim;
    private AmmoController ammoController;
    public int AmmoAmount;
    public bool testShoot = false;

    public float fireRate = 0.5F;
    private float nextFire = 0.0F;
    [SerializeField] private ChangeColorOverTime colorChange;
    // Start is called before the first frame update
    void Start()
    {
        ammoController = GetComponent<AmmoController>();
        aim = GetComponent<AutoAim>();
       // AmmoText = GameObject.FindObjectOfType<TextMeshProUGUI>();
        //UpdateAmmo();
    }

    // Update is called once per frame
    void Update()
    {
        //if has ammo
        if (AmmoAmount != 0 && Time.time > nextFire && AmmoAmount > 0)
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

    public void UpdateAmmo()
    {
        //if (AmmoText)
          //  AmmoText.text = "Fruit: " + AmmoAmount;
    }

    IEnumerator FireRate()
    {
        int i = 2;
        //colorChange.LerpToColor();
        //yield return new WaitForSeconds(.5f);

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
