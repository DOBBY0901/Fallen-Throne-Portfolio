using UnityEngine;

public class FlameDamageArea : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag))
            return;

        PlayerStatusEffect statusEffect =
            other.GetComponentInParent<PlayerStatusEffect>();

        statusEffect?.ApplyBurn();
    }
}
