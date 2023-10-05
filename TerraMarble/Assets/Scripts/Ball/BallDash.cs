using MathUtility;
using System.Collections.Generic;
using UnityEngine;
using UnityUtility;

public class BallDash : MonoBehaviour
{
    [Header("Config Dash")]
    [SerializeField] private float minScreenDragForDash = 0.01f;
    [SerializeField] private float minVelocityForDash = .1f;
    [SerializeField] private float minVelocityForSideDash = 1f;
    [SerializeField] private float dashForce = 50f;
    //[SerializeField] private int dashPartEmitCount = 5;
    [SerializeField] private bool useMinVelocityForDash;

    [Header("Config Dash Costs")]
    [SerializeField] private float dashCost = 5f;

    [SerializeField] private ParticleSystem dashParticleSystem;
    [SerializeField] private Color freeDashColor = Color.yellow;
    [SerializeField] private Color paidDashColor = Color.cyan;
    public bool FreeDash = true;
    public int freeDashCurrent = 2;
    [SerializeField] private int freeDashMax;


    [SerializeField] private int dashCostDefaultIndex = 1;
    public List<string> dashCostOptions = new() { "1", "5", "10", "12.5", "16.67", "25", "50" };
    [SerializeField] private CycleButton dashCostButton;
    private const string dashCostButtonName = "Dash Cost Button";
    //[Header("Config Dash Recharge")]
    //[SerializeField] private float dashRechargeTime = 0.25f;
    //[SerializeField] private float dashRechargeRange = 20f;
    //[SerializeField] private ToggleParticles dashRechargeParticles;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Rigidbody2D ballRb;
    [SerializeField] private NearbySensor nearbySensor;
    [SerializeField] private WheelRegionsManager wheel;

    public LayerMask collisionLayer;
    Collider2D[] dashCollisionObjs;
    CircleCollider2D ballCollider;

    void Start()
    {
        dashParticleSystem = dashParticleSystem != null ? dashParticleSystem
            : transform.Find("Dash Particles")?.GetComponent<ParticleSystem>();

        ballRb = gameObject.GetComponentInParent<Rigidbody2D>();
        nearbySensor = nearbySensor != null ? nearbySensor
            : GetComponentInChildren<NearbySensor>();
        playerInput = playerInput != null ? playerInput
            : FindObjectOfType<PlayerInput>();
        playerHealth = playerHealth != null ? playerHealth
            : FindObjectOfType<PlayerHealth>();
        wheel = wheel != null ? wheel
            : FindObjectOfType<WheelRegionsManager>();

        dashCostButton = dashCostButton != null ? dashCostButton
            : UnityU.FindObjectByName<CycleButton>(dashCostButtonName, true);
        dashCostButton?.Initialise(dashCostDefaultIndex, dashCostOptions, UpdateDashCost);

        //dashRechargeParticles = dashRechargeParticles != null ? dashRechargeParticles
        //    : UnityU.FindObjectByName<ToggleParticles>("Dash Charging Particles", true);

        InputManager.TapLeft += () => DoDash(dashForce, -1);
        InputManager.TapRight += () => DoDash(dashForce, 1);

        //nearbySurfaceObjects = nearbySensor.FindBuffer(dashRechargeBufferName);
        //nearbySensor.Updated += NearbySensorUpdated;

        dashCollisionObjs = new Collider2D[1];
        ballCollider = GetComponentInChildren<CircleCollider2D>();
    }

    private void UpdateDashCost(string _newDashCost)
    {
        float newCost = float.Parse(_newDashCost);
        dashCost = newCost;
    }

    //private void Update()
    //{
    //    //bool isNearby = nearbySurfaceObjects.ColliderSet.Stay.Count > 0;

    //    //bool leftNearby = nearbySurfaceObjects.ColliderSet.Exit.Count > 0
    //    //                     && nearbySurfaceObjects.ColliderSet.Stay.Count == 0;
    //    //bool enteredNearby = nearbySurfaceObjects.ColliderSet.Enter.Count ==
    //    //                         nearbySurfaceObjects.ColliderSet.Stay.Count;


    //    //bool isRange = wheel.transform.position.To2DXY()
    //    //                             .Towards(transform.position.To2DXY())
    //    //                             .sqrMagnitude <
    //    //                         (dashRechargeRange * dashRechargeRange);

    //    //if (dashRechargeParticles == null) return;

    //    //if (isRange)
    //    //    dashRechargeParticles.Play();
    //    //else
    //    //    dashRechargeParticles.Pause();
    //}

    public void ResetFreeDash()
    {
        FreeDash = true;
        freeDashCurrent = freeDashMax;
    }

    public bool PayForDash()
    {
        if (playerHealth == null) return false;

        //if on ground reset free dash amount
        if (CheckForCollision())
        {
            FreeDash = true;
            freeDashCurrent = freeDashMax - 1;
            return true;
            //call minus one free dash
        }

        //if we are in air
        if (FreeDash)
        {
            if (freeDashCurrent > 0)
            {
                freeDashCurrent -= 1;
                if (freeDashCurrent < 1)
                {
                    FreeDash = false;
                }
                return true;
            }
        }

        if (playerHealth.CurrentShield > dashCost - float.Epsilon)
        {
            playerHealth.ConsumeShield(dashCost);
            return true;
        }

        return false;
    }

    public bool CheckForCollision()
    {
        var size = Physics2D.OverlapCircleNonAlloc(
            transform.position, ballCollider.radius + 0.1f, dashCollisionObjs, collisionLayer);
        return size > 0;
    }

    /// <returns>If successfully applied force.</returns>
    public bool DoDash(float newDashForce, int side, bool forceDash = false, bool consumeDash = true)
    {
        if (!forceDash && useMinVelocityForDash)
        {
            if (ballRb.velocity.sqrMagnitude < minVelocityForDash)
                return false;
        }

        bool isFreeDash = FreeDash;

        if (consumeDash && !PayForDash())
            return false;

        Vector2 dashDirection;

        // if is dragging
        if (playerInput.RawScreenDrag.sqrMagnitude > minScreenDragForDash)
        {
            // Dash towards screen drag direction

            // Convert DragLeftScreenVector to world position
            Vector3 dragStart, dragEnd;

            if (side == -1)
            {
                dragStart = InputManager.DragLeftStartScreenPos;
                dragEnd = InputManager.DragLeftEndScreenPos;
            }
            else
            {
                dragStart = InputManager.DragRightStartScreenPos;
                dragEnd = InputManager.DragRightEndScreenPos;
            }

            dragStart = Camera.main.ScreenToWorldPoint(dragStart);
            dragEnd = Camera.main.ScreenToWorldPoint(dragEnd);

            // Calculate the direction in world space
            Vector2 worldVector = (dragEnd - dragStart).To2DXY();

            // Normalize the local direction
            dashDirection = worldVector.normalized;
        }
        // else if is moving
        else if (ballRb.velocity.sqrMagnitude > minVelocityForSideDash)
        {
            // Dash forwards
            dashDirection = ballRb.velocity.normalized;
        }
        else
        {
            // Dash upwards
            dashDirection = ballRb.transform.up.To2DXY();
        }

        // apply force
        ballRb.velocity = dashDirection * newDashForce;

        // particles
        EmitDashParticles(isFreeDash, dashDirection);

        // Apply Damage

        return true;
    }

    public void EmitDashParticles(bool useFreeDashColor, Vector2 direction)
    {
        // set color
        Color dashColor = paidDashColor;
        if (useFreeDashColor)
            dashColor = freeDashColor;

        ParticleSystem.MainModule ma = dashParticleSystem.main;
        ma.startColor = dashColor;

        // set rotation
        dashParticleSystem.transform.rotation =
            Quaternion.LookRotation(Vector3.forward, -direction.To3DXY(0f));

        // play
        dashParticleSystem.Play();
    }
}
