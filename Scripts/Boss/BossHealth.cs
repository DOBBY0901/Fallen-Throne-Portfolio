using System;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHp = 1000;

    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private BossAI bossAI;
    [SerializeField] private BossHpUI bossHpUI;
    [SerializeField] private BossPatternController patternController;
    [SerializeField] private string bossName;

    [Header("Death")]
    [SerializeField] private AudioClip deathSfx;

    private int currentHp;
    private bool isDead;
    private bool isInvincible;

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;
    public bool IsDead => isDead;
    public bool IsInvincible => isInvincible;

    public event Action<int, int> OnHpChanged;

    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int AttackAHash = Animator.StringToHash("AttackA");
    private static readonly int AttackBHash = Animator.StringToHash("AttackB");
    private static readonly int AttackCHash = Animator.StringToHash("AttackC");

    private void Awake()
    {
        currentHp = maxHp;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (bossAI == null)
            bossAI = GetComponent<BossAI>();

        if (patternController == null)
            patternController = GetComponent<BossPatternController>();
    }

    private void Start()
    {
        if (bossHpUI != null)
        {
            bossHpUI.SetBossName(bossName);
            bossHpUI.UpdateHp(currentHp, maxHp);
        }

        OnHpChanged?.Invoke(currentHp, maxHp);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvincible || damage <= 0)
            return;

        currentHp = Mathf.Clamp(currentHp - damage, 0, maxHp);

        OnHpChanged?.Invoke(currentHp, maxHp);
        bossHpUI?.UpdateHp(currentHp, maxHp);

        if (currentHp <= 0)
        {
            Die();
            return;
        }

        patternController?.CheckHp(currentHp, maxHp);
    }

    public void SetInvincible(bool value)
    {
        if (isDead)
            return;

        isInvincible = value;
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;
        isInvincible = false;

        bossAI?.StopBoss();

        if (deathSfx != null)
            AudioManager.Instance?.Play3DSfx(deathSfx, transform.position);

        if (animator != null)
        {
            animator.ResetTrigger(AttackAHash);
            animator.ResetTrigger(AttackBHash);
            animator.ResetTrigger(AttackCHash);
            animator.SetTrigger(DieHash);
        }

        bossHpUI?.EndBossFight();
    }
}
