using System;
using System.Collections.Generic;
using NaughtyAttributes;
using System.Linq;
using MathUtility;
using Shapes;
using UnityEngine;
using UnityUtility;
using static Region;

public class WheelRegionsManager : MonoBehaviour
{
    [Header("Config")] public Transform regionsParent = null;

    public LayerMask surfaceLayer;
    public LayerMask wheelLayer;

    public RegionConfigSet configs;
    public GameObject entityPrefabMan = null;
    public GameObject entityPrefabBeast = null;

    [Header("Data")] [SerializeField] private Region regionTemplate = null;
    [SerializeField] private Region[] regions;

    private WheelGenerator _wheelGenerator = null;


    public Region RegionTemplate
    {
        get
        {
            if (regionTemplate == null)
                CreateRegionTemplate();
            return regionTemplate;
        }
        set => regionTemplate = value;
    }

    public bool RegionTemplateIsNull => regionTemplate == null;

    public Region this[int key]
        => regions[key];

    public Region[] Regions
    {
        get => regions;
        set => regions = value;
    }

    public WheelGenerator WheelGenerator
    {
        get => _wheelGenerator == null
            ? _wheelGenerator = FindObjectOfType<WheelGenerator>()
            : _wheelGenerator;
        set => _wheelGenerator = value;
    }

    public int RegionCount => WheelGenerator.regionCount;

    public float WheelRadius => RegionTemplate.RegionPosition(0f, 1f).x;

    private void Awake()
    {
        // Connect with existing Regions
        if (regionsParent == null)
            regionsParent = transform.Find("Regions");

        if (regions.Length == 0 || regions.Contains(null))
            FindRegions();

        // Initialise RegionTemplate, who's Disc properties we use to calculation positions on the Wheel.
        if (RegionTemplate == null)
            CreateRegionTemplate();

        //if (configs.SetupData.Count > 0) configs.Initialise();

        if (surfaceLayer.value == 0) surfaceLayer = LayerMaskUtility.Create("Surface");
        if (wheelLayer.value == 0) wheelLayer = LayerMaskUtility.Create("Wheel");
    }


    [Button]
    public void CreateRegionTemplate()
    {
        var exampleRegionGO = Instantiate(WheelGenerator.pregenRegionDefault, Vector3.zero, Quaternion.identity);
        var exampleRegion = exampleRegionGO.GetComponent<Region>();

        var go = new GameObject("Region Template");
        var r = go.AddComponent<Region>(exampleRegion);


        if (regionTemplate != null)
        {
            UnityU.SafeDestroy(regionTemplate.gameObject);
            regionTemplate = null;
        }

        regionTemplate = r;

        var d = go.AddComponent<Disc>(exampleRegion.RegionDisc);
        go.transform.SetParent(transform, false);

        r.Base = null;
        WheelGenerator.ConfigTemplateRegion(r, 0);

        d.enabled = false;
        r.RegionDisc = d;


        UnityU.SafeDestroy(exampleRegionGO);
    }

    [Button]
    public void FindRegions()
    {
        var regs = regionsParent.GetComponentsInChildren<Region>(true);

        if (regions == null || regions.Length != regs.Length)
            regions = new Region[regs.Length];

        for (var i = 0; i < regs.Length; i++)
        {
            regs[i].FindBase();
            regions[i] = regs[i];
        }
    }

    [Button]
    public void MakeRegionBases()
    {
        for (var i = 0; i < regions.Length; i++)
        {
            if (regions[i] == null) continue;
            regions[i].TryCreateBase();
        }
    }

    [Button]
    public void ResetEmptyRegionBases()
    {
        for (var i = 0; i < regions.Length; i++)
        {
            if (regions[i] == null) continue;
            var rBase = regions[i].Base;
            if (rBase != null && rBase.childCount == 0)
                regions[i].ResetBase();
        }
    }

    public int WorldToRegionIndex(Vector2 _worldPos)
    {
        return RegionTemplate.WorldToRegionIndex(_worldPos);
    }

    /// <summary>
    /// Returns Index + 0..1f
    /// </summary>
    public float WorldToRegionDistance(Vector2 _worldPos)
    {
        return RegionTemplate.WorldToRegionDistance(_worldPos);
    }

    public Region GetClosestRegion(Vector2 _worldPos)
    {
        return regions[WorldToRegionIndex(_worldPos)];
    }

    public float RegionDistanceDelta(float _currentDst, float _targetDst)
    {
        float delta = MathU.DeltaRange(_currentDst, _targetDst, RegionCount);
        return delta;
    }

    public RegionHitInfo ProcessWheelHit(Collision2D collision, BallStateTracker ballState)
    {
        bool hitSurfaceObj = IsHitSurfaceObj(collision);
        bool hitRegion = IsHitRegion(collision);

        if (hitSurfaceObj || hitRegion)
        {
            RegionHitInfo info = new();

            // Find Region from Surface Object
            Region regionSearch = collision.collider.GetComponentInParent<Region>();

            // Alternative Find region
            if (regionSearch == null) // if failed, find Region from contact point
            {
                Vector2 p = collision.contactCount > 0 // use Contact.point if possible
                    ? collision.GetContact(0).point // contact position
                    : transform.position; // Ball position
                regionSearch = GetClosestRegion(p);
            }

            if (hitSurfaceObj)
                info.surfaceObj = collision.collider.GetComponentInParent<SurfaceObject>();
            else info.surfaceObj = null;

            info.ballState = ballState;
            info.collision = collision;
            info.region = regionSearch;
            return info;
        }

        return null;
    }

    public bool IsHitSurfaceObj(Collision2D collision)
    {
        return surfaceLayer.Contains(collision.collider.gameObject);
    }

    public bool IsHitRegion(Collision2D collision)
    {
        return wheelLayer.Contains(collision.collider.gameObject);
    }
}