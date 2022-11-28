using System;
using System.Collections.Generic;
using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityUtility;

public class WheelRegionsManager : MonoBehaviour
{
    public enum RegionID
    {
        Water,
        Rock,
        Dirt,
        Grass,
        Forest,
        SIZE
    }

    [Serializable]
    public class RegionConfig
    {
        public RegionID ID;
        public Color RegionColor;
        public GameObject SurfacePrefab;
    }

    [Header("Config")]
    public Transform regionsParent = null;

    [SerializeField] private RegionConfig[] configs = new RegionConfig[(int)RegionID.SIZE];

    [Header("Data")]
    public Region regionTemplate = null;
    private Region[] regions;

    public int RegionCount => GetComponent<WheelGenerator>().regionCount;
    public void SetRegions(Region[] _regions) => regions = _regions;

    void Awake()
    {
        // Connect with existing Regions
        if (regionsParent == null)
            regionsParent = transform.Find("Regions");

        if (regions.Length == 0 || regions.Contains(null))
            FindRegions();

        // Initialise RegionTemplate, who's Disc properties we use to calculation positions on the Wheel.
        if (regionTemplate == null)
        {
            GameObject go = new GameObject("Region Template");
            go.AddComponent<Region>(regions.First());
        }


    }

    [Button]
    public void FindRegions()
    {
        Region[] regs = regionsParent.GetComponentsInChildren<Region>(true);

        if (regions.Length != regs.Length)
            regions = new Region[regs.Length];

        for (int i = 0; i < regs.Length; i++)
        {
            regs[i].FindBase();
            regions[i] = regs[i];
        }
    }

    [Button]
    public void MakeRegionBases()
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (regions[i] == null) continue;
            regions[i].TryCreateBase();
        }
    }
    [Button]
    public void ResetEmptyRegionBases()
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (regions[i] == null) continue;
            Transform rBase = regions[i].Base;
            if (rBase != null && rBase.childCount == 0)
                regions[i].ResetBase();
        }
    }

    public int WorldToRegionIndex(Vector2 _worldPos)
        => regionTemplate.WorldToRegionIndex(_worldPos);

    /// <summary>
    /// Returns Index + 0..1f
    /// </summary>
    public float WorldToRegionDistance(Vector2 _worldPos)
        => regionTemplate.WorldToRegionDistance(_worldPos);

    public Region GetClosestRegion(Vector2 _worldPos)
        => regions[WorldToRegionIndex(_worldPos)];
}
