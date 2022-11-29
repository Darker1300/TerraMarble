using MathUtility;
using UnityEngine;

public class Walker : MonoBehaviour
{
    private Wheel wheel = null;
    private Region regionTemplate = null;

    [Header("Config")] public float speed = 3f;

    public Vector2 targetWheelPos = new(0f, 1f);
    [Header("Debug")] public Vector2 currentWheel = new(0f, 1f);
    public Vector2 currentGoalWheel = new(0f, 1f);


    private void Awake()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionTemplate ??= wheel.regions.regionTemplate;

        currentWheel = regionTemplate.WorldToRegion(transform.position);
        targetWheelPos = currentWheel;
        currentGoalWheel = targetWheelPos;
    }

    private void Update()
    {
        currentWheel = regionTemplate.WorldToRegion(transform.position);

        var currentSpeed = speed * 2f * Time.deltaTime;
        currentGoalWheel = new Vector2(
            MathU.MoveTowardsRange(currentWheel.x, targetWheelPos.x, currentSpeed, 36f),
            Mathf.MoveTowards(currentWheel.y, targetWheelPos.y, currentSpeed)
        );
    }

    private void FixedUpdate()
    {
        var target = regionTemplate.RegionPosition(currentGoalWheel.x, currentGoalWheel.y);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionTemplate ??= wheel.regions.regionTemplate;

        var target = regionTemplate.RegionPosition(currentGoalWheel.x, currentGoalWheel.y);
        Gizmos.color = Color.white;

        if (currentWheel != currentGoalWheel)
            Gizmos.DrawLine(transform.position, target);
    }
}