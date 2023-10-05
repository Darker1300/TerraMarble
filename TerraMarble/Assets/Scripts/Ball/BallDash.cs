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
    [SerializeField] private int dashPartEmitCount = 5;
    [SerializeField] private bool useMinVelocityForDash = false;

    [Header("Config Dash Costs")]
    [SerializeField] private float dashCost = 5f;

    [SerializeField] private int dashCostDefaultIndex = 1;
    public List<string> dashCostOptions = new() { "1", "5", "10", "12.5", "16.67", "25", "50"};
    [SerializeField] private CycleButton dashCostButton;
    private const string dashCostButtonName = "Dash Cost Button";
    public bool FreeDash=true;
    //[Header("Config Dash Recharge")]
    //[SerializeField] private float dashRechargeTime = 0.25f;
    //[SerializeField] private float dashRechargeRange = 20f;
    //[SerializeField] private ToggleParticles dashRechargeParticles;

    [Header("References")]
    [SerializeField] private PlayerInput playerInput = null;
    [SerializeField] private PlayerHealth playerHealth = null;
    [SerializeField] private Rigidbody2D ballRb = null;
    [SerializeField] private ParticleSystem partSystem = null;
    [SerializeField] private NearbySensor nearbySensor = null;
    [SerializeField] private WheelRegionsManager wheel = null;

    //[Header("Data")]
    //[SerializeField] private float dashRechargeTimer = 0f;

    public LayerMask collisionLayer;
    void Start()
    {
        ballRb = gameObject.GetComponentInParent<Rigidbody2D>();
        partSystem = partSystem != null ? partSystem
            : GetComponentInChildren<ParticleSystem>();
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

    public bool CanAffordDash()
    {
        //if (playerHealth == false) return false;
        //return

        //if (playerHealth == null) return false;
        if (FreeDash==true)
        {
            FreeDash = false;
            return true;
        }
        if (playerHealth.CurrentShield > dashCost - float.Epsilon || CheckForCollision())
        {
            return true;
        }
        else if (FreeDash == true)
        {
            FreeDash = false;
            return true;
        }
        else return false;

    }
    public bool CheckForCollision()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, GetComponentInChildren<CircleCollider2D>().radius, collisionLayer);
        if (colliders.Length > 0)
        {
            return true;
        }
        else return false;
    }
    /// <returns>If successfully applied force.</returns>
    public bool DoDash(float newDashForce, int side, bool forceDash = false, bool consumeDash = true)
    {
        if (!forceDash && useMinVelocityForDash)
        {
            if (ballRb.velocity.sqrMagnitude < minVelocityForDash)
                return false;
        }

        if (consumeDash && !CanAffordDash())
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
        float shieldPercent = playerHealth.CurrentShield * (1f / playerHealth.MaxShield);
        float emitCount = dashPartEmitCount * Mathf.Max(1f - shieldPercent, 0.2f);
        int partEmitCount = Mathf.CeilToInt(emitCount);
        partSystem?.Emit(partEmitCount);

        // Apply Damage
        playerHealth.ConsumeShield(dashCost);

        return true;
    }
}
