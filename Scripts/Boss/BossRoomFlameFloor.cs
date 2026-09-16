using UnityEngine;

public class BossRoomFlameFloor : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool applyBurn = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !applyBurn)
            return;

        PlayerStatusEffect playerStatus =
            other.GetComponentInParent<PlayerStatusEffect>();

        playerStatus?.ApplyBurn();
    }
}
