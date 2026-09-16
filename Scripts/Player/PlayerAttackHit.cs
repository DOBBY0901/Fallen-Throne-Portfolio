using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackHit : MonoBehaviour
{
    [Header("Hit Area")]
    [SerializeField] private Transform hitOrigin;
    [SerializeField] private float hitRange = 1.8f;
    [SerializeField] private float hitRadius = 0.7f;
    [SerializeField] private LayerMask enemyLayers;

    [Header("Damage")]
    [SerializeField] private int baseDamage = 10;

    [Header("Hit Limit")]
    [SerializeField] private int maxHits = 16;

    [Header("Stats")]
    [SerializeField] private PlayerStats playerStats;

    private readonly Collider[] hitResults =
        new Collider[32];

    private readonly HashSet<EnemyHealth> hitEnemies =
        new HashSet<EnemyHealth>();

    private readonly HashSet<BossHealth> hitBosses =
        new HashSet<BossHealth>();

    private void Awake()
    {
        if (hitOrigin == null)
            hitOrigin = transform;
    }

    public void DoHit()
    {
        Vector3 center =
            hitOrigin.position +
            hitOrigin.forward *
            (hitRange * 0.5f);

        int count =
            Physics.OverlapSphereNonAlloc(
                center,
                hitRadius,
                hitResults,
                enemyLayers,
                QueryTriggerInteraction.Ignore
            );

        if (count <= 0)
            return;

        hitEnemies.Clear();
        hitBosses.Clear();

        int bonusDamage =
            playerStats != null
                ? playerStats
                    .DamageBonusFromAttack
                : 0;

        int finalDamage =
            Mathf.Max(
                1,
                baseDamage + bonusDamage
            );

        int appliedHits = 0;

        for (int i = 0;
             i < count &&
             appliedHits < maxHits;
             i++)
        {
            Collider hit = hitResults[i];

            if (hit == null)
                continue;

            EnemyHealth enemyHealth =
                hit.GetComponentInParent<
                    EnemyHealth
                >();

            if (enemyHealth != null)
            {
                if (hitEnemies.Add(
                        enemyHealth))
                {
                    enemyHealth.TakeDamage(
                        finalDamage,
                        transform
                    );

                    appliedHits++;
                }

                continue;
            }

            BossHealth bossHealth =
                hit.GetComponentInParent<
                    BossHealth
                >();

            if (bossHealth != null &&
                hitBosses.Add(
                    bossHealth))
            {
                bossHealth.TakeDamage(
                    finalDamage
                );

                appliedHits++;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (hitOrigin == null)
            return;

        Vector3 center =
            hitOrigin.position +
            hitOrigin.forward *
            (hitRange * 0.5f);

        Gizmos.DrawWireSphere(
            center,
            hitRadius
        );
    }
#endif
}
