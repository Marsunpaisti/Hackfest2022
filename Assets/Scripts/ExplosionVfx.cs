using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
public class ExplosionVfx : PooledBehaviour
{
    VisualEffect _effect;
    [SerializeField] BoxCollider _particleCollisionCube;
    [SerializeField] Light _light;
    [SerializeField] AudioSource _audioSource;
    float _playedTime;
    float _minLifeTime = 5f;
    [SerializeField] private AudioClip[] audioClips;
    [SerializeField] AnimationCurve lightIntensity;

    public void Awake()
    {
        if (_effect == null) _effect = GetComponent<VisualEffect>();
        if (_light == null) _light = GetComponent<Light>();
        if (_audioSource) _audioSource = GetComponent<AudioSource>();
        if (_particleCollisionCube == null) _particleCollisionCube = GameObject.FindGameObjectWithTag("Floor").GetComponent<BoxCollider>();
        _effect.SetVector3("CollisionBoxCenter", _particleCollisionCube.transform.position);
        _effect.SetVector3("CollisionBoxSize", _particleCollisionCube.bounds.size);
    }
    public void OnEnable()
    {
        _effect.Play();
        _playedTime = Time.time;
        if (audioClips.Length > 0)
        {
            int randomIndex = Random.Range(0, audioClips.Length - 1);
            _audioSource.PlayOneShot(audioClips[randomIndex]);
        }
    }

    public void Update()
    {
        float timeAlive = Time.time - _playedTime;
        _light.intensity = lightIntensity.Evaluate(timeAlive);

        if (_effect.aliveParticleCount == 0 && timeAlive > _minLifeTime)
        {
            ReturnToPoolOrDestroy(this);
        }
    }

}
