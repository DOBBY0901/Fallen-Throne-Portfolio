using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/Heal HP Percent")]
public class HealHpPercentEffectSO : ItemEffectSO
{
    [Range(0f, 1f)]
    [SerializeField] private float healHpPercent;

    public override bool CanApply(GameObject user)
    {
        if (!TryGetHealth(user, out PlayerHealth playerHealth))
            return false;

        if (playerHealth.CurrentHP <= 0 ||
            playerHealth.CurrentHP >= playerHealth.MaxHP)
        {
            return false;
        }

        return GetHealAmount(playerHealth) > 0;
    }

    public override void Apply(GameObject user)
    {
        if (!TryGetHealth(user, out PlayerHealth playerHealth))
            return;

        playerHealth.Heal(GetHealAmount(playerHealth));
    }

    private int GetHealAmount(PlayerHealth playerHealth)
    {
        return Mathf.RoundToInt(
            playerHealth.MaxHP * healHpPercent
        );
    }

    private static bool TryGetHealth(
        GameObject user,
        out PlayerHealth playerHealth)
    {
        playerHealth =
            user != null
                ? user.GetComponent<PlayerHealth>()
                : null;

        return playerHealth != null;
    }
}
