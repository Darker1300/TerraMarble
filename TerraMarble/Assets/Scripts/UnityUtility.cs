using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityUtility
{
    public static class UnityUtility
    {
        /// <summary>
        /// Sets self to be like target.
        /// </summary>
        public static T Copy<T>(this Component _self, T target) where T : Component
        {
            Type type = _self.GetType();
            if (type != target.GetType()) return null; // type mis-match
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            PropertyInfo[] pinfos = type.GetProperties(flags)
                .Where(property => !property.CustomAttributes
                    .Any(attribute => attribute.AttributeType == typeof(ObsoleteAttribute)))
                .ToArray();

            foreach (PropertyInfo pinfo in pinfos)
            {
                if (pinfo.CanWrite)
                {
                    try
                    {
                        pinfo.SetValue(_self, pinfo.GetValue(target, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown.
                }
            }
            FieldInfo[] finfos = type.GetFields(flags);
            foreach (FieldInfo finfo in finfos)
                finfo.SetValue(_self, finfo.GetValue(target));
            return _self as T;
        }

        public static T AddComponent<T>(this GameObject _gameObject, T target) where T : Component
        {
            return _gameObject.AddComponent<T>().Copy(target) as T;
        }
    }
}
