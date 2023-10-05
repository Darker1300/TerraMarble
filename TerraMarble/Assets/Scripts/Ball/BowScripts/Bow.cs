using MathUtility;
using System.Collections;
using TMPro;
using UnityEngine;


public class Bow : MonoBehaviour
{
    private AutoAim aim;
    private AmmoController ammoController;
    private GameObject target;

    private TextMeshProUGUI ammoText;

    public int AmmoAmount = 2000;
    [SerializeField] private int maxShots = 2;
    public float fireRate = 0.2f;
    public float burstFireRate = 0.1f;

   private float nextFire;

    private Transform wheelTransform;
    private float wheelRadius;
    [SerializeField] private float MinHeight = 8f;

    [SerializeField] private string AmmoTextObjectName = "AmmoText";
    [SerializeField] private string WheelTag = "Wheel";

    private Coroutine fireRateCoroutine;

    private void Start()
    {
        InitializeComponents();
        UpdateAmmo();
        FindWheelTransform();
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

        ammoText = GameObject.Find(AmmoTextObjectName)?.GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (CanFire())
        {
            target = aim.FindClosestTarget();
            if (target != null && IsTargetAboveHeight()) 
                StartFiring();
        }
    }

    public void UpdateAmmo()
    {
        if (ammoText != null)
            ammoText.text = "Ammo: " + AmmoAmount;
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

    private bool IsTargetAboveHeight()
    {
        return wheelTransform.position.To2DXY()
                   .Towards(transform.position).sqrMagnitude
               > (wheelRadius + MinHeight).Squared();
    }

    private void FindWheelTransform()
    {
        wheelTransform = GameObject.FindGameObjectWithTag(WheelTag).transform;
        wheelRadius = wheelTransform.GetComponent<CircleCollider2D>().radius;
    }
}