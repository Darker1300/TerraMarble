using Shapes;
using UnityEngine;

[ExecuteAlways]
public class Wheel : MonoBehaviour
{
    [Header("Generation")]
    public bool doCreateRegions = false;
    public int regionCount = 36;
    public float regionRadius = 3f;
    public GameObject pregenRegionDefault = null;

    [Header("Debug")]
    public Transform regionsParent;
    public Region[] regions;

    private void Start()
    {

    }

    private void Update()
    {
        if (doCreateRegions)
        {
            doCreateRegions = false;
            ClearRegions();
            CreateRegions();
        }

    }

    public void ClearRegions()
    {
        Region[] childRegions = regionsParent.GetComponentsInChildren<Region>();
        foreach (Region childRegion in childRegions)
        {
            if (Application.isEditor && !Application.isPlaying)
                DestroyImmediate(childRegion.gameObject);
            else Destroy(childRegion.gameObject);
        }
        regions = null;
    }

    public void CreateRegions()
    {
        if (pregenRegionDefault == null)
        {
            Debug.LogWarning("Regions could not generate. Missing prefab.");
            return;
        }

        Region[] newRegions = new Region[regionCount];

        float radianSize = Mathf.PI * 2f / regionCount;
        float previousRadians = 0f;
        for (int i = 0; i < regionCount; i++)
        {
            GameObject newGO = GameObject.Instantiate(pregenRegionDefault, Vector3.zero, Quaternion.identity, regionsParent);

            Region newRegion = newGO.GetComponent<Region>();
            Disc newDisc = newGO.GetComponentInChildren<Disc>();
            // Set array
            newRegions[i] = newRegion;
            // Config
            newGO.name = "Region " + i.ToString("D2");
            newDisc.Radius = regionRadius;
            newDisc.AngRadiansStart = previousRadians;
            float newRadians = previousRadians + radianSize;
            newDisc.AngRadiansEnd = newRadians;
            // End
            previousRadians = newRadians;
        }

        regions = newRegions;
    }
}
