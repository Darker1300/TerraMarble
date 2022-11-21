using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Toggle : MonoBehaviour
{
    [Foldout("Events")] public UnityEvent<bool> ToggleEvent = new UnityEvent<bool>();
    [Foldout("Events")] public UnityEvent<bool> ToggleInvertedEvent = new UnityEvent<bool>();

    public bool IsToggled = false;

    public void DoToggle()
    {
        IsToggled = !IsToggled;
        ToggleEvent.Invoke(IsToggled);
        ToggleInvertedEvent.Invoke(!IsToggled);
    }
}
