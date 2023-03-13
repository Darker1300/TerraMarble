using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;

// v1.2 - added helper class that uses SimpleJSON to parse needed Unity types
/// <summary>
/// Helper class that uses SimpleJSON as base to parse needed Unity types
/// </summary>
public static class JSON_Helper
{
    public static AnimationCurve AsAnimationCurve(JSONNode jsonNode)
    {
        AnimationCurve curve = new AnimationCurve();

        JSONArray jsonCurveKeys = jsonNode["m_Curve"].AsArray;
        foreach (SimpleJSON.JSONNode node in jsonCurveKeys)
        {
            Keyframe key = new Keyframe();
            
            key.time = node["time"].AsFloat;
            key.value = node["value"].AsFloat;
            key.inTangent = node["inSlope"].AsFloat;
            key.outTangent = node["outSlope"].AsFloat; 
            key.tangentMode = node["tangentMode"].AsInt;
            key.weightedMode = (WeightedMode)node["weightedMode"].AsInt;
            key.inWeight = node["inWeight"].AsFloat;
            key.outWeight = node["outWeight"].AsFloat;

            curve.AddKey(key);
        }

        return curve;
    }

    public static Keyframe[] AsKeyframeArray(JSONNode jsonNode)
    {
        List<Keyframe> keyframes = new List<Keyframe>();

        JSONArray jsonCurveKeys = jsonNode["m_Curve"].AsArray;
        foreach (SimpleJSON.JSONNode node in jsonCurveKeys)
        {
            Keyframe key = new Keyframe();
            
            key.time = node["time"].AsFloat;
            key.value = node["value"].AsFloat;
            key.inTangent = node["inSlope"].AsFloat;
            key.outTangent = node["outSlope"].AsFloat;
            key.tangentMode = node["tangentMode"].AsInt;
            key.weightedMode = (WeightedMode)node["weightedMode"].AsInt;
            key.inWeight = node["inWeight"].AsFloat;
            key.outWeight = node["outWeight"].AsFloat;

            keyframes.Add(key);
        }

        return keyframes.ToArray();
    }

    public static T AsEnum<T>(JSONNode jsonNode)
    {
        return (T)System.Enum.Parse(typeof(T), jsonNode);
    }

    public static List<Vector3> AsVector3List(JSONNode jsonNode)
    {
        List<Vector3> vector3List = new List<Vector3>();

        JSONArray jsonVector3Array = jsonNode.AsArray;
        foreach (SimpleJSON.JSONNode node in jsonVector3Array)
        {
            Vector3 vector = new Vector3();
            vector = node.ReadVector3();
            vector3List.Add(vector);
        }

        return vector3List;
    }
}
