using System;
using System.Collections.Generic;
using NaughtyAttributes;
using System.Linq;
using Newtonsoft.Json.Converters;
using Shapes;
using UnityEditor.AnimatedValues;
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
        public List<RegionConfig> SetupData = new((int) Region.RegionID.SIZE);

        public void Initialise()
        {
            foreach (var config in SetupData)
                Add(config.ID, config);
            SetupData.Clear();
        }
    }

    [Header("Config")] public Transform regionsParent = null;

    [SerializeField] private RegionConfigs configs = new();

    [Header("Data")] public Region regionTemplate = null;
    [SerializeField] private Region[] regions;

    public int RegionCount => GetComponent<WheelGenerator>().regionCount;

    public void SetRegions(Region[] _regions)
    {
        regions = _regions;
    }

    private void Awake()
    {
        // Connect with existing Regions
        if (regionsParent == null)
            regionsParent = transform.Find("Regions");

        if (regions.Length == 0 || regions.Contains(null))
            FindRegions();

        // Initialise RegionTemplate, who's Disc properties we use to calculation positions on the Wheel.
        if (regionTemplate == null)
        {
            var go = new GameObject("Region Template");
            go.transform.SetParent(transform, false);
            var d = go.AddComponent<Disc>(regions.First().RegionDisc);
            d.enabled = false;
            var r = go.AddComponent<Region>(regions.First());
            r.RegionDisc = d;
            regionTemplate = r;
        }

        configs.Initialise();
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
        return regionTemplate.WorldToRegionIndex(_worldPos);
    }

    /// <summary>
    /// Returns Index + 0..1f
    /// </summary>
    public float WorldToRegionDistance(Vector2 _worldPos)
    {
        return regionTemplate.WorldToRegionDistance(_worldPos);
    }

    public Region GetClosestRegion(Vector2 _worldPos)
    {
        return regions[WorldToRegionIndex(_worldPos)];
    }
}