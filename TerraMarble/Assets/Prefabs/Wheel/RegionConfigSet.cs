using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Wheel/Regions Config", order = 2, fileName = "RegionsConfig")]
[Serializable]
public class RegionConfigSet : SerializedScriptableObject
{
    public RegionConfigDictionary Data;

    public static implicit operator RegionConfigDictionary(RegionConfigSet wrapper)
    {
        return wrapper.Data;
    }

    public RegionConfig this[Region.RegionID regionID]
    {
        get
        {
            Data.TryGetValue(regionID, out RegionConfig actualValue);
            return actualValue;
        }
    }

    [Button("Instantiate")]
    public void Instantiate()
    {
        for (int i = 0; i < (int) Region.RegionID.SIZE; i++)
            if (!Data.ContainsKey((Region.RegionID) i))
            {
                // Create new config
                RegionConfig regionConfig = new RegionConfig();
                // Color
                float lux = (i + 1f) / (int) Region.RegionID.SIZE;
                regionConfig.RegionColor = new Color(lux, lux, lux, 1f);
                // Add
                Data.Add((Region.RegionID) i, regionConfig);
            }
    }

    [Button("Clear")]
    public void Clear()
    {
        Data.Clear();
    }
}


[Serializable]
public class RegionConfigDictionary : SerializedDictionary<Region.RegionID, RegionConfig>
{
}

[Serializable]
public class RegionConfig
{
    public Color RegionColor;
    public GameObject SurfacePrefab;
}