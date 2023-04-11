using NaughtyAttributes;
using Shapes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class PlantBombController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float radius = 5f;
    [SerializeField] private float duration = 3f;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashSpeed = 0.25f;

    [SerializeField] private ContactFilter2D explodeFilter;

    [SerializeField] private bool showGizmos = true;
    [SerializeField] private bool showDebug = false;

    [SerializeField] private Disc bodyRenderer = null;
    [SerializeField] private BallGrabbable grabbable = null;

    [Header("Data")]
    public bool isCounting = false;
    [SerializeField] private float countdownTimer = float.PositiveInfinity;
    [SerializeField] private Color startColor = Color.white;
    [SerializeField] private WheelRegionsManager regions = null;


    private void Start()
    {
        startColor = bodyRenderer.Color;

        regions = regions != null ? regions
            : FindObjectOfType<WheelRegionsManager>();

        grabbable = grabbable != null ? grabbable
            : GetComponentInChildren<BallGrabbable>();

        grabbable.GrabEnd += OnGrabEnd;
    }

    private void OnGrabEnd(BallGrabber grabber)
        => StartCountdown();

    private void OnEnable()
    {
        isCounting = false;
        countdownTimer = duration;
    }

    [Button]
    public void StartCountdown()
    {
        countdownTimer = duration;
        isCounting = true;
    }

    void Update()
    {
        UpdateCountdown();
    }

    private void UpdateCountdown()
    {
        if (!isCounting) return;

        countdownTimer -= Time.deltaTime;
        if (countdownTimer > 0f)
        {
            bodyRenderer.Color = Color.Lerp(flashColor, startColor,
                Mathf.PingPong(countdownTimer, flashSpeed) / flashSpeed);
        }
        else
        {
            countdownTimer = 0f;
            isCounting = false;
            Explode();
        }
    }

    [Button] public void Explode()
    {
        List<Collider2D> targets = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, radius, explodeFilter, targets);

        for (var index = 0; index < targets.Count; index++)
        {
            Collider2D target = targets[index];

            // case: Region
            Transform targetParent = target.transform.parent;
            if (targetParent != null)
            {
                Region region = targetParent.GetComponent<Region>();
                if (region != null)
                {
                    if (showDebug)
                        Debug.Log($"Explode: {region.gameObject.name}");

                    if (region.regionID != Region.RegionID.Water)
                        region.TerraformToDirt();
                    continue;
                }
            }

            // case: Enemy
            ExploConfigure enemyExplosion = target.GetComponent<ExploConfigure>();
            if (enemyExplosion != null)
            {
                if (showDebug)
                    Debug.Log($"Explode: {enemyExplosion.gameObject.name}");

                // explode instantly
                enemyExplosion.ConfigureOtherExplosion();
                continue;
            }
        }

        bodyRenderer.Color = startColor;

        ObjectPooler explosionPartPool = GameObject.Find("EnemySpawner")
            .transform.Find("ParticleSpawner")
            .GetComponent<ObjectPooler>();
        GameObject newPartGO = explosionPartPool.SpawnFromPool();
        newPartGO.transform.SetParent(null, false);
        newPartGO.transform.SetPositionAndRotation(transform.position, transform.rotation);
        newPartGO.SetActive(true);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.red;
        GizmosExtensions.DrawWireCircle(transform.position, radius,
            72, Quaternion.LookRotation(Vector3.up, Vector3.forward));
    }


}
