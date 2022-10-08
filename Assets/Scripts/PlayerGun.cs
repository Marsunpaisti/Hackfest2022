using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] private Transform _gunBarrel;
    [SerializeField] private InputHandler _inputHandler;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private float _timeBetweenShots = 0.7f;
    private float _currentShotCooldown = 0f;
    private ObjectPool<Projectile> _projectilePool = null;


    // Start is called before the first frame update
    private void Awake()
    {
        SetProjectilePrefab(_projectilePrefab);
        if (!_inputHandler)
        {
            _inputHandler = FindObjectOfType<InputHandler>();
        }

        _inputHandler.onFire += Fire;
    }

    void SetProjectilePrefab(Projectile projectilePrefab)
    {
        if (_projectilePool != null)
        {
            StartCoroutine(nameof(DestroyOldObjectPoolOnceEmpty), _projectilePool);
        }
        this._projectilePrefab = projectilePrefab;
        _projectilePool = new ObjectPool<Projectile>(createFunc: () =>
        {
            var created = Object.Instantiate(this._projectilePrefab);
            created.SetObjectPool(_projectilePool);
            return created;
        },
        actionOnGet: (proj) => {
            proj.gameObject.SetActive(true);
        },
        actionOnRelease: (proj) =>
        {
            proj.gameObject.SetActive(false);
        },
        actionOnDestroy: (proj) =>
        {
            Object.Destroy(proj);
        }, collectionCheck: true, defaultCapacity: 20, maxSize: 200);
    }

    IEnumerator DestroyOldObjectPoolOnceEmpty(ObjectPool<Projectile> poolToDestroy)
    {
        while (poolToDestroy.CountActive != 0)
        {
            yield return new WaitForSeconds(2f);
        }

        poolToDestroy.Dispose();
    }

    private void Fire()
    {
        if (!_inputHandler.IsWandTracked) return;
        if (_currentShotCooldown > 0) return;
        Projectile newProjectile = _projectilePool.Get();
        newProjectile.Fire(_gunBarrel.position, _gunBarrel.forward);
        _currentShotCooldown = _timeBetweenShots;
    }
    public void Update()
    {
        this.transform.position = _gunBarrel.position;
        this.transform.rotation = _gunBarrel.rotation;
        _currentShotCooldown = Mathf.Max(0, _currentShotCooldown - Time.deltaTime);
    }
}
