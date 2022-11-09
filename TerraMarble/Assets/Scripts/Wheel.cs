using UnityEngine;


public class Wheel : MonoBehaviour
{
    [Header("Debug")]
    public Transform regionsParent;
    public Region[] regions;

    public bool dragging = false;

    private void Start()
    {
        InputManager.LeftDragEvent += OnLeftDrag;
        InputManager.LeftDragVectorEvent += OnLeftDragUpdate;
    }

    private void Update()
    {

    }

    private void OnLeftDrag(bool state)
    {
        dragging = state;
    }

    private void OnLeftDragUpdate(Vector2 currentScreenPosition, Vector2 delta)
    {
        if (!dragging) return;
        Debug.Log(delta);
    }
}
