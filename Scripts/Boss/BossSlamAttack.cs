using UnityEngine;

public class BossSlamAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossPhaseController phaseController;

    [Header("Attack")]
    [SerializeField] private Transform slamPoint;
    [SerializeField] private float slamRadius = 4f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Damage")]
    [SerializeField] private int slamDamage = 30;
    [SerializeField] private float knockbackPower = 8f;

    [Header("VFX")]
    [SerializeField] private GameObject normalSlamVfxPrefab;
    [SerializeField] private GameObject flameSlamVfxPrefab;
    [SerializeField] private float slamVfxLifeTime = 3f;

    [Header("SFX")]
    [SerializeField] private AudioClip normalSlamSfx;
    [SerializeField] private AudioClip flameSlamSfx;

    private readonly Collider[] hitResults = new Collider[8];
    private bool hasHit;

    // Animation Event에서 호출한다.
    public void SlamHit()
    {
        if (hasHit || slamPoint == null)
            return;

        hasHit = true;

        bool isFlamePhase =
            phaseController != null && phaseController.IsFlamePhase;

        PlaySlamVfx(isFlamePhase);
        PlaySlamSfx(isFlamePhase);
        ApplySlamDamage(isFlamePhase);
    }

    public void ResetSlamHit()
    {
        hasHit = false;
    }

    private void PlaySlamVfx(bool isFlamePhase)
    {
        GameObject selectedVfx =
            isFlamePhase ? flameSlamVfxPrefab : normalSlamVfxPrefab;

        if (selectedVfx == null)
            return;

        GameObject vfx = Instantiate(
            selectedVfx,
            slamPoint.position,
            Quaternion.identity
        );

        Destroy(vfx, slamVfxLifeTime);
    }

    private void PlaySlamSfx(bool isFlamePhase)
    {
        AudioClip selectedSfx =
            isFlamePhase ? flameSlamSfx : normalSlamSfx;

        if (selectedSfx != null)
            AudioManager.Instance?.Play3DSfx(
                selectedSfx,
                slamPoint.position
            );
    }

    private void ApplySlamDamage(bool isFlamePhase)
    {
        int count = Physics.OverlapSphereNonAlloc(
            slamPoint.position,
            slamRadius,
            hitResults,
            playerLayer,
            QueryTriggerInteraction.Ignore
        );

        for (int i = 0; i < count; i++)
        {
            PlayerHealth playerHealth =
                hitResults[i].GetComponentInParent<PlayerHealth>();

            if (playerHealth == null)
                continue;

            bool damaged = playerHealth.TakeDamage(slamDamage);

            if (!damaged)
                break;

            ApplyKnockback(playerHealth.transform);

            if (isFlamePhase)
                playerHealth.GetComponent<PlayerStatusEffect>()?.ApplyBurn();

            break;
        }
    }

    private void ApplyKnockback(Transform playerTransform)
    {
        PlayerKnockback knockback =
            playerTransform.GetComponent<PlayerKnockback>();

        if (knockback == null)
            return;

        Vector3 direction =
            playerTransform.position - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
            direction.Normalize();

        knockback.ApplyKnockback(direction, knockbackPower);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (slamPoint != null)
            Gizmos.DrawWireSphere(slamPoint.position, slamRadius);
    }
#endif
}
