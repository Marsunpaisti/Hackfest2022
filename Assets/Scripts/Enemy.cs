using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    public bool HasBeenHit { get; private set; }
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private Vector3 _movementSpeed;
    public event Action onDestroyed;
    public event Action onReachedDamageZone;


    // Start is called before the first frame update
    void Awake()
    {
        if (_rb == null) _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasBeenHit)
        {
            _rb.velocity = _movementSpeed;
        }
    }

    public void ReceiveHit()
    {
        if (HasBeenHit) return;
        HasBeenHit = true;
        Destroy(this.gameObject, 0.5f);
    }

    public void ReachDamageZone()
    {
        if (HasBeenHit) return;
        onReachedDamageZone?.Invoke();
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        onDestroyed?.Invoke();
    }
}
