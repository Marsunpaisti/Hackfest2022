using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    [SerializeField] public AudioClip firingSound;
    [SerializeField] private float _initialVelocity = 5;
    [SerializeField] private float _lifeTimeSeconds = 8;
    [SerializeField] private float _explosionForce = 300f;
    [SerializeField] private float _explosionRadius = 4f;
    private RaycastHit[] raycastHitsBuffer = new RaycastHit[3];
    private Vector3 _lastUpdatesPosition;
    ObjectPool<Projectile> _pool = null;
    float _firedAtTime;
    public void Fire(Vector3 startPosition, Vector3 direction)
    {
        this._firedAtTime = Time.time;
        this.transform.position = startPosition;
        this.transform.rotation = Quaternion.FromToRotation(Vector3.forward, direction);
        this._rb.velocity = _initialVelocity * direction.normalized;
    }

    public void SetObjectPool(ObjectPool<Projectile> pool)
    {
        _pool = pool;
    }

    public void Explode()
    {
        VFXSpawner.SpawnExplosion(this.transform.position);
        ReturnToPoolOrDestroy();
        Collider[] hits = Physics.OverlapSphere(this.transform.position, _explosionRadius);
        foreach (Collider hit in hits)
        {
            Rigidbody rb = hit.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(_explosionForce, this.transform.position, _explosionRadius + 0.5f, 0.1f);
            }
            Enemy enemy = hit.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                enemy.ReceiveHit();
            }
        }
    }

    public void ReturnToPoolOrDestroy()
    {
        if (_pool != null)
        {
            _pool.Release(this);
        } else
        {
            Destroy(this);
        }
    }

    public void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
    }
    public void Update()
    {
        if (Time.time - _firedAtTime >= _lifeTimeSeconds)
        {
            ReturnToPoolOrDestroy();
        }
    }

    public void FixedUpdate()
    {
        CheckForImpacts();

        if (_rb.velocity.sqrMagnitude > 0.01f)
        {
            this.transform.rotation = Quaternion.FromToRotation(Vector3.forward, _rb.velocity);
        }
    }

    private void CheckForImpacts()
    {
        Vector3 currentPosition = this.transform.position;
        Vector3 delta = currentPosition - _lastUpdatesPosition;
        Ray ray = new Ray(_lastUpdatesPosition, delta);
        int hits = Physics.RaycastNonAlloc(ray, raycastHitsBuffer, delta.magnitude, -1, QueryTriggerInteraction.Ignore);
        if (hits > 0)
        {
            this.transform.position = raycastHitsBuffer[0].point;
            this.Explode();
        }
        _lastUpdatesPosition = this.transform.position;
    }
}
