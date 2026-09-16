using UnityEngine;

public class BossBasicAttack : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossPhaseController phaseController;

    [Header("Attack")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 3f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Damage")]
    [SerializeField] private int damage = 15;
    [SerializeField] private float knockbackPower = 5f;

    private readonly Collider[] hitResults = new Collider[8];

    // Animation Event에서 호출한다.
    public void BossBasicAttackHit()
    {
        if (attackPoint == null)
            return;

        bool isFlamePhase =
            phaseController != null && phaseController.IsFlamePhase;

        int count = Physics.OverlapSphereNonAlloc(
            attackPoint.position,
            attackRadius,
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

            bool damaged = playerHealth.TakeDamage(damage);

            if (!damaged)
                break;

            ApplyKnockback(playerHealth.transform);

            if (isFlamePhase)
                ApplyBurn(playerHealth.gameObject);

            // 싱글 플레이 기준으로 같은 공격이 여러 Collider에 중복 적용되지 않도록 종료한다.
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

    private static void ApplyBurn(GameObject playerObject)
    {
        PlayerStatusEffect status =
            playerObject.GetComponent<PlayerStatusEffect>();

        status?.ApplyBurn();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
#endif
}
