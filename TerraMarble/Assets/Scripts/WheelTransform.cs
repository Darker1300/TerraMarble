using UnityEngine;

public class WheelTransform : MonoBehaviour
{
    private Wheel wheel = null;
    private Region regionLeader = null;

    void Awake()
    {
        wheel ??= GameObject.FindGameObjectWithTag("Wheel").GetComponent<Wheel>();
        regionLeader ??= wheel.regions.regionTemplate;
    }

    void Update()
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
