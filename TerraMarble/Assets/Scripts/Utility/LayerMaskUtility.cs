using System.Collections.Generic;
using UnityEngine;

namespace UnityUtility
{
    public static class LayerMaskUtility
    {
        public static LayerMask Create(params string[] layerNames)
        {
            return NamesToMask(layerNames);
        }

        public static LayerMask Create(params int[] layerValues)
        {
            return layerValuesToMask(layerValues);
        }

        public static LayerMask NamesToMask(params string[] layerNames)
        {
            LayerMask ret = (LayerMask) 0;
            foreach (var name in layerNames) ret |= 1 << LayerMask.NameToLayer(name);
            return ret;
        }

        public static LayerMask layerValuesToMask(params int[] layerValues)
        {
            LayerMask ret = (LayerMask) 0;
            foreach (var layer in layerValues) ret |= 1 << layer;
            return ret;
        }

        public static LayerMask Inverse(this LayerMask original)
        {
            return ~original;
        }

        public static LayerMask AddToMask(this LayerMask original, params string[] layerNames)
        {
            return original | NamesToMask(layerNames);
        }

        public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
        {
            LayerMask invertedOriginal = ~original;
            return ~(invertedOriginal | NamesToMask(layerNames));
        }

        public static string[] MaskToNames(this LayerMask original)
        {
            var output = new List<string>();

            for (int i = 0; i < 32; ++i)
            {
                int shifted = 1 << i;
                if ((original & shifted) == shifted)
                {
                    string layerName = LayerMask.LayerToName(i);
                    if (!string.IsNullOrEmpty(layerName)) output.Add(layerName);
                }
            }

            return output.ToArray();
        }

        public static string NamesToString(this LayerMask original, string delimiter = ", ")
        {
            return string.Join(delimiter, MaskToNames(original));
        }

        public static bool CompareLayerMask(this GameObject obj, LayerMask mask)
        {
            return mask.Contains(obj);
        }

        public static bool Contains(this LayerMask mask, GameObject obj)
        {
            return mask.Contains(obj.layer);
        }

        public static bool Contains(this LayerMask mask, int layerValue)
        {
            return (mask.value & (1 << layerValue)) > 0;
        }

        public static bool CompareLayerMask(this GameObject obj, string mask)
        {
            return (LayerMask.NameToLayer(mask) & (1 << obj.layer)) > 0;
        }
    }
}