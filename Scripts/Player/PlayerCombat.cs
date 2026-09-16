using StarterAssets;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

    [Header("Combo")]
    [SerializeField] private int maxCombo = 3;
    [SerializeField] private float comboBufferTime = 0.6f;

    private StarterAssetsInputs input;
    private ThirdPersonController controller;
    private PlayerCombatState combatState;

    private int comboIndex;
    private bool canAcceptCombo;
    private bool queuedCombo;
    private float bufferTimer;

    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DoComboHash = Animator.StringToHash("DoCombo");
    private static readonly int ComboIndexHash = Animator.StringToHash("Comboindex");

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        input = GetComponentInParent<StarterAssetsInputs>();
        controller = GetComponentInParent<ThirdPersonController>();
        combatState = GetComponentInParent<PlayerCombatState>();
    }

    private void Update()
    {
        if (input == null || controller == null)
            return;

        // 회피 중이거나 전력 질주 중에는 공격 입력을 받지 않는다.
        if (controller._isRolling || controller._speed >= 5f)
            return;

        UpdateComboBuffer();

        if (!input.attack)
            return;

        // 공격 입력은 1회만 처리한다.
        input.attack = false;

        combatState?.EnterCombat();

        if (comboIndex == 0)
        {
            StartCombo();
            return;
        }

        QueueCombo();
    }

    private void UpdateComboBuffer()
    {
        if (bufferTimer <= 0f)
            return;

        bufferTimer -= Time.deltaTime;

        if (bufferTimer <= 0f)
            queuedCombo = false;
    }

    private void StartCombo()
    {
        comboIndex = 1;

        animator.SetInteger(ComboIndexHash, comboIndex);
        animator.SetTrigger(AttackHash);

        queuedCombo = false;
        canAcceptCombo = false;
        bufferTimer = 0f;
    }

    private void QueueCombo()
    {
        if (comboIndex >= maxCombo)
            return;

        queuedCombo = true;
        bufferTimer = comboBufferTime;

        // 콤보 허용 구간이 이미 열려 있다면 즉시 다음 공격으로 진행한다.
        if (canAcceptCombo)
            ConsumeComboIfPossible();
    }

    private void ConsumeComboIfPossible()
    {
        if (!queuedCombo || comboIndex >= maxCombo)
            return;

        comboIndex++;

        animator.SetInteger(ComboIndexHash, comboIndex);
        animator.SetTrigger(DoComboHash);

        queuedCombo = false;
        canAcceptCombo = false;
        bufferTimer = 0f;
    }

    // Animation Event: 다음 공격 입력을 받을 수 있는 구간 시작.
    public void ComboWindowOpen()
    {
        canAcceptCombo = true;

        // 사용자가 미리 입력해 둔 경우 바로 다음 공격으로 진행한다.
        ConsumeComboIfPossible();
    }

    // Animation Event: 콤보 입력 허용 구간 종료.
    public void ComboWindowClose()
    {
        canAcceptCombo = false;
    }

    // Animation Event 또는 Idle 진입 시 콤보 상태 초기화.
    public void ComboReset()
    {
        ResetComboState();
    }

    // StateMachineBehaviour 등 외부 로직에서 강제로 콤보를 초기화할 때 사용.
    public void ForceComboReset()
    {
        ResetComboState();
    }

    private void ResetComboState()
    {
        comboIndex = 0;
        queuedCombo = false;
        canAcceptCombo = false;
        bufferTimer = 0f;

        animator.SetInteger(ComboIndexHash, 0);
    }
}
