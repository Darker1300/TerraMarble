using System.Collections;
using System.Collections.Generic;
using MathUtility;
using Shapes;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private float cooldownDuration = 2f;
    [SerializeField] private float attackTime = 0.015f;

    [SerializeField] private Disc attackOutlineDisc = null;
    [SerializeField] private Disc attackFillDisc = null;

    [SerializeField] private float cooldownTimeRemaining = 0f;

    public bool IsOnCooldown = false;
    public bool IsAttacking = false;

    private float playerSqrDistance = float.MaxValue;
    private Transform playerTransform = null;
    private PlayerHealth playerHealth = null;
    private CircleCollider2D attackCollider = null;
    private float attackRange => attackCollider.radius;
    private float attackVelocity = 0f;

    void Start()
    {
        playerTransform ??= FindObjectOfType<BallAnimController>()?.transform;
        playerHealth ??= FindObjectOfType<PlayerHealth>();
        attackCollider = GetComponent<CircleCollider2D>();
    }

    void FixedUpdate()
    {
        UpdateCooldown();

        if (!IsOnCooldown)
        {   // Can Attack
            playerSqrDistance = transform.position.To2DXY()
                .Towards(playerTransform.position.To2DXY())
                .sqrMagnitude;

            if (!playerHealth.IsImmune && playerSqrDistance < attackRange)
            {   // target in range
                IsAttacking = true;
                attackOutlineDisc.Radius = attackRange;
                attackFillDisc.Radius =
                    Mathf.SmoothDamp(attackFillDisc.Radius, attackRange, ref attackVelocity, attackTime);

                if (Mathf.InverseLerp(0f, attackRange, attackFillDisc.Radius) > 0.99f)
                {   // finished attacking
                    playerHealth.Damage(1);
                    ResetAttacking();
                    StartCooldown();
                }
            }
            else if (IsAttacking && attackOutlineDisc.Radius > 0f)
            {   // target just left range
                ResetAttacking();
            }
        }


    }

    void StartCooldown()
    {
        IsOnCooldown = true;
        cooldownTimeRemaining = cooldownDuration;
    }

    void UpdateCooldown()
    {
        if (cooldownTimeRemaining > 0f)
        {
            cooldownTimeRemaining -= Time.deltaTime;
            if (cooldownTimeRemaining < 0f)
            {
                cooldownTimeRemaining = 0f;
                IsOnCooldown = false;
            }
        }
    }

    void ResetAttacking()
    {
        IsAttacking = false;
        attackVelocity = 0f;
        attackFillDisc.Radius = 0f;
        attackOutlineDisc.Radius = 0f;
    }
}
