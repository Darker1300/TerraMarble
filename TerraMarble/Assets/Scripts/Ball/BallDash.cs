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

    [Header("Config Dash Limit")]
    [SerializeField] private int dashMax = 5;
    [SerializeField] private int dashRemaining = 5;
    [SerializeField] private int dashMaxDefaultIndex = 5;
    public List<string> dashMaxOptions = new() { "0", "1", "2", "3", "4", "5", "10", "99" };
    [SerializeField] private CycleButton dashMaxButton;
    private const string dashMaxButtonName = "Dash Max Button";

    [Header("Config Dash Recharge")]
    [SerializeField] private float dashRechargeTime = 0.25f;
    [SerializeField] private float dashRechargeRange = 20f;
    //[SerializeField] private string dashRechargeBufferName = "Surface";
    //[SerializeField] private NearbySensor.ColliderBuffer nearbySurfaceObjects;
    [SerializeField] private ToggleParticles dashRechargeParticles;


    [Header("References")]
    [SerializeField] private PlayerInput playerInput = null;
    [SerializeField] private Rigidbody2D ballRb = null;
    [SerializeField] private ParticleSystem partSystem = null;
    [SerializeField] private NearbySensor nearbySensor = null;
    [SerializeField] private WheelRegionsManager wheel = null;

    [Header("Data")]
    [SerializeField] private float dashRechargeTimer = 0f;


    void Start()
    {
        ballRb = gameObject.GetComponentInParent<Rigidbody2D>();
        partSystem = partSystem != null ? partSystem
            : GetComponentInChildren<ParticleSystem>();
        nearbySensor = nearbySensor != null ? nearbySensor
            : GetComponentInChildren<NearbySensor>();
        playerInput = playerInput != null ? playerInput
            : FindObjectOfType<PlayerInput>();
        wheel = wheel != null ? wheel
                : FindObjectOfType<WheelRegionsManager>();

        dashMaxButton = dashMaxButton != null ? dashMaxButton
            : UnityU.FindObjectByName<CycleButton>(dashMaxButtonName, true);
        dashMaxButton?.Initialise(dashMaxDefaultIndex, dashMaxOptions, UpdateDashMax);

        dashRechargeParticles = dashRechargeParticles != null ? dashRechargeParticles
            : UnityU.FindObjectByName<ToggleParticles>("Dash Charging Particles", true);

        InputManager.TapLeft += () => DoDash(dashForce, -1);
        InputManager.TapRight += () => DoDash(dashForce, 1);

        //nearbySurfaceObjects = nearbySensor.FindBuffer(dashRechargeBufferName);
        //nearbySensor.Updated += NearbySensorUpdated;
    }

    private void UpdateDashMax(string _newMaxDash)
    {
        int newMax = int.Parse(_newMaxDash);
        dashMax = newMax;
        dashRemaining = newMax;
    }

    private void Update()
    {
        //bool isNearby = nearbySurfaceObjects.ColliderSet.Stay.Count > 0;
        
        //bool leftNearby = nearbySurfaceObjects.ColliderSet.Exit.Count > 0
        //                     && nearbySurfaceObjects.ColliderSet.Stay.Count == 0;
        //bool enteredNearby = nearbySurfaceObjects.ColliderSet.Enter.Count ==
        //                         nearbySurfaceObjects.ColliderSet.Stay.Count;

        bool isRange = wheel.transform.position.To2DXY()
                                     .Towards(transform.position.To2DXY())
                                     .sqrMagnitude <
                                 (dashRechargeRange * dashRechargeRange);

        if (dashRechargeParticles == null) return;

        if (isRange)
            dashRechargeParticles.Play();
        else
            dashRechargeParticles.Pause();
    }

    /// <returns>If successfully applied force.</returns>
    public bool DoDash(float newDashForce, int side, bool forceDash = false, bool consumeDash = true)
    {
        if (!forceDash && useMinVelocityForDash)
        {
            if (ballRb.velocity.sqrMagnitude < minVelocityForDash)
                return false;
        }

        if (consumeDash && dashRemaining < 1)
            return false;

        Vector2 dashDirection;

        // if is dragging
        if (playerInput.RawScreenDrag.sqrMagnitude > minScreenDragForDash)
        {
            // Dash towards screen drag direction

            // Convert DragLeftScreenVector to world position
            Vector3 dragLeftWorldVectorStart = Camera.main.ScreenToWorldPoint(
                side == -1 ? InputManager.DragLeftStartScreenPos : InputManager.DragRightStartScreenPos);
            Vector3 dragLeftWorldVectorEnd = Camera.main.ScreenToWorldPoint(
                side == -1 ? InputManager.DragLeftEndScreenPos : InputManager.DragRightEndScreenPos);
            // Calculate the direction in world space
            Vector2 worldDirection = dragLeftWorldVectorEnd - dragLeftWorldVectorStart;

            // Normalize the local direction
            dashDirection = worldDirection.normalized;
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
        int partEmitCount = Mathf.CeilToInt(dashPartEmitCount * ((float)dashRemaining / dashMax));
        partSystem?.Emit(partEmitCount);

        dashRemaining -= 1;

        return true;
    }
}
