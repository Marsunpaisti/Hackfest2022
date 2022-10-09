using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class GameManager : MonoBehaviour
{
    [SerializeField] BoxCollider _spawnZone;
    [SerializeField] BoxCollider _floor;
    [SerializeField] AudioSource _soundEffectsSource;
    [SerializeField] AudioSource _musicPlayerSource;
    [SerializeField] AudioClip _damageSound;
    [SerializeField] AudioClip _victorySound;
    [SerializeField] AudioClip _gameLostSound;
    [SerializeField] AudioClip _bgMusic;
    [SerializeField] Enemy _enemyPrefab;
    [SerializeField] float _initialSpawnInterval = 2;
    [SerializeField] float _spawnInterval = 2;
    [SerializeField] int _maxSpawnCount = 25;
    [SerializeField] int _startingHealth = 3;
    [SerializeField] int _currentHealth = 3;
    [SerializeField] int _totalEnemiesSpawned = 0;
    [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
    Coroutine spawnEnemiesCoutine = null;
    public static Vector3 RandomPointInBounds(Bounds bounds)
    {
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }

    public void Awake()
    {
        _musicPlayerSource.clip = _bgMusic;
    }
    public void RestartGame()
    {
        Debug.Log("Game restarting.");
        if (spawnEnemiesCoutine != null) StopCoroutine(spawnEnemiesCoutine);
        _currentHealth = _startingHealth;
        _spawnInterval = _initialSpawnInterval;
        _totalEnemiesSpawned = 0;
        _enemies.ForEach(e => Destroy(e.gameObject));
        _enemies.Clear();
        _musicPlayerSource.Stop();
        spawnEnemiesCoutine = StartCoroutine(nameof(EnemySpawnerCoroutine));
    }
    IEnumerator EnemySpawnerCoroutine()
    {
        yield return new WaitForSeconds(3f);
        _musicPlayerSource.Play();
        while (_totalEnemiesSpawned < _maxSpawnCount)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_spawnInterval);
            _spawnInterval = _spawnInterval * 0.95f;
        }
        spawnEnemiesCoutine = null;
    }
    void SpawnEnemy()
    {
        Vector3 spawnPoint = RandomPointInBounds(_spawnZone.bounds);
        spawnPoint.y = _floor.transform.position.y + (_floor.bounds.size.y / 2);
        var spawned = Instantiate(_enemyPrefab);
        spawned.transform.position = spawnPoint;
        _enemies.Add(spawned);
        _totalEnemiesSpawned += 1;
        Debug.Log($"Spawned enemy number {_totalEnemiesSpawned}");
        spawned.onDestroyed += () =>
        {
            _enemies.Remove(spawned);
            MaybeWinGame();
        };

        spawned.onReachedDamageZone += OnEnemyReachedDamageZone;

    }

    void MaybeWinGame()
    {
        if (_totalEnemiesSpawned < _maxSpawnCount) return;
        if (_enemies.Any(e => !e.HasBeenHit)) return;

        Debug.Log("Player won the game!");
        _soundEffectsSource.PlayOneShot(_victorySound);
        RestartGame();
    }

    void OnEnemyReachedDamageZone()
    {
        _currentHealth -= 1;
        _soundEffectsSource.PlayOneShot(_damageSound);
        Debug.Log($"Damage received. Current health: {_currentHealth}");
        if (_currentHealth == 0 )
        {
            LoseGame();
        }
    }

    void LoseGame()
    {
        _soundEffectsSource.PlayOneShot(_gameLostSound);
        RestartGame();
    }

    public void Start()
    {
        RestartGame();
    }
}
