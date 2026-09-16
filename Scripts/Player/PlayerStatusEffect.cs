using System.Collections;
using UnityEngine;

public class PlayerStatusEffect : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject burnVfx;

    [Header("Burn")]
    [SerializeField] private float burnDuration = 3f;
    [SerializeField] private float burnTickInterval = 0.5f;
    [SerializeField] private int burnDamagePerTick = 2;

    private Coroutine burnCoroutine;

    private void Awake()
    {
        if (burnVfx != null)
            burnVfx.SetActive(false);
    }

    public void ApplyBurn()
    {
        if (burnCoroutine != null)
            StopCoroutine(burnCoroutine);

        burnCoroutine = StartCoroutine(BurnRoutine());
    }

    private IEnumerator BurnRoutine()
    {
        if (burnVfx != null)
            burnVfx.SetActive(true);

        float elapsed = 0f;

        while (elapsed < burnDuration)
        {
            if (playerHealth != null)
                playerHealth.TakeDamage(burnDamagePerTick);

            yield return new WaitForSeconds(burnTickInterval);
            elapsed += burnTickInterval;
        }

        if (burnVfx != null)
            burnVfx.SetActive(false);

        burnCoroutine = null;
    }

    private void OnDisable()
    {
        if (burnCoroutine != null)
        {
            StopCoroutine(burnCoroutine);
            burnCoroutine = null;
        }

        if (burnVfx != null)
            burnVfx.SetActive(false);
    }
}
