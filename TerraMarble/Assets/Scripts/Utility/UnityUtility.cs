using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace UnityUtility
{
    public static class UnityU
    {
        /// <summary>
        /// Sets self to be like target.
        /// </summary>
        public static T Copy<T>(this Component _self, T target) where T : Component
        {
            var type = _self.GetType();
            if (type != target.GetType()) return null; // type mis-match
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default |
                        BindingFlags.DeclaredOnly;
            var pinfos = type.GetProperties(flags)
                .Where(property => property.CustomAttributes
                    .All(attribute => attribute.AttributeType != typeof(ObsoleteAttribute)))
                .ToArray();

            foreach (var pinfo in pinfos)
                if (pinfo.CanWrite)
                    try
                    {
                        pinfo.SetValue(_self, pinfo.GetValue(target, null), null);
                    }
                    catch
                    {
                    } // In case of NotImplementedException being thrown.

            var finfos = type.GetFields(flags);
            foreach (var finfo in finfos)
                finfo.SetValue(_self, finfo.GetValue(target));
            return _self as T;
        }

        public static T AddComponent<T>(this GameObject _gameObject, T target) where T : Component
        {
            return _gameObject.AddComponent<T>().Copy(target) as T;
        }


        ///   <para>Returns transform with tag or any of its children. Works recursively.</para>
        public static List<Transform> FindChildrenWithTag(this Transform parent, string tag, List<Transform> results = null)
        {
            if (results == null) results = new List<Transform>();

            if (parent.gameObject.CompareTag(tag))
                results.Add(parent);

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                FindChildrenWithTag(child, tag, results);
            }
            return results;
        }
        /// <summary>
        ///   <para>Returns transform with tag or the first of its children with the tag. Works recursively.</para>
        /// </summary>
        public static Transform FindChildWithTag(this Transform parent, string tag)
        {
            if (parent.gameObject.CompareTag(tag))
                return parent;

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                Transform search = FindChildWithTag(child, tag);
                if (search != null) return search;
            }

            return null;
        }

        /// <summary>
        ///   <para>Finds the first transform that has a parent with a matching name, returning the parent's child. Works recursively.</para>
        /// </summary>
        public static Transform FindChildOfParentWithName(this Transform child, string name)
        {
            if (child.parent != null)
            {
                if (child.parent.name == name)
                    return child;
                return FindChildOfParentWithName(child.parent, name);
            }
            return null;
        }
        /// <summary>
        ///   <para>Finds the first transform that has a parent with a matching tag, returning the parent's child. Works recursively.</para>
        /// </summary>
        public static Transform FindChildOfParentWithTag(this Transform child, string tag)
        {
            if (child.parent != null)
            {
                if (child.parent.CompareTag(tag))
                    return child;
                return FindChildOfParentWithTag(child.parent, tag);
            }
            return null;
        }
    }

    
}