using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class UIRecipeManager : MonoBehaviour
{
    public enum RecipeCode
    {
        NULL,
        TREE,
        MOUNTAIN
    }

    public Dictionary<string, RecipeCode> TagCodes = new Dictionary<string, RecipeCode>()
    {
        {"Tree", RecipeCode.TREE},
        {"Mountain", RecipeCode.MOUNTAIN}
    };

    [Header("Config")]
    public RecipeCode[] RecipeGoalCodes =
    {
        RecipeCode.TREE, RecipeCode.TREE, RecipeCode.TREE, RecipeCode.MOUNTAIN
    };

    public float offsetX = 1f;
    public float paddingX = 2f;

    [Header("Prefabs")]
    public GameObject PrefabIconTree;
    public GameObject PrefabIconMountain;

    [Header("Data")]
    public List<Toggle> Highlighted = new List<Toggle>();
    public Toggle[] Icons;

    void Awake()
    {

    }

    void Update()
    {

    }

    public GameObject CodeToPrefab(RecipeCode _code)
    {
        return _code switch
        {
            RecipeCode.TREE => PrefabIconTree,
            RecipeCode.MOUNTAIN => PrefabIconMountain,
            _ => null
        };
    }

    public RecipeCode ToggleToCode(Toggle _tog)
    {
        if (!TagCodes.TryGetValue(_tog.tag, out RecipeCode code))
            return RecipeCode.NULL;
        return code;
    }

    public void ClearIcons()
    {
        for (int i = 0; i < Icons.Length; i++)
        {
            if (Icons[i] == null) continue;
            Destroy(Icons[i].gameObject);
            Icons[i] = null;
        }

        if (Icons == null || Icons.Length > 0) Icons = new Toggle[0];
    }

    [Button]
    public void CreateIcons()
    {
        ClearIcons();
        Icons = new Toggle[RecipeGoalCodes.Length];
        for (int i = 0; i < RecipeGoalCodes.Length; i++)
        {
            GameObject goIcoPrefab = CodeToPrefab(RecipeGoalCodes[i]);
            GameObject goIcon = GameObject.Instantiate(goIcoPrefab, transform, false);
            goIcon.name = (i + 1).ToString() + ": " + RecipeGoalCodes[i].ToString();
            goIcon.transform.localPosition = Vector3.right * paddingX * ((i - offsetX) / RecipeGoalCodes.Length);
            ToggleColors goTog = goIcon.GetComponent<ToggleColors>();
            goTog.DoToggle();
            Icons[i] = (Toggle)goTog;
        }
    }

    public void TryHighlight(GameObject _gameObject)
    {
        Toggle tog = _gameObject.GetComponent<Toggle>();
        if (tog == null) return;

        string togTag = tog.tag;
        if (!TagCodes.ContainsKey(togTag)) return;

        if (!Highlighted.Contains(tog))
        {
            for (int i = 0; i < Icons.Length; i++)
            {
                var icon = Icons[i];
                if (icon == null) continue;
                if (RecipeGoalCodes[i] == TagCodes[togTag] && icon.IsToggled)
                {
                    icon.DoToggle();
                    break;
                }
            }

            Highlighted.Add(tog);
            tog.DoToggle();

            int progress = 0;
            for (int i = 0; i < Icons.Length; i++)
            {
                var icon = Icons[i];
                if (icon == null) continue;
                if (!icon.IsToggled) progress++;
            }
            if (progress == RecipeGoalCodes.Length) ClearHighlights();
        }
    }

    [Button]
    public void ClearHighlights()
    {
        for (var index = 0; index < Highlighted.Count; index++)
        {
            Toggle tog = Highlighted[index];
            if (tog.IsToggled) tog.DoToggle();
            Highlighted[index] = null;
        }
        Highlighted.Clear();

        for (var index = 0; index < Icons.Length; index++)
        {
            Toggle icon = Icons[index];
            if (icon == null) continue;
            if (!icon.IsToggled) icon.DoToggle();
        }
    }
}
