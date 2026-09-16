using UnityEngine;

public class PlayerCombatState : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Combat State")]
    [SerializeField] private float exitDelay = 3f;

    private float exitTimer;
    private bool isCombat;

    private static readonly int IsCombatHash = Animator.StringToHash("IsCombat");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (!isCombat)
            return;

        exitTimer -= Time.deltaTime;

        if (exitTimer <= 0f)
            ExitCombat();
    }

    // 공격, 피격 등 전투 행동이 발생했을 때 전투 상태를 갱신한다.
    public void EnterCombat()
    {
        if (!isCombat)
        {
            isCombat = true;

            if (animator != null)
                animator.SetBool(IsCombatHash, true);
        }

        exitTimer = exitDelay;
    }

    public void ExitCombat()
    {
        if (!isCombat)
            return;

        isCombat = false;
        exitTimer = 0f;

        if (animator != null)
            animator.SetBool(IsCombatHash, false);
    }
}
