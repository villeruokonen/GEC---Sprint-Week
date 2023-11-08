using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public List<Transform> spawnPoints;
    public List<float> spawnPercentages; // Percentage for each spawn point
    public int totalEnemiesInWave = 50;
    public float spawnDelay = 1.0f; // Delay in seconds between enemy spawns

    private List<Transform> availableSpawnPoints;
    private GameObject enemyPrefab;
    private List<int> numberEnemiesPerPoint;

    void Start()
    {
        numberEnemiesPerPoint = CalculateEnemiesPerSpawnPoint();
        availableSpawnPoints = new List<Transform>(spawnPoints);
        enemyPrefab = Resources.Load<GameObject>("Enemy");
        StartCoroutine(SpawnWaveWithDelay());
    }

    // Calculates and returns a list of int that represents the number of enemies to spawn per spawn points according to percentages given
    private List<int> CalculateEnemiesPerSpawnPoint()
    {
        var enemiesToSpawnPerPoint = new List<int>();

        for (var i = 0; i < spawnPoints.Count; i++)
        {
            var enemiesToSpawn = Mathf.RoundToInt(spawnPercentages[i] / 100f * totalEnemiesInWave);
            enemiesToSpawnPerPoint.Add(enemiesToSpawn);
        }

        return enemiesToSpawnPerPoint;
    }

    // Spawns enemies with a delay in between spawns
    private IEnumerator SpawnWaveWithDelay()
    {
        while (availableSpawnPoints.Count > 0)
        {
            var randomSpawnPoint = Random.Range(0, availableSpawnPoints.Count);

            var spawnedEnemy = Instantiate(enemyPrefab, availableSpawnPoints[randomSpawnPoint].position, availableSpawnPoints[randomSpawnPoint].rotation);
            numberEnemiesPerPoint[randomSpawnPoint]--;

            if (numberEnemiesPerPoint[randomSpawnPoint] <= 0)
            {
                availableSpawnPoints.RemoveAt(randomSpawnPoint);
                numberEnemiesPerPoint.RemoveAt(randomSpawnPoint);
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }
}