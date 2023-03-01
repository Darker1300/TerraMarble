using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleReturnToPool : MonoBehaviour
{
    private void OnDisable()
    {
        GetComponent<PoolObject>()?.Pool.ReturnToPool(this.gameObject);
    }
}
