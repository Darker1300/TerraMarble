using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Shapes;
using UnityEngine;
using UnityUtility;

public class PlantBombController : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private float radius = 5f;
    public float duration = 3f;
    public Color flashColor = Color.red;
    [SerializeField] private float flashSpeed = 0.25f;
    public bool showGizmos = true;
    public bool showDebug = false;

    [Header("Data")]
    public bool isCounting = false;
    public float timer = float.PositiveInfinity;
    public TallPlantController mother = null;
    public WheelRegionsManager regions = null;
    public Disc renderer = null;
    public Color startColor = Color.white;

    public LayerMask explodeLayerMask;

    private void Awake()
    {
        renderer = renderer == null ? GetComponent<Disc>() : renderer;
        startColor = renderer.Color;
    }

    public void Initialise(TallPlantController _mother, WheelRegionsManager _regions)
    {
        mother = _mother;
        regions = _regions;
        isCounting = false;
        timer = duration;
        renderer = renderer == null ? GetComponent<Disc>() : renderer;
    }

    void Update()
        => UpdateCountdown();

    [Button]
    public void TestPrefire()
    {
        Initialise(
            GetComponentInParent<TallPlantController>(),
            FindObjectOfType<WheelRegionsManager>());
    }

    [Button]
    public void StartCountdown()
    {
        timer = duration;
        isCounting = true;
    }

    private void UpdateCountdown()
    {
        if (!isCounting) return;

        timer -= Time.deltaTime;
        if (timer > 0f)
        {
            renderer.Color = Color.Lerp(flashColor, startColor,
                Mathf.PingPong(timer, flashSpeed) / flashSpeed);
        }
        else
        {
            timer = 0f;
            isCounting = false;
            Explode();
        }
    }

    [Button]
    public void Explode()
    {
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(explodeLayerMask);
        filter.useTriggers = true;

        List<Collider2D> targets = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, radius, filter, targets);

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

                    region.TerraformToWater();
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
        //int closestRegionIndex = regions.RegionTemplate.WorldToRegionIndex(transform.position);
        //var closestRegion =  regions[closestRegionIndex];

        renderer.Color = startColor;

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
