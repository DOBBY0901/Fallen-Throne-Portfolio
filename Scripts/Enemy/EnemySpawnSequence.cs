using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawnSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyCombatAI combatAI;
    [SerializeField] private EnemyMove enemyMove;

    [Header("Spawn Animation")]
    [SerializeField] private string spawnTriggerName = "Spawn";
    [SerializeField] private float spawnDuration = 4.833f;

    [Header("SFX")]
    [SerializeField] private AudioClip spawnSfx;

    private int spawnHash;
    private Coroutine spawnCoroutine;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (combatAI == null)
            combatAI = GetComponent<EnemyCombatAI>();

        if (enemyMove == null)
            enemyMove = GetComponent<EnemyMove>();

        spawnHash = Animator.StringToHash(spawnTriggerName);
    }

    private void OnEnable()
    {
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        DisableEnemyControl();
        PlaySpawnPresentation();

        yield return new WaitForSeconds(spawnDuration);

        EnableEnemyControl();
        spawnCoroutine = null;
    }

    private void DisableEnemyControl()
    {
        if (combatAI != null)
            combatAI.enabled = false;

        if (enemyMove != null)
            enemyMove.enabled = false;

        if (agent == null)
            return;

        if (agent.enabled && agent.isOnNavMesh)
            agent.isStopped = true;

        agent.enabled = false;
    }

    private void EnableEnemyControl()
    {
        if (agent != null)
        {
            agent.enabled = true;

            if (agent.isOnNavMesh)
                agent.isStopped = false;
        }

        if (enemyMove != null)
            enemyMove.enabled = true;

        if (combatAI != null)
            combatAI.enabled = true;
    }

    private void PlaySpawnPresentation()
    {
        if (spawnSfx != null)
            AudioManager.Instance?.Play3DSfx(spawnSfx, transform.position);

        if (animator == null)
            return;

        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Hit");
        animator.ResetTrigger("Die");
        animator.SetTrigger(spawnHash);
    }
}
