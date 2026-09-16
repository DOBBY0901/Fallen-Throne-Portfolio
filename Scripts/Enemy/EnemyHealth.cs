using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHp = 100;

    [Header("Death")]
    [SerializeField] private float despawnDelay = 3f;
    [SerializeField] private AudioClip deathSfx;

    [Range(0f, 1f)]
    [SerializeField] private float deathVolume = 0.9f;

    private int currentHp;
    private bool isDead;

    private EnemyHitReaction hitReaction;
    private EnemyCombatAI combatAI;
    private Animator animator;
    private Collider[] colliders;

    private static readonly int DieHash =
        Animator.StringToHash("Die");

    private static readonly int AttackHash =
        Animator.StringToHash("Attack");

    private static readonly int HitHash =
        Animator.StringToHash("Hit");

    public int CurrentHp => currentHp;
    public int MaxHp => maxHp;

    public event Action<int, int> OnHpChanged;

    private void Awake()
    {
        currentHp = maxHp;

        hitReaction =
            GetComponent<EnemyHitReaction>();

        combatAI =
            GetComponent<EnemyCombatAI>();

        animator =
            GetComponentInChildren<Animator>();

        colliders =
            GetComponentsInChildren<Collider>();
    }

    private void Start()
    {
        OnHpChanged?.Invoke(
            currentHp,
            maxHp
        );
    }

    public void TakeDamage(
        int damage,
        Transform attacker)
    {
        if (isDead || damage <= 0)
            return;

        currentHp =
            Mathf.Clamp(
                currentHp - damage,
                0,
                maxHp
            );

        hitReaction?.PlayHitFeedback();

        OnHpChanged?.Invoke(
            currentHp,
            maxHp
        );

        combatAI?.OnDamage(attacker);

        if (currentHp <= 0)
            Die();
    }

    private void Die()
    {
        if (isDead)
            return;

        isDead = true;

        combatAI?.Die();

        GetComponent<
            EnemyDropToInventory
        >()?.DropOnce();

        PlayDeathSfx();

        if (animator != null)
        {
            animator.ResetTrigger(
                AttackHash
            );

            animator.ResetTrigger(
                HitHash
            );

            animator.SetTrigger(
                DieHash
            );
        }

        if (hitReaction != null)
            hitReaction.enabled = false;

        foreach (Collider enemyCollider
                 in colliders)
        {
            if (enemyCollider != null)
                enemyCollider.enabled = false;
        }

        MinimapEnemyIconManager.Instance
            ?.UnregisterEnemy(transform);

        StartCoroutine(
            DespawnRoutine()
        );
    }

    private void PlayDeathSfx()
    {
        if (deathSfx == null)
            return;

        AudioManager.Instance
            ?.Play3DSfx(
                deathSfx,
                transform.position,
                deathVolume
            );
    }

    private IEnumerator DespawnRoutine()
    {
        yield return new WaitForSeconds(
            despawnDelay
        );

        Destroy(gameObject);
    }
}
