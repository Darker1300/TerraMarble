using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHighlighter : MonoBehaviour
{
    private UIRecipeManager recipeManager;

    void Awake()
    {
        recipeManager ??= GameObject.FindObjectOfType<UIRecipeManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (recipeManager == null) return;

        recipeManager.TryHighlight(collision.collider.gameObject);
    }

}
