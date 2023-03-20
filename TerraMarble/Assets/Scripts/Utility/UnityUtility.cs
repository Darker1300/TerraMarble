using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

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
        public static List<Transform> FindChildrenWithTag(this Transform parent, string tag,
            List<Transform> results = null)
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

        public static Coroutine StartCoroutine(this MonoBehaviour mb, (object, Func<object, YieldInstruction>) funcs)
        {
            return mb.StartCoroutine(CoroutineGroup(new (object, Func<object, YieldInstruction>)[] { funcs }));
        }

        public static Coroutine StartCoroutine(this MonoBehaviour mb,
            params (object, Func<object, YieldInstruction>)[] funcs)
        {
            return mb.StartCoroutine(CoroutineGroup(funcs));
        }

        private static IEnumerator CoroutineGroup((object, Func<object, YieldInstruction>)[] funcs)
        {
            foreach (var func in funcs)
            {
                yield return func.Item2.Invoke(func.Item1);
            }
        }

        public static void SafeDestroy(GameObject gameObject)
        {
            if (Application.isEditor && !Application.isPlaying)
                Object.DestroyImmediate(gameObject);
            else Object.Destroy(gameObject);
        }

        public static void LogArray(IEnumerable<GameObject> array)
            => LogArray(array, i => i.name);

        public static void LogArray<T, P>(IEnumerable<T> array, Func<T, P> dataSelection)
        {
            Debug.Log($"{array.Count()}:[{string.Join(",", array.Select(dataSelection))}]");
        }

        public static float FirstKeyValue(this AnimationCurve _self)
            => _self.keys[0].value;

        public static float LastKeyValue(this AnimationCurve _self)
            => _self.keys[^1].value;

        public static float FirstKeyTime(this AnimationCurve _self)
            => _self.keys[0].time;

        public static float LastKeyTime(this AnimationCurve _self)
            => _self.keys[^1].time;

    }
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> Exclude<TSource, TKey>(this IEnumerable<TSource> source,
            IEnumerable<TSource> exclude, Func<TSource, TKey> keySelector)
        {
            var excludedSet = new HashSet<TKey>(exclude.Select(keySelector));
            return source.Where(item => !excludedSet.Contains(keySelector(item)));
        }
    }
}