using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// WIP
/// </summary>
[Serializable]
public abstract class SerializedHashSet<T> : HashSet<T>, ISerializationCallbackReceiver
{
    [SerializeField] [HideInInspector] private List<T> currentList = new();

    /// <returns>Ignores Null</returns>
    public abstract T HandleDuplicateAdded(T duplicate);


    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        // Move List into HashSet
        Clear();

        for (var index = 0; index < currentList.Count; index++)
        {
            var currentItem = currentList[index];
            if (!Contains(currentItem))
            {
                Add(currentItem);
            }
            else
            {
                T updatedItem = HandleDuplicateAdded(currentItem);
                if (updatedItem != null)
                    Add(updatedItem);
            }
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        // Move HashSet into List
        currentList.Clear();
        foreach (var item in this) currentList.Add(item);
    }
}