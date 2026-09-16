using System.Collections;
using UnityEngine;

public class BossPatternController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossAI bossAI;
    [SerializeField] private Animator animator;
    [SerializeField] private BossSlamAttack slamAttack;
    [SerializeField] private BossRockFallPattern rockFallPattern;
    [SerializeField] private BossPhaseController phaseController;
    [SerializeField] private BossHpUI bossHpUI;
    [SerializeField] private BossHealth bossHealth;

    [Header("HP Pattern Rates")]
    [SerializeField, Range(0f, 1f)] private float firstSlamRate = 0.7f;
    [SerializeField, Range(0f, 1f)] private float flamePhaseRate = 0.5f;
    [SerializeField, Range(0f, 1f)] private float secondSlamRate = 0.3f;

    [Header("Pattern Duration")]
    [SerializeField] private float rockFallDuration = 6f;

    private bool usedFirstSlam;
    private bool usedFlamePhase;
    private bool usedSecondSlam;
    private bool isRunningPattern;
    private bool slamHitDone;

    private Coroutine rockFallCoroutine;

    private static readonly int SlamHash = Animator.StringToHash("Slam");
    private static readonly int IsRockFallHash = Animator.StringToHash("IsRockFall");
    private static readonly int AttackAHash = Animator.StringToHash("AttackA");
    private static readonly int AttackBHash = Animator.StringToHash("AttackB");
    private static readonly int AttackCHash = Animator.StringToHash("AttackC");
    private static readonly int RoarHash = Animator.StringToHash("Roar");

    private void Awake()
    {
        if (bossAI == null)
            bossAI = GetComponent<BossAI>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (slamAttack == null)
            slamAttack = GetComponent<BossSlamAttack>();

        if (rockFallPattern == null)
            rockFallPattern = GetComponent<BossRockFallPattern>();

        if (phaseController == null)
            phaseController = GetComponent<BossPhaseController>();

        if (bossHealth == null)
            bossHealth = GetComponent<BossHealth>();

        if (bossHpUI == null)
            bossHpUI = GetComponent<BossHpUI>();
    }

    public void CheckHp(int currentHp, int maxHp)
    {
        if (maxHp <= 0 || isRunningPattern)
            return;

        float hpRate = (float)currentHp / maxHp;

        if (!usedFirstSlam && hpRate <= firstSlamRate)
        {
            usedFirstSlam = true;
            StartCoroutine(SlamRockFallPatternRoutine());
            return;
        }

        if (!usedFlamePhase && hpRate <= flamePhaseRate)
        {
            usedFlamePhase = true;
            StartCoroutine(FlamePhasePatternRoutine());
            return;
        }

        if (!usedSecondSlam && hpRate <= secondSlamRate)
        {
            usedSecondSlam = true;
            StartCoroutine(SlamRockFallPatternRoutine());
        }
    }

    private IEnumerator SlamRockFallPatternRoutine()
    {
        isRunningPattern = true;

        yield return WaitForBasicAttackEnd();

        BeginSpecialPattern();

        slamHitDone = false;
        slamAttack?.ResetSlamHit();

        ResetAttackTriggers();

        if (animator != null)
        {
            animator.SetBool(IsRockFallHash, false);
            animator.SetTrigger(SlamHash);
        }
    }

    private IEnumerator FlamePhasePatternRoutine()
    {
        isRunningPattern = true;

        yield return WaitForBasicAttackEnd();

        BeginSpecialPattern();
        ResetAttackTriggers();

        if (phaseController != null)
            yield return StartCoroutine(phaseController.EnterFlamePhaseRoutine());

        EndSpecialPattern();
        isRunningPattern = false;
    }

    private IEnumerator WaitForBasicAttackEnd()
    {
        while (bossAI != null && bossAI.IsAttacking)
            yield return null;
    }

    private void BeginSpecialPattern()
    {
        bossAI?.StartSpecialPattern();
        bossHealth?.SetInvincible(true);
        bossHpUI?.SetInvincibleUI(true);
    }

    private void EndSpecialPattern()
    {
        bossHealth?.SetInvincible(false);
        bossHpUI?.SetInvincibleUI(false);
        bossAI?.EndSpecialPattern();
    }

    // Animation Event: Slam 타격 프레임에서 호출한다.
    public void SlamHitEvent()
    {
        if (slamHitDone)
            return;

        slamHitDone = true;
        slamAttack?.SlamHit();
    }

    // Animation Event: Slam 종료 시 낙석 루프를 시작한다.
    public void StartRockFallLoopEvent()
    {
        if (rockFallCoroutine != null)
            return;

        rockFallCoroutine = StartCoroutine(RockFallLoopRoutine());
    }

    private IEnumerator RockFallLoopRoutine()
    {
        if (animator != null)
            animator.SetBool(IsRockFallHash, true);

        rockFallPattern?.StartRockFallPattern(rockFallDuration);

        yield return new WaitForSeconds(rockFallDuration);

        if (animator != null)
            animator.SetBool(IsRockFallHash, false);

        EndSpecialPattern();

        isRunningPattern = false;
        slamHitDone = false;
        rockFallCoroutine = null;
    }

    private void ResetAttackTriggers()
    {
        if (animator == null)
            return;

        animator.ResetTrigger(AttackAHash);
        animator.ResetTrigger(AttackBHash);
        animator.ResetTrigger(AttackCHash);
        animator.ResetTrigger(SlamHash);
        animator.ResetTrigger(RoarHash);
    }
}
