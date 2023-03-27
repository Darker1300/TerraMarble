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
    [SerializeField] private float duration = 3f;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashSpeed = 0.25f;

    [SerializeField] private bool showGizmos = true;
    [SerializeField] private bool showDebug = false;

    [SerializeField] private Disc grabUI = null;
    [SerializeField] private Disc renderer = null;
    [SerializeField] private BallGrabbable grabbable = null;

    [Header("Data")]
    [SerializeField] private bool isCounting = false;
    [SerializeField] private float timer = float.PositiveInfinity;
    
    [SerializeField] private bool isGrabbed = true;

    [SerializeField] private TallPlantController mother = null;
    [SerializeField] private WheelRegionsManager regions = null;

    [SerializeField] private Color startColor = Color.white;

    [SerializeField] private LayerMask explodeLayerMask;

    private void Awake()
    {
        startColor = renderer.Color;

        mother = mother == null
            ? GetComponentInParent<TallPlantController>()
            : mother;

        regions = regions == null
            ? FindObjectOfType<WheelRegionsManager>()
            : regions;

        grabbable = grabbable == null
            ? GetComponentInChildren<BallGrabbable>()
            : grabbable;

        grabbable.GrabEnd.AddListener(StartCountdown);
    }

    public void Initialise(TallPlantController _mother, WheelRegionsManager _regions)
    {
        mother = _mother;
        regions = _regions;
        isCounting = false;
        timer = duration;
        isGrabbed = false;
    }

    void Update()
    {
        UpdateCountdown();
    }

    public void StartGrab()
    {
        if (isCounting) return;


    }

    public void EndGrab()
    {
        isCounting = true;
    }

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
