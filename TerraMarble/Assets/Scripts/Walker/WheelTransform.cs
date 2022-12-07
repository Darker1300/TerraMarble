using UnityEngine;

public class WheelTransform : MonoBehaviour
{
    private Wheel wheel = null;
    private Region regionLeader = null;

    private void Awake()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionLeader ??= wheel.regions.RegionTemplate;
    }

    private void Update()
    {
    }

    private void OnDrawGizmosSelected()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionLeader ??= wheel.regions.RegionTemplate;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(wheel.transform.position,
            regionLeader.transform.lossyScale.x * regionLeader.RadiusFull);
    }
}