using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.Mathematics;
using UnityEngine;

namespace UnityUtility
{
    public static class UnityU
    {
        /// <summary>
        /// Copies the properties and fields of the target component to this component.
        /// </summary>
        /// <param name="_self">This component that is being copied to.</param>
        /// <param name="target">The target component that is being copied from.</param>
        /// <returns>This component with updated properties and fields, or null if the components are of different types.</returns>
        public static T Copy<T>(this T _self, T target) where T : Component
        {
            var type = _self.GetType();

            // If this and target component are of different types, return null
            if (type != target.GetType()) return null;

            // Define the common flags for fields and properties
            const BindingFlags CommonFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                             BindingFlags.Instance | BindingFlags.DeclaredOnly;

            // Get the properties of the component
            var properties = type.GetProperties(CommonFlags)
                // Exclude properties marked with the Obsolete attribute
                .Where(property =>
                    property.CustomAttributes.All(attribute => attribute.AttributeType != typeof(ObsoleteAttribute)));

            // Copy the values of the properties from the target to this component
            foreach (var property in properties)
                if (property.CanWrite)
                    try
                    {
                        var value = property.GetValue(target, null);
                        property.SetValue(_self, value, null);
                    }
                    catch
                    {
                        // Ignore properties that can't be copied
                    }

            // Get the fields of the component
            var fields = type.GetFields(CommonFlags);

            // Copy the values of the fields from the target to this component
            foreach (var field in fields)
            {
                var value = field.GetValue(target);
                field.SetValue(_self, value);
            }

            return _self;
        }

        /// <summary>
        /// Adds a component to a game object and copies the properties and fields of a source component to the new component.
        /// </summary>
        /// <param name="_gameObject">The game object to which the component is being added.</param>
        /// <param name="copySource">The source component that is being copied from.</param>
        /// <returns>The newly added component with the properties and fields of the source component, or null if the components are of different types.</returns>
        public static T AddComponent<T>(this GameObject _gameObject, T copySource) where T : Component
        {
            var newComponent = _gameObject.AddComponent<T>();
            return newComponent.Copy(copySource);
        }

        public static Transform DestroyChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            return transform;
        }

        public static Transform DestroyImmediateChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
            return transform;
        }

        public static T FindObjectByName<T>(string _name, bool _includeInactive) where T : UnityEngine.Object
        {
            var collection = UnityEngine.Object.FindObjectsOfType<T>(_includeInactive);
            return collection.FirstOrDefault(obj => obj.name == _name);
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
            foreach (var func in funcs) yield return func.Item2.Invoke(func.Item1);
        }

        public static void SafeDestroy(GameObject gameObject)
        {
            if (Application.isEditor && !Application.isPlaying)
                UnityEngine.Object.DestroyImmediate(gameObject);
            else UnityEngine.Object.Destroy(gameObject);
        }

        public static void LogArray(IEnumerable<GameObject> array)
        {
            LogArray(array, i => i.name);
        }

        public static void LogArray<T, P>(IEnumerable<T> array, Func<T, P> dataSelection)
        {
            Debug.Log($"{array.Count()}:[{string.Join(",", array.Select(dataSelection))}]");
        }

        public static float FirstKeyValue(this AnimationCurve _self)
        {
            return _self.keys[0].value;
        }

        public static float LastKeyValue(this AnimationCurve _self)
        {
            return _self.keys[^1].value;
        }

        public static float FirstKeyTime(this AnimationCurve _self)
        {
            return _self.keys[0].time;
        }

        public static float LastKeyTime(this AnimationCurve _self)
        {
            return _self.keys[^1].time;
        }

        public static float Evaluate(this AnimationCurve _self, float time,
            float remapTimeMin, float remapTimeMax)
        {
            float normalisedTime = Mathf.Lerp(remapTimeMin, remapTimeMax, time);
            float evaluation = _self.Evaluate(normalisedTime);
            return Mathf.Lerp(remapTimeMin, remapTimeMax, evaluation);
        }

        public static float Evaluate(this AnimationCurve _self, float time,
            float remapTimeInMin, float remapTimeInMax,
            float remapTimeOutMin, float remapTimeOutMax)
        {
            return _self.Evaluate(
                math.remap(remapTimeInMin, remapTimeInMax,
                    remapTimeOutMin, remapTimeOutMax,
                    time));
        }

        public static Color AsAlpha(this Color _color, float _a)
        {
            _color.a = _a;
            return _color;
        }

        public static Dictionary<T, int> EnumToHashIDs<T>()
        {
            Dictionary<T, int> hashIDs = new();
            foreach (T parameterName in Enum.GetValues(typeof(T)))
                hashIDs.Add(parameterName,
                    Animator.StringToHash(Enum.GetName(typeof(T), parameterName)));
            return hashIDs;
        }

    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> Exclude<TSource, TKey>(this IEnumerable<TSource> source,
            IEnumerable<TSource> exclude, Func<TSource, TKey> keySelector)
        {
            var excludedSet = new HashSet<TKey>(exclude.Select(keySelector));
            return source.Where(item => !excludedSet.Contains(keySelector(item)));
        }

        /// <summary>
        /// Removes all items equal to Null.
        /// </summary>
        /// <returns>Count of items removed.</returns>
        public static int RemoveAllNull<TSource>(this IList<TSource> collection) where TSource : Component
        {
            int removeCount = 0;
            for (int i = collection.Count - 1; i >= 0; i--)
                if (collection[i] == null)
                {
                    collection.RemoveAt(i);
                    removeCount++;
                }

            return removeCount;
        }
    }
}