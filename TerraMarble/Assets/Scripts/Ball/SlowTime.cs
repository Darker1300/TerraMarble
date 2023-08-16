using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityUtility;

public class SlowTime : MonoBehaviour
{
    [SerializeField] private int defaultTimeIndex = 2;
    public List<string> timeOptions = new() { "0.5", "0.6", "0.75", "0.9", "1", "1.1", "1.25", "1.4", "1.5" };

    [SerializeField] private CycleButton timeScaleButton;
    private const string timeScaleButtonName = "Time Scale Button";

    [Header("Slow Motion Config")]
    [FormerlySerializedAs("isInput")]
    [SerializeField] private bool useSlowMotion;
    [SerializeField] private float slowModeTimeScale = 0.4f;

    void Start()
    {
        timeScaleButton = timeScaleButton != null ? timeScaleButton
            : UnityU.FindObjectByName<CycleButton>(timeScaleButtonName, true);

        if (timeScaleButton != null)
        {
            timeScaleButton.SetOptions(timeOptions);
            timeScaleButton.ValueChanged.AddListener(UpdateTime);
            timeScaleButton.SetIndex(defaultTimeIndex);
        }

        InputManager.LeftDragEvent += SetSlowMotion;
        InputManager.RightDragEvent += SetSlowMotion;
    }

    private void UpdateTime() => UpdateTime(timeScaleButton.Value);

    private void UpdateTime(string _timeOption)
    {
        Time.timeScale = Convert.ToSingle(_timeOption);
    }

    public void SetSlowMotion(bool state)
    {
        if (!useSlowMotion) return;
        if (state)
        {
            Time.timeScale = slowModeTimeScale;
        }
        else
        {
            UpdateTime(timeScaleButton.Value);
        }
    }
}

