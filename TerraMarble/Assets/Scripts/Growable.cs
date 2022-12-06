using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Growable : MonoBehaviour
{
    private Animator _animator = null;

    public Animator Animator
    {
        get => (_animator == null) ?
            (_animator = GetComponent<Animator>())
            : _animator;
        set => _animator = value;
    }

    [Header("Data")]
    public int animGoalIndex = 0;
    //[Range(0f, 1f)]
    //[SerializeField] private float animPercentage = 0.0f;
    public int animMaxIndex = 0;

    private readonly int idProgress = Animator.StringToHash("Progress");
    private readonly int idProgressMax = Animator.StringToHash("MaxProgress");
    private readonly int tagEmpty = Animator.StringToHash("Empty");

    public bool isDestroyed = false;

    [HideInInspector]
    public UnityEvent<Growable> Destroyed = new UnityEvent<Growable>();

    void Awake()
    {
        animMaxIndex = Animator.GetInteger(idProgressMax);

        if (animGoalIndex != 0)
            TrySetState(animGoalIndex);

        //CalcTotalPercentage();
    }

    void Update()
    {
        if (isDestroyed
            //& Animator.IsInTransition(0)
            & Animator.GetCurrentAnimatorStateInfo(0).tagHash == tagEmpty)
        {
            if (!Application.isPlaying) return;

            Destroy(gameObject);
        }
    }

    [Button]
    public void Destroy()
    {
        isDestroyed = true;
        ResetState();
    }

    //0 = nothing on tile
    [Button]
    public bool TryGrowState()
        => TrySetState(animGoalIndex + 1);

    [Button]
    public void ResetState()
        => TrySetState(0);

    private bool TrySetState(int state)
    {
        int nextState = Math.Clamp(state, 0, animMaxIndex);

        if (state == nextState)
        {
            animGoalIndex = nextState;
            Animator.SetInteger(idProgress, animGoalIndex);

            //CalcTotalPercentage();
            return true;
        }
        else return false;
    }

    //private float CalcTotalPercentage()
    //{
    //    if (animMaxIndex == 0)
    //        animMaxIndex = Animator.GetInteger(idProgressMax);

    //    animPercentage = Mathf.InverseLerp(
    //        0f,
    //        (float)animMaxIndex,
    //        (float)animGoalIndex);
    //    return animPercentage;
    //}
}
