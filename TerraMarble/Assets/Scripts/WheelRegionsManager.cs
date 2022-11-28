using NaughtyAttributes;
using System.Linq;
using UnityEngine;
using UnityUtility;

public class WheelRegionsManager : MonoBehaviour
{
    public Transform regionsParent = null;

    public Region[] regions;

    public Region regionTemplate = null;

    void Awake()
    {
        if (regionsParent == null)
            regionsParent = transform.Find("Regions");

        if (regions.Length == 0 || regions.Contains(null))
            FindRegions();

        if (regionTemplate == null)
        {
            GameObject go = new GameObject("Region Template");
            go.AddComponent<Region>(regions.First());
        }
    }

    void Update()
    {

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
