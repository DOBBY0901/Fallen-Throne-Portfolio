using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WolfEncounterSequence : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private EnemyCombatAI combatAI;
    [SerializeField] private EnemyMove enemyMove;
    [SerializeField] private Transform player;

    [Header("Animation")]
    [SerializeField] private float howlDuration = 2f;

    [Header("Rush")]
    [SerializeField] private float rushSpeed = 7f;
    [SerializeField] private float rushDuration = 0.8f;

    [Header("SFX")]
    [SerializeField] private AudioClip howlSfx;

    private bool hasStarted;
    private Coroutine encounterCoroutine;

    private static readonly int HowlHash = Animator.StringToHash("Howl");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

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

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        SetEnemyControl(false);
    }

    public void StartEncounter()
    {
        if (hasStarted || player == null)
            return;

        encounterCoroutine = StartCoroutine(EncounterRoutine());
    }

    private IEnumerator EncounterRoutine()
    {
        hasStarted = true;
        SetEnemyControl(false);

        FacePlayer();

        if (howlSfx != null)
            AudioManager.Instance?.Play3DSfx(howlSfx, transform.position);

        if (animator != null)
            animator.SetTrigger(HowlHash);

        yield return new WaitForSeconds(howlDuration);

        if (animator != null)
            animator.SetFloat(SpeedHash, 1f);

        float elapsed = 0f;

        while (elapsed < rushDuration)
        {
            elapsed += Time.deltaTime;
            transform.position +=
                transform.forward * rushSpeed * Time.deltaTime;

            yield return null;
        }

        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = false;

        if (combatAI != null)
        {
            combatAI.enabled = true;
            combatAI.ForceChase(player);
        }

        encounterCoroutine = null;
    }

    private void SetEnemyControl(bool enabled)
    {
        if (combatAI != null)
            combatAI.enabled = enabled;

        if (enemyMove != null)
            enemyMove.enabled = enabled;

        if (agent != null && agent.enabled && agent.isOnNavMesh)
            agent.isStopped = !enabled;
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
            transform.rotation = Quaternion.LookRotation(direction);
    }

    private void OnDisable()
    {
        if (encounterCoroutine != null)
        {
            StopCoroutine(encounterCoroutine);
            encounterCoroutine = null;
        }
    }
}
