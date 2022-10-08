using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class VFXSpawner : MonoBehaviour
{
    private static VFXSpawner _instance;
    [SerializeField] private PooledBehaviour _explosionPrefab;
    private ObjectPool<PooledBehaviour> _explosionPool;
    void Awake()
    {
        if (_instance != null) Object.Destroy(this.gameObject);
        _instance = this;

        _explosionPool = new ObjectPool<PooledBehaviour>(
            createFunc: () => {
                var newObj =  Object.Instantiate(_explosionPrefab);
                newObj.SetPool(_explosionPool);
                return newObj;
            }, 
            actionOnGet: OnGetFromPool, 
            actionOnRelease: OnReleaseToPool, 
            actionOnDestroy: OnDestroyFromPool);
    }

    public static PooledBehaviour SpawnExplosion(Vector3 position)
    {
        var spawned = _instance._explosionPool.Get();
        spawned.gameObject.transform.position = position;
        return spawned;
    }

    void OnGetFromPool(PooledBehaviour obj)
    {
        obj.gameObject.SetActive(true);
    }

    void OnReleaseToPool(PooledBehaviour obj)
    {
        obj.gameObject.SetActive(false);
    }
    void OnDestroyFromPool(PooledBehaviour obj)
    {
        Destroy(obj);
    }

}
