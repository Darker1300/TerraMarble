using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CycleButton : MonoBehaviour
{
    [Header("Config")]
    public string text = "Data: {0}";

    [Header("References")]
    public Button ButtonUI;
    public TextMeshProUGUI TextUI;
    [Header("Data")]
    public int index = -1;
    [ReadOnly] public List<string> options = null;
    [HideInInspector] public UnityEvent<string> ValueChanged = new();
    public string Value => options?[index];

    void Awake()
    {
        ButtonUI = GetComponent<Button>();
        ButtonUI?.onClick.AddListener(Increment);
    }

    public void Initialise(int _defaultIndex, List<string> _options, UnityAction<string> _onUpdate)
    {
        SetOptions(_options);
        if (_onUpdate != null) ValueChanged.AddListener(_onUpdate);
        SetIndex(_defaultIndex);
    }

    public void SetOptions(List<string> _options)
    {
        options = _options;
        if (options.Count <= 0)
            index = -1;
        else
            SetIndex(0);
    }

    public void UpdateText()
    {
        // valid test, otherwise update text
        TextUI.text = index < 0 || options == null || options.Count == 0 ? text
            : String.Format(text, options[index]);
    }

    public void SetIndex(int _index)
    {
        index = _index;
        UpdateText();

        ValueChanged.Invoke(index == -1 ? string.Empty : options[index]);
    }

    public void Increment(int _amount)
    {
        if (options == null || options.Count == 0) return;

        int newIndex = (index + _amount) % options.Count;
        SetIndex(newIndex);
    }

    public void Increment()
        => Increment(1);
}
