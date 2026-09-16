using System;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private int maxHP = 100;

    [Header("Death")]
    [SerializeField] private AudioClip deathSfx;

    [Range(0f, 1f)]
    [SerializeField] private float deathVolume = 0.9f;

    [Header("References")]
    [SerializeField] private GameObject deathPanel;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerStats playerStats;

    private ThirdPersonController controller;
    private Animator animator;
    private PlayerHitReaction hitReaction;
    private PlayerCombatState combatState;

    private bool isDead;

    private static readonly int DieHash =
        Animator.StringToHash("Die");

    public int CurrentHP { get; private set; }
    public int MaxHP => maxHP;

    public float NormalizedHP =>
        maxHP <= 0
            ? 0f
            : (float)CurrentHP / maxHP;

    public event Action<int, int> OnHpChanged;

    private void Awake()
    {
        CurrentHP = maxHP;

        animator = GetComponentInChildren<Animator>();
        controller = GetComponent<ThirdPersonController>();
        hitReaction = GetComponent<PlayerHitReaction>();
        combatState = GetComponent<PlayerCombatState>();

        if (playerInput == null)
            playerInput = GetComponent<PlayerInput>();

        if (deathPanel != null)
            deathPanel.SetActive(false);
    }

    private void Start()
    {
        OnHpChanged?.Invoke(CurrentHP, maxHP);
    }

    public void Heal(int amount)
    {
        if (isDead || amount <= 0)
            return;

        CurrentHP =
            Mathf.Clamp(CurrentHP + amount, 0, maxHP);

        OnHpChanged?.Invoke(CurrentHP, maxHP);
    }

    public bool TakeDamage(int damage)
    {
        if (isDead)
            return false;

        if (controller != null && controller._isRolling)
            return false;

        int defenseReduction =
            playerStats != null
                ? playerStats.DamageReductionFromDefense
                : 0;

        int finalDamage =
            Mathf.Max(1, damage - defenseReduction);

        CurrentHP =
            Mathf.Clamp(CurrentHP - finalDamage, 0, maxHP);

        OnHpChanged?.Invoke(CurrentHP, maxHP);

        if (CurrentHP <= 0)
        {
            Die();
            return true;
        }

        hitReaction?.PlayHitFeedback(transform.position);
        combatState?.EnterCombat();

        return true;
    }

    public void Die()
    {
        if (isDead)
            return;

        isDead = true;

        if (controller != null)
            controller.enabled = false;

        if (playerInput != null)
            playerInput.enabled = false;

        if (deathSfx != null)
        {
            AudioManager.Instance?.Play3DSfx(
                deathSfx,
                transform.position,
                deathVolume
            );
        }

        if (animator != null)
            animator.SetTrigger(DieHash);

        if (deathPanel != null)
            deathPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void Respawn()
    {
        RespawnManager respawnManager =
            RespawnManager.Instance;

        if (respawnManager == null)
            return;

        Time.timeScale = 1f;

        CharacterController characterController =
            GetComponent<CharacterController>();

        if (characterController != null)
            characterController.enabled = false;

        transform.SetPositionAndRotation(
            respawnManager.GetRespawnPosition(),
            respawnManager.GetRespawnRotation()
        );

        Physics.SyncTransforms();

        respawnManager.ApplyRespawnEnvironment();

        if (characterController != null)
            characterController.enabled = true;

        CurrentHP = maxHP;
        isDead = false;

        OnHpChanged?.Invoke(CurrentHP, maxHP);

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        if (controller != null)
            controller.enabled = true;

        if (playerInput != null)
            playerInput.enabled = true;

        if (deathPanel != null)
            deathPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
