using System.Collections;
using UnityEngine;

public class PlayerStatusEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject burnVfx;

    [Header("Burn")]
    [Min(0f)]
    [SerializeField] private float burnDuration = 3f;

    [Min(0.01f)]
    [SerializeField] private float burnTickInterval = 0.5f;

    [Min(1)]
    [SerializeField] private int burnDamagePerTick = 2;

    private Coroutine burnCoroutine;

    private void Awake()
    {
        if (playerHealth == null)
            playerHealth = GetComponent<PlayerHealth>();

        SetBurnVfx(false);
    }

    public void ApplyBurn()
    {
        if (playerHealth == null ||
            playerHealth.CurrentHP <= 0 ||
            burnDuration <= 0f ||
            burnDamagePerTick <= 0)
        {
            return;
        }

        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);

        burnCoroutine =
            StartCoroutine(BurnRoutine());
    }

    public void ClearAllEffects()
    {
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }

        SetBurnVfx(false);
    }

    private IEnumerator BurnRoutine()
    {
        SetBurnVfx(true);

        float interval =
            Mathf.Max(
                0.01f,
                burnTickInterval
            );

        float elapsed = 0f;

        while (elapsed < burnDuration)
        {
            if (playerHealth == null ||
                playerHealth.CurrentHP <= 0)
            {
                break;
            }

            playerHealth.TakeDamage(
                burnDamagePerTick
            );

            if (playerHealth.CurrentHP <= 0)
                break;

            yield return new WaitForSeconds(
                interval
            );

            elapsed += interval;
        }

        SetBurnVfx(false);
        burnCoroutine = null;
    }

    private void OnDisable()
    {
        ClearAllEffects();
    }

    private void SetBurnVfx(bool active)
    {
        if (burnVfx != null)
            burnVfx.SetActive(active);
    }
}
