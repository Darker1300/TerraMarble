using System.Collections;
using UnityEngine;

public class EnemyWeaponOne : MonoBehaviour
{
    private AutoAim aim;
    private AmmoController ammoController;
    private GameObject target;

    public int AmmoAmount = 2000;
    [SerializeField] private int maxShots = 2;
    public float fireRate = 1f;
    public float burstFireRate = .3f;

    private float nextFire;

    private Coroutine fireRateCoroutine;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        ammoController = GetComponent<AmmoController>();
        aim = GetComponent<AutoAim>();

        if (aim == null || ammoController == null)
        {
            Debug.LogError("Missing required components. Ensure AutoAim and AmmoController are attached.");
            enabled = false; // Disable the script if components are missing.
        }
    }

    private void Update()
    {
        if (CanFire())
        {
            target = aim.FindClosestTarget();
            if (target != null)
            {
                StartFiring();
            }
        }
    }

    private bool CanFire()
    {
        return Time.time > nextFire && ammoController != null && aim != null;
    }

    private void StartFiring()
    {
        if (fireRateCoroutine == null)
        {
            fireRateCoroutine = StartCoroutine(FireRoutine());
            nextFire = Time.time + fireRate;
        }
    }

    private IEnumerator FireRoutine()
    {
        int shotsRemaining = maxShots;

        while (shotsRemaining > 0)
        {
            if (AmmoAmount > 0)
            {
                FireProjectile();
                shotsRemaining--;
                AmmoAmount--;
            }
            else
            {
                break;
            }

            yield return new WaitForSeconds(burstFireRate);
        }

        fireRateCoroutine = null;
    }

    private void FireProjectile()
    {
        if (target == null) return;

        Vector3 targetPosition = target.transform.position;
        ammoController.GetProjectile(BallStateTracker.BallState.NoEffector, targetPosition);
        ammoController.currentProjectile.SetActive(true);
    }
}
