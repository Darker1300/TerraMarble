using MathUtility;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Walker : MonoBehaviour
{
    private Wheel wheel = null;
    private Region regionTemplate = null;
    private EntityObject entityObject = null;
    private Region regionParent = null;

    [Header("Config")]
    public float speed = 3f;
    public Vector2 targetWheelPos = new(0f, 1f);
    [Header("Debug")]
    public bool isMoving = false;
    public Vector2 currentWheelPos = new(0f, 1f);
    public Vector2 prevWheelPos = new(0f, 1f);
    public Vector2 immediateTargetWheelPos = new(0f, 1f);


    private void Awake()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionTemplate ??= wheel.regions.RegionTemplate;
        entityObject ??= GetComponent<EntityObject>();

        currentWheelPos = regionTemplate.WorldToRegion(transform.position);
        targetWheelPos = prevWheelPos = immediateTargetWheelPos = currentWheelPos;
        isMoving = false;
    }

    private void Update()
    {
        if (!isMoving) return;

        prevWheelPos = currentWheelPos;
        currentWheelPos = regionTemplate.WorldToRegion(transform.position);

        var currentSpeed = speed * 2f * Time.deltaTime;
        Vector2 oldImmedTarget = immediateTargetWheelPos;
        Vector2 newImmedTarget = new Vector2(
            MathU.MoveTowardsRange(currentWheelPos.x, targetWheelPos.x, currentSpeed, 36f),
            Mathf.MoveTowards(currentWheelPos.y, targetWheelPos.y, currentSpeed)
        );
        immediateTargetWheelPos = newImmedTarget;

        if (currentWheelPos.x < Mathf.Floor(prevWheelPos.x)
            || currentWheelPos.x > Mathf.Ceil(prevWheelPos.x))
        {
            // moved to new region

        }

        if (currentWheelPos == prevWheelPos)
        {
            // reached target
            isMoving = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isMoving) return;

        var target = regionTemplate.RegionPosition(immediateTargetWheelPos.x, immediateTargetWheelPos.y);
        var oldPos = transform.position;
        var newPos = Vector3.MoveTowards(oldPos, target, speed * Time.fixedDeltaTime);

        transform.position = newPos;
    }

    private void OnDrawGizmosSelected()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionTemplate ??= wheel.regions.RegionTemplate;

        var target = regionTemplate.RegionPosition(immediateTargetWheelPos.x, immediateTargetWheelPos.y);
        Gizmos.color = Color.white;

        if (currentWheelPos != immediateTargetWheelPos)
            Gizmos.DrawLine(transform.position, target);
    }

    public void MoveTowards(Vector2 wheelXY)
    {
        isMoving = true;

    }

    public void MoveTo(float wheelX)
    {

    }
}