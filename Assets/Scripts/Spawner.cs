using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnArea
{
    public string pathName;
    public GameObject areaGameObject;
    public float spawnPercentage;
}

public class Spawner : MonoBehaviour
{
    public List<SpawnArea> spawnAreas; // Define the areas using GameObjects with Collider
    public float minDelay = 1.0f; // Minimum delay in seconds between enemy spawns
    public float maxDelay = 2.0f; // Maximum delay in seconds between enemy spawns

    private int _currentWave = 1;
    private int _totalEnemiesInWave;
    private GameObject _enemyPrefab;
    private List<SpawnArea> _availableSpawnAreas;
    private List<int> _numberEnemiesPerArea;

    void Start()
    {
        _enemyPrefab = Resources.Load<GameObject>("Enemy");
        CalculateAmountOfEnemies();
        _availableSpawnAreas = new List<SpawnArea>(spawnAreas);
        _numberEnemiesPerArea = CalculateEnemiesPerSpawnArea();
        StartCoroutine(SpawnWaveWithRandomDelay());
    }

    private void CalculateAmountOfEnemies()
    {
        _totalEnemiesInWave = _currentWave * 10;
    }

    void Update()
    {
        var enemiesAlive = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemiesAlive.Length == 0 && _availableSpawnAreas.Count == 0)
        {
            StartNextWave();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (var enemy in enemiesAlive)
            {
                Destroy(enemy);
            }
        }
    }

    // Calculates and returns a list of int that represents the number of enemies to spawn per spawn areas according to percentages given
    private List<int> CalculateEnemiesPerSpawnArea()
    {
        var enemiesToSpawnPerArea = new List<int>();

        for (var i = 0; i < spawnAreas.Count; i++)
        {
            var enemiesToSpawn = Mathf.RoundToInt(spawnAreas[i].spawnPercentage / 100f * _totalEnemiesInWave);
            enemiesToSpawnPerArea.Add(enemiesToSpawn);
        }

        Debug.Log(_currentWave);

        for (var i = 0; i < spawnAreas.Count; i++)
        {
            Debug.Log("Spawning " + enemiesToSpawnPerArea[i] + " in " + spawnAreas[i].pathName);
        }

        return enemiesToSpawnPerArea;
    }

    // Spawns enemies with a random delay between spawns
    private IEnumerator SpawnWaveWithRandomDelay()
    {
        while (_availableSpawnAreas.Count > 0)
        {
            var randomSpawnAreaIndex = Random.Range(0, _availableSpawnAreas.Count);
            var spawnArea = _availableSpawnAreas[randomSpawnAreaIndex];

            var spawnAreaCollider = spawnArea.areaGameObject.GetComponent<Collider>();

            if (spawnAreaCollider != null)
            {
                var spawnBounds = spawnAreaCollider.bounds;

                var randomX = Random.Range(spawnBounds.min.x, spawnBounds.max.x);
                var randomZ = Random.Range(spawnBounds.min.z, spawnBounds.max.z);

                var randomSpawnPoint = new Vector3(randomX, spawnBounds.center.y, randomZ);

                var enemy = Instantiate(_enemyPrefab, randomSpawnPoint, Quaternion.identity);

                //Randomize speed for each enemy
                var randomSpeedMultiplier = Random.Range(0.8f, 1.5f);
                enemy.GetComponent<EnemyMovement>().speed *= randomSpeedMultiplier;

                _numberEnemiesPerArea[randomSpawnAreaIndex]--;

                if (_numberEnemiesPerArea[randomSpawnAreaIndex] <= 0)
                {
                    _availableSpawnAreas.RemoveAt(randomSpawnAreaIndex);
                    _numberEnemiesPerArea.RemoveAt(randomSpawnAreaIndex);
                }

                var randomDelay = Random.Range(minDelay, maxDelay);
                yield return new WaitForSeconds(randomDelay);
            }
        }
    }

    private void StartNextWave()
    {
        _currentWave++;
        CalculateAmountOfEnemies();
        _availableSpawnAreas = new List<SpawnArea>(spawnAreas);
        _numberEnemiesPerArea = CalculateEnemiesPerSpawnArea();
        StartCoroutine(SpawnWaveWithRandomDelay());
    }
}
