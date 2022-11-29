using UnityEngine;

public class WheelTransform : MonoBehaviour
{
    private Wheel wheel = null;
    private Region regionLeader = null;

    private void Awake()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionLeader ??= wheel.regions.regionTemplate;
    }

    private void Update()
    {
    }

    private void OnDrawGizmosSelected()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionLeader ??= wheel.regions.regionTemplate;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wheel.transform.position,
            regionLeader.transform.lossyScale.x * regionLeader.RadiusFull);
    }
}