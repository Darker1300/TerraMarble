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

        //public static List<GameObject> FindChildrenWithTag(this Transform parent, string tag)
        //{
        //    List<GameObject> taggedGameObjects = new List<GameObject>();

        //    for (int i = 0; i < parent.childCount; i++)
        //    {
        //        Transform child = parent.GetChild(i);
        //        if (child.tag == tag)
        //        {
        //            taggedGameObjects.Add(child.gameObject);
        //        }
        //        if (child.childCount > 0)
        //        {
        //            taggedGameObjects.AddRange(FindChildrenWithTag(child, tag));
        //        }
        //    }
        //    return taggedGameObjects;
        //}
    }

    public static class Tween
    {
        [Serializable]
        public class AnimCurve
        {
            public enum CurveType
            {
                Linear,
                EaseInOut
            }

            public AnimCurve()
            {
            }

            public AnimCurve(float _start, float _target, float _duration,
                AnimationCurve _curve, float _speed = 1f, float _progress = 0f)
            {
                Start = _start;
                Target = _target;
                Duration = _duration;
                Speed = _speed;
                Progress = _progress;

                Curve = _curve;
                Curve.Evaluate(Progress);
            }

            public AnimCurve(float _start = 0f, float _target = 1f, float _duration = 1f,
                CurveType _curveType = CurveType.Linear, float _speed = 1f, float _progress = 0f)
            {
                Start = _start;
                Target = _target;
                Duration = _duration;
                Speed = _speed;
                Progress = _progress;

                if (_curveType == CurveType.Linear)
                    Curve = AnimationCurve.Linear(0, Progress, 1, Duration);
                else if (_curveType == CurveType.EaseInOut)
                    Curve = AnimationCurve.EaseInOut(0, Progress, 1, Duration);
                Curve.Evaluate(Progress);
            }

            public AnimationCurve Curve = AnimationCurve.Linear(0, 1, 1, 1);

            public float Start = 0f;
            public float Target = 1f;

            public float Duration = 1f;
            public float Speed = 1f;

            public float Progress = 0f;
            public float CurveValue = 0f;

            [SerializeField] private bool _animating = false;
            public bool Animating => _animating;

            [HideInInspector] public UnityEvent<bool> Activation = new();
            [HideInInspector] public UnityEvent<float> Updated = new();
            [HideInInspector] public UnityEvent<float> Finished = new();

            public void Play()
            {
                if (_animating) return;
                _animating = true;
                CurveValue = Curve.Evaluate(0f);
                Activation.Invoke(Animating);
            }

            public void Play(float _target)
            {
                if (_animating) return;
                Start = CurveValue;
                Target = _target;
                Progress = 0f;
                Play();
            }

            public void Reset()
            {
                Progress = 0f;
                CurveValue = Curve.Evaluate(Progress);
            }

            public void Pause()
            {
                if (_animating != false) return;
                _animating = false;
                Activation.Invoke(Animating);
            }

            public void Stop()
            {
                if (_animating != false) return;
                _animating = false;
                Duration = Progress;
                Activation.Invoke(Animating);
            }

            private void Finish()
            {
                _animating = false;
                Progress = Curve.keys.Last().time;
                Finished.Invoke(Progress);
            }

            public void Update()
            {
                if (Animating)
                {
                    var newProgress = Mathf.MoveTowards(Progress, Curve.keys.Last().time,
                        Time.deltaTime * Speed * (1f / Duration));
                    Progress = newProgress;
                    CurveValue = Mathf.Lerp(Start, Target, Curve.Evaluate(Progress));

                    Updated.Invoke(CurveValue);
                    if (Mathf.Approximately(Progress, Curve.keys.Last().time))
                        Finish();
                }
            }
        }
    }
}