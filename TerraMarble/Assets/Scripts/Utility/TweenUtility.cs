using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UnityUtility
{
    public static class TweenUtility
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

                Curve = _curveType switch
                {
                    CurveType.Linear => AnimationCurve.Linear(0, Progress, 1, Duration),
                    CurveType.EaseInOut => AnimationCurve.EaseInOut(0, Progress, 1, Duration),
                    _ => Curve
                };
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