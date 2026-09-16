using UnityEngine;
using UnityEngine.AI;

public class EnemyCombatAI : MonoBehaviour
{
    private enum State
    {
        Idle,
        Chase,
        Attack
    }

    [Header("State")]
    [SerializeField] private State state = State.Idle;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyMove scout;

    [Header("Detection")]
    [SerializeField] private float detectRange = 10f;
    [SerializeField] private float viewAngle = 90f;
    [SerializeField] private float eyeHeight = 1.6f;
    [SerializeField] private float targetHeight = 1.2f;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask obstacleMask;

    [Header("Combat")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private float faceSpeed = 10f;

    [Header("Hit")]
    [SerializeField] private Transform hitOrigin;
    [SerializeField] private float hitRadius = 0.6f;
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask playerHitMask;

    private readonly Collider[] hitResults = new Collider[8];

    private float lastAttackTime;
    private bool isDead;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);

        if (scout == null)
            scout = GetComponent<EnemyMove>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isDead || player == null || agent == null || animator == null)
            return;

        if (state == State.Idle)
        {
            if (DetectPlayer())
                EnterChase();

            return;
        }

        UpdateMovementAnimation();

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        switch (state)
        {
            case State.Chase:
                UpdateChase(distanceToPlayer);
                break;

            case State.Attack:
                UpdateAttack(distanceToPlayer);
                break;
        }
    }

    private void UpdateChase(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            state = State.Attack;

            if (agent.enabled && agent.isOnNavMesh)
                agent.ResetPath();

            return;
        }

        if (agent.enabled && agent.isOnNavMesh)
            agent.SetDestination(player.position);
    }

    private void UpdateAttack(float distanceToPlayer)
    {
        FacePlayer();

        if (distanceToPlayer > attackRange)
        {
            state = State.Chase;
            return;
        }

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        lastAttackTime = Time.time;
        animator.ResetTrigger(AttackHash);
        animator.SetTrigger(AttackHash);
    }

    private bool DetectPlayer()
    {
        Vector3 eyePosition =
            transform.position + Vector3.up * eyeHeight;

        Vector3 targetPosition =
            player.position + Vector3.up * targetHeight;

        Vector3 direction = targetPosition - eyePosition;
        float distance = direction.magnitude;

        if (distance > detectRange)
            return false;

        float angle = Vector3.Angle(transform.forward, direction);

        if (angle > viewAngle * 0.5f)
            return false;

        int detectionMask = playerMask | obstacleMask;

        if (Physics.Raycast(
                eyePosition,
                direction.normalized,
                out RaycastHit hit,
                distance,
                detectionMask,
                QueryTriggerInteraction.Ignore))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    private void EnterChase()
    {
        if (scout != null)
            scout.enabled = false;

        state = State.Chase;
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * faceSpeed
        );
    }

    private void UpdateMovementAnimation()
    {
        if (agent != null && agent.enabled)
            animator.SetFloat(SpeedHash, agent.velocity.magnitude);
    }

    public void AttackHit()
    {
        if (isDead || player == null)
            return;

        Vector3 center = hitOrigin != null
            ? hitOrigin.position
            : transform.position + transform.forward + Vector3.up;

        int count = Physics.OverlapSphereNonAlloc(
            center,
            hitRadius,
            hitResults,
            playerHitMask,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < count; i++)
        {
            PlayerHealth playerHealth =
                hitResults[i].GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
                continue;

            playerHealth.TakeDamage(damage);
            break;
        }
    }

    public void OnDamage(Transform attacker)
    {
        if (isDead)
            return;

        if (attacker != null)
            player = attacker;

        EnterChase();
    }

    public void ForceChase(Transform target)
    {
        if (isDead || target == null || agent == null)
            return;

        player = target;
        EnterChase();

        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.ResetPath();
            agent.SetDestination(player.position);
        }

        if (animator != null)
            animator.ResetTrigger(AttackHash);
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (agent != null && agent.enabled)
        {
            if (agent.isOnNavMesh)
                agent.ResetPath();

            agent.enabled = false;
        }

        if (scout != null)
            scout.enabled = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Vector3 left =
            Quaternion.Euler(0f, -viewAngle * 0.5f, 0f) * transform.forward;

        Vector3 right =
            Quaternion.Euler(0f, viewAngle * 0.5f, 0f) * transform.forward;

        Gizmos.DrawRay(
            transform.position + Vector3.up * eyeHeight,
            left * detectRange
        );

        Gizmos.DrawRay(
            transform.position + Vector3.up * eyeHeight,
            right * detectRange
        );

        if (hitOrigin != null)
            Gizmos.DrawWireSphere(hitOrigin.position, hitRadius);
    }
#endif
}
