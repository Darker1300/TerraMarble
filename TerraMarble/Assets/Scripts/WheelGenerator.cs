using NaughtyAttributes;
using Shapes;
using UnityEngine;

[ExecuteInEditMode]
public class WheelGenerator : MonoBehaviour
{
    [Header("Config")]
    public Wheel wheel;
    //public bool doCreateRegions = false;
    public int regionCount = 36;
    public float regionRadius = 3f;
    public GameObject pregenRegionDefault = null;

    void Awake()
    {
        wheel ??= GetComponent<Wheel>();
    }

    void Update()
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

        Region[] childRegions = wheel.regions.regionsParent.GetComponentsInChildren<Region>();
        foreach (Region childRegion in childRegions)
        {
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(childRegion.gameObject);
            else Destroy(childRegion.gameObject);
        }
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

        Region[] newRegions = new Region[regionCount];

        float radianSize = Mathf.PI * 2f / regionCount;
        float previousRadians = 0f;
        for (int i = 0; i < regionCount; i++)
        {
            GameObject newGO = GameObject.Instantiate(pregenRegionDefault, Vector3.zero, Quaternion.identity, wheel.regions.regionsParent);

            Region newRegion = newGO.GetComponent<Region>();
            Disc newDisc = newGO.GetComponentInChildren<Disc>();
            // Set array
            newRegions[i] = newRegion;
            // Config
            newGO.name = "Region " + i.ToString("D2");

            newRegion.RegionDisc = newDisc;

            newDisc.Radius = regionRadius;
            newDisc.AngRadiansStart = previousRadians;
            float newRadians = previousRadians + radianSize;
            newDisc.AngRadiansEnd = newRadians;
            // End
            previousRadians = newRadians;
        }

        wheel.regions.SetRegions(newRegions);
    }
}
