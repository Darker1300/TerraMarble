using MathUtility;
using UnityEngine;

public class Walker : MonoBehaviour
{
    private Wheel wheel = null;
    private Region regionTemplate = null;

    [Header("Config")]
    public float speed = 3f;

    public Vector2 targetWheelPos = new Vector2(0f, 1f);
    [Header("Debug")]
    public Vector2 currentWheel = new Vector2(0f, 1f);
    public Vector2 currentGoalWheel = new Vector2(0f, 1f);


    void Awake()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionTemplate ??= wheel.regions.regionTemplate;

        currentWheel = regionTemplate.WorldToRegion(transform.position);
        targetWheelPos = currentWheel;
        currentGoalWheel = targetWheelPos;
    }

    void Update()
    {
        currentWheel = regionTemplate.WorldToRegion(transform.position);

        float currentSpeed = speed * 2f * Time.deltaTime;
        currentGoalWheel = new Vector2(
            MathU.MoveTowardsRange(currentWheel.x, targetWheelPos.x, currentSpeed, 36f),
            Mathf.MoveTowards(currentWheel.y, targetWheelPos.y, currentSpeed)
            );
    }

    void FixedUpdate()
    {
        Vector2 target = regionTemplate.RegionPosition(currentGoalWheel.x, currentGoalWheel.y);
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.fixedDeltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionTemplate ??= wheel.regions.regions[0];

        Vector2 target = regionTemplate.RegionPosition(currentGoalWheel.x, currentGoalWheel.y);
        Gizmos.color = Color.white;

        if (currentWheel != currentGoalWheel)
            Gizmos.DrawLine(transform.position, target);
    }
}
