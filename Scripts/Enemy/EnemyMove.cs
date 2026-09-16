using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    [Header("Scout")]
    [SerializeField] private float scoutRadius = 6f;
    [SerializeField] private float chooseNewDestination = 3f;
    [SerializeField] private float arriveDistance = 0.6f;
    [SerializeField] private float waitMin = 0.6f;
    [SerializeField] private float waitMax = 1.8f;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private Vector3 origin;
    private float destinationTimer;
    private float waitTimer;

    private static readonly int SpeedHash = Animator.StringToHash("Speed");

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>(true);

        origin = transform.position;
    }

    private void Start()
    {
        PickNewDestination();
        destinationTimer = chooseNewDestination;
    }

    private void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        UpdateAnimation();

        bool arrived =
            !agent.pathPending &&
            agent.remainingDistance <= arriveDistance;

        if (arrived)
        {
            HandleArrival();
            return;
        }

        destinationTimer -= Time.deltaTime;

        if (destinationTimer <= 0f)
        {
            PickNewDestination();
            destinationTimer = chooseNewDestination;
        }
    }

    private void HandleArrival()
    {
        if (waitTimer <= 0f)
            waitTimer = Random.Range(waitMin, waitMax);

        waitTimer -= Time.deltaTime;

        if (waitTimer > 0f)
            return;

        PickNewDestination();
        destinationTimer = chooseNewDestination;
    }

    private void PickNewDestination()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        const int maxAttempts = 12;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPoint = Random.insideUnitCircle * scoutRadius;
            Vector3 candidate =
                origin + new Vector3(randomPoint.x, 0f, randomPoint.y);

            if (NavMesh.SamplePosition(
                    candidate,
                    out NavMeshHit hit,
                    2f,
                    NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
                return;
            }
        }
    }

    private void UpdateAnimation()
    {
        if (animator != null)
            animator.SetFloat(SpeedHash, agent.velocity.magnitude);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 center =
            Application.isPlaying ? origin : transform.position;

        Gizmos.DrawWireSphere(center, scoutRadius);
    }
#endif
}
