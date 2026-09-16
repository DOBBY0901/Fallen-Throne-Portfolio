using UnityEngine;

public class EnemySpawnTrigger : MonoBehaviour
{
    [SerializeField] private EnemySpawnManager spawnManager;
    [SerializeField] private bool triggerOnlyOnce = true;

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (triggerOnlyOnce && triggered)
            return;

        triggered = true;
        spawnManager?.SpawnEnemies();
    }
}
