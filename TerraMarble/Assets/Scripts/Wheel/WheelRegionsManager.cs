using System;
using System.Collections.Generic;
using NaughtyAttributes;
using System.Linq;
using Newtonsoft.Json.Converters;
using Shapes;

using UnityEngine;
using UnityEngine.Rendering;
using UnityUtility;

public class WheelRegionsManager : MonoBehaviour
{
    [Serializable]
    public class RegionConfig
    {
        public Region.RegionID ID;
        public Color RegionColor;
        public GameObject SurfacePrefab;
    }

    [Serializable]
    public class RegionConfigs : Dictionary<Region.RegionID, RegionConfig>
    {
        public List<RegionConfig> SetupData = new((int)Region.RegionID.SIZE);

        public void Initialise()
        {
            foreach (var config in SetupData)
                Add(config.ID, config);
            SetupData.Clear();
        }
    }

    [Header("Config")] public Transform regionsParent = null;

    public LayerMask surfaceLayer;
    public LayerMask wheelLayer;

    public RegionConfigs configs;
    public GameObject entityPrefabMan = null;
    public GameObject entityPrefabBeast = null;

    [Header("Data")]
    [SerializeField]
    private Region regionTemplate = null;
    [SerializeField] private Region[] regions;

    private WheelGenerator _wheelGenerator = null;


    public Region RegionTemplate
    {
        get
        {
            if (regionTemplate == null) InitRegionTemplate();
            return regionTemplate;
        }
        set => regionTemplate = value;
    }

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

    private void Awake()
    {
        // Connect with existing Regions
        if (regionsParent == null)
            regionsParent = transform.Find("Regions");

        if (regions.Length == 0 || regions.Contains(null))
            FindRegions();

        // Initialise RegionTemplate, who's Disc properties we use to calculation positions on the Wheel.
        if (RegionTemplate == null)
            InitRegionTemplate();

        if (configs.SetupData.Count > 0) configs.Initialise();

        if (surfaceLayer.value == 0) surfaceLayer = LayerMaskUtility.Create("Surface");
        if (wheelLayer.value == 0) wheelLayer = LayerMaskUtility.Create("Wheel");
    }

    [Button]
    private void InitRegionTemplate()
    {
        if (regionTemplate != null)
        {
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(regionTemplate.gameObject);
            else Destroy(regionTemplate.gameObject);
            regionTemplate = null;
        }

        var go = new GameObject("Region Template");
        go.transform.SetParent(transform, false);
        var d = go.AddComponent<Disc>(regions.First().RegionDisc);
        d.enabled = false;
        var r = go.AddComponent<Region>(regions.First());
        r.RegionDisc = d;
        regionTemplate = r;
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


    public Region.RegionHitInfo ProcessWheelHit(Collision2D collision, BallStateTracker ballState)
    {
        bool hitSurfaceObj = IsHitSurfaceObj(collision);
        bool hitRegion = IsHitRegion(collision);

        if (hitSurfaceObj || hitRegion)
        {
            Region.RegionHitInfo info = new();

            // Find Region from Surface Object
            Region regionSearch = collision.collider.GetComponentInParent<Region>();

            // Alternative Find region
            if (regionSearch == null) // if failed, find Region from contact point
            {
                Vector2 p = collision.contactCount > 0  // use Contact.point if possible
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

    private bool IsHitSurfaceObj(Collision2D collision)
    {
        return surfaceLayer.Contains(collision.collider.gameObject);
    }

    private bool IsHitRegion(Collision2D collision)
    {
        return wheelLayer.Contains(collision.collider.gameObject);
    }
}