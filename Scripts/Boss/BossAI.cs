using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Basic Attack")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float attackCooldown = 4f;

    [Header("Root Motion")]
    [SerializeField] private bool useRootMotionAttack = true;

    private bool isActivated;
    private bool isAttacking;
    private bool isDead;
    private bool isSpecialPattern;

    public bool IsAttacking => isAttacking;
    public bool IsSpecialPattern => isSpecialPattern;
    public bool IsDead => isDead;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int AttackAHash = Animator.StringToHash("AttackA");
    private static readonly int AttackBHash = Animator.StringToHash("AttackB");
    private static readonly int AttackCHash = Animator.StringToHash("AttackC");

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent != null)
            agent.isStopped = true;
    }

    private void Start()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (!isActivated || isDead || isAttacking || isSpecialPattern)
            return;

        if (player == null)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
            StartCoroutine(BasicAttackRoutine());
        else
            ChasePlayer();

        UpdateAnimation();
    }

    public void StartBoss()
    {
        if (isDead)
            return;

        isActivated = true;

        if (agent != null && agent.enabled)
            agent.isStopped = false;
    }

    public void StopBoss()
    {
        isDead = true;
        isActivated = false;
        isAttacking = false;
        isSpecialPattern = false;

        if (agent != null && agent.enabled)
        {
            agent.isStopped = true;
            agent.updatePosition = true;
            agent.updateRotation = true;
        }

        if (animator != null)
            animator.SetFloat(SpeedHash, 0f);
    }

    private void ChasePlayer()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh || player == null)
            return;

        agent.isStopped = false;
        agent.SetDestination(player.position);
    }

    private IEnumerator BasicAttackRoutine()
    {
        isAttacking = true;

        StopAgentForAttack();
        LookAtPlayer();
        PlayRandomAttack();

        yield return new WaitForSeconds(attackCooldown);

        if (isDead)
        {
            isAttacking = false;
            yield break;
        }

        RestoreAgentAfterAttack();
        isAttacking = false;
    }

    private void PlayRandomAttack()
    {
        if (animator == null)
            return;

        int attackIndex = Random.Range(0, 3);

        switch (attackIndex)
        {
            case 0:
                animator.SetTrigger(AttackAHash);
                break;
            case 1:
                animator.SetTrigger(AttackBHash);
                break;
            default:
                animator.SetTrigger(AttackCHash);
                break;
        }
    }

    public void StartSpecialPattern()
    {
        if (isDead)
            return;

        isSpecialPattern = true;
        StopAgentForAttack();
        LookAtPlayer();
    }

    public void EndSpecialPattern()
    {
        if (isDead)
            return;

        isSpecialPattern = false;
        RestoreAgentAfterAttack();
    }

    private void StopAgentForAttack()
    {
        if (agent == null || !agent.enabled)
            return;

        agent.isStopped = true;

        if (!useRootMotionAttack)
            return;

        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    private void RestoreAgentAfterAttack()
    {
        if (agent == null || !agent.enabled)
            return;

        if (useRootMotionAttack)
        {
            if (agent.isOnNavMesh)
                agent.Warp(transform.position);

            agent.updatePosition = true;
            agent.updateRotation = true;
        }

        if (agent.isOnNavMesh)
            agent.isStopped = false;
    }

    private void LookAtPlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        transform.rotation = Quaternion.LookRotation(direction);
    }

    private void UpdateAnimation()
    {
        if (animator == null || agent == null || !agent.enabled)
            return;

        animator.SetFloat(SpeedHash, agent.velocity.magnitude);
    }
}
