using NaughtyAttributes;
using Shapes;
using UnityEngine;
using UnityUtility;

[ExecuteInEditMode]
public class WheelGenerator : MonoBehaviour
{
    [Header("Config")] public Wheel wheel;

    //public bool doCreateRegions = false;
    public int regionCount = 36;
    public float regionRadius = 3f;
    public float regionThickness = 0.5f;
    public GameObject pregenRegionDefault = null;
    public GameObject pregenBase = null;

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
            UnityU.SafeDestroy(childRegion.gameObject);
        wheel.regions.Regions = null;
    }

    public void CreateRegions()
    {
        if (pregenRegionDefault == null)
        {
            Debug.LogWarning("Regions could not generate. Missing prefab.");
            return;
        }

        wheel ??= GetComponent<Wheel>();

        var regionsMan = FindObjectOfType<WheelRegionsManager>();
        regionsMan.InitRegionTemplate();

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

            newDisc.Thickness = regionThickness;
            newDisc.Radius = regionRadius;
            newDisc.AngRadiansStart = previousRadians;
            var newRadians = previousRadians + radianSize;
            newDisc.AngRadiansEnd = newRadians;
            // End
            previousRadians = newRadians;

            ScaleColliderByThickness(newRegion.RegionCollider, regionThickness * 2f);
        }

        wheel.regions.Regions = newRegions;

        wheel.wheelCollider2D.radius = regionRadius - regionThickness * 0.5f;
        //wheel.waterWaves.GetComponent<CircleCollider2D>().radius = regionRadius + regionThickness * 0.5f;

    }

    public void ConfigTemplateRegion(Region newRegion, int index = 0)
    {
        var newGO = newRegion.gameObject;
        var newDisc = newGO.GetComponentInChildren<Disc>();
        // Config
        var radianSize = Mathf.PI * 2f / regionCount;
        var previousRadians = Mathf.Repeat(index - 1, regionCount) * radianSize;

        newDisc.Thickness = regionThickness;
        newDisc.Radius = regionRadius;
        newDisc.AngRadiansStart = previousRadians;
        var newRadians = previousRadians + radianSize;
        newDisc.AngRadiansEnd = newRadians;
        // End
        previousRadians = newRadians;

        ScaleColliderByThickness(newRegion.RegionCollider, regionThickness * 2f);
    }

    public void ScaleColliderByThickness(PolygonCollider2D collider2D, float thickness)
    {
        var points = collider2D.GetPath(0);
        for (int i = 0; i < points.Length; i++)
            points[i] *= new Vector2(1f, thickness);

        collider2D.SetPath(0, points);
    }
}