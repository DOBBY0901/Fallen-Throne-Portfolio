using UnityEngine;

[CreateAssetMenu(menuName = "Items/Effects/Heal HP Percent")]
public class HealHpPercentEffectSO : ItemEffectSO
{
    [Range(0f, 1f)]
    [SerializeField] private float healHpPercent;

    public override bool Apply(GameObject user)
    {
        if (user == null)
            return false;

        PlayerHealth playerHealth = user.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return false;

        if (playerHealth.CurrentHP <= 0)
            return false;

        if (playerHealth.CurrentHP >= playerHealth.MaxHP)
            return false;

        int healAmount =
            Mathf.RoundToInt(playerHealth.MaxHP * healHpPercent);

        if (healAmount <= 0)
            return false;

        playerHealth.Heal(healAmount);
        return true;
    }
}
