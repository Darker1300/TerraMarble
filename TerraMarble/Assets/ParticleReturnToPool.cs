using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleReturnToPool : MonoBehaviour
{
    public void OnParticleSystemStopped()
    {
        PoolObject poolObject = GetComponent<PoolObject>();
        if (poolObject == null) return;
        poolObject.Pool.ReturnToPool(this.gameObject);
    }
}