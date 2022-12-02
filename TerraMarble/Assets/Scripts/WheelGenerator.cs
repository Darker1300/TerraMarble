using NaughtyAttributes;
using Shapes;
using UnityEngine;

[ExecuteInEditMode]
public class WheelGenerator : MonoBehaviour
{
    [Header("Config")] public Wheel wheel;

    //public bool doCreateRegions = false;
    public int regionCount = 36;
    public float regionRadius = 3f;
    public GameObject pregenRegionDefault = null;

    private void Awake()
    {
        wheel ??= GetComponent<Wheel>();
    }

    private void Update()
    {
        //if (doCreateRegions)
        //{
        //    doCreateRegions = false;
        //}
    }

    [Button]
    public void RecreateRegions()
    {
        ClearRegions();
        CreateRegions();
    }

    public void ClearRegions()
    {
        wheel ??= GetComponent<Wheel>();

        var childRegions = wheel.regions.regionsParent.GetComponentsInChildren<Region>();
        foreach (var childRegion in childRegions)
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(childRegion.gameObject);
            else Destroy(childRegion.gameObject);
        wheel.regions = null;
    }

    public void CreateRegions()
    {
        if (pregenRegionDefault == null)
        {
            Debug.LogWarning("Regions could not generate. Missing prefab.");
            return;
        }

        wheel ??= GetComponent<Wheel>();

        var newRegions = new Region[regionCount];

        var radianSize = Mathf.PI * 2f / regionCount;
        var previousRadians = 0f;
        for (var i = 0; i < regionCount; i++)
        {
            var newGO = Instantiate(pregenRegionDefault, Vector3.zero, Quaternion.identity,
                wheel.regions.regionsParent);

            var newRegion = newGO.GetComponent<Region>();
            var newDisc = newGO.GetComponentInChildren<Disc>();
            // Set array
            newRegions[i] = newRegion;
            // Config
            newGO.name = "Region " + i.ToString("D2");

            newRegion.RegionDisc = newDisc;

            newDisc.Radius = regionRadius;
            newDisc.AngRadiansStart = previousRadians;
            var newRadians = previousRadians + radianSize;
            newDisc.AngRadiansEnd = newRadians;
            // End
            previousRadians = newRadians;
        }

        wheel.regions.SetRegions(newRegions);
    }
}