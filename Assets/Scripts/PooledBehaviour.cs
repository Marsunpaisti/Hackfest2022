using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public abstract class PooledBehaviour : MonoBehaviour
{
    public ObjectPool<PooledBehaviour> Pool { get; set; }
    public void SetPool(ObjectPool<PooledBehaviour> pool)
    {
        Pool = pool;
    }
    public virtual void ReturnToPoolOrDestroy(ExplosionVfx explosionVfx)
    {
        if (Pool == null)
        {
            Destroy(this.gameObject);
            return;
        }
        Pool.Release(this);
    }
}
