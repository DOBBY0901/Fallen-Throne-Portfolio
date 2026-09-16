using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Options")]
    [SerializeField] private bool spawnOnlyOnce = true;

    private bool hasSpawned;

    private readonly List<GameObject> spawnedEnemies =
        new List<GameObject>();

    public IReadOnlyList<GameObject> SpawnedEnemies =>
        spawnedEnemies;

    public void SpawnEnemies()
    {
        if (spawnOnlyOnce && hasSpawned)
            return;

        if (enemyPrefab == null || spawnPoints == null)
            return;

        hasSpawned = true;

        foreach (Transform point in spawnPoints)
        {
            if (point == null)
                continue;

            GameObject enemy = Instantiate(
                enemyPrefab,
                point.position,
                point.rotation
            );

            spawnedEnemies.Add(enemy);

            MinimapEnemyIconManager.Instance
                ?.RegisterEnemy(enemy.transform);
        }
    }
}
