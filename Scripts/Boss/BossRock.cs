using UnityEngine;

public class BossRock : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private float knockbackPower = 8f;
    [SerializeField] private bool applyBurn;

    private GameObject warningObject;
    private bool hasCollided;

    public void SetWarningObject(GameObject warning)
    {
        warningObject = warning;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided)
            return;

        hasCollided = true;
        ClearWarning();

        PlayerHealth playerHealth =
            collision.gameObject.GetComponentInParent<PlayerHealth>();

        if (playerHealth != null)
        {
            bool damaged = playerHealth.TakeDamage(damageAmount);

            if (damaged)
            {
                ApplyKnockback(playerHealth.transform);

                if (applyBurn)
                    playerHealth.GetComponent<PlayerStatusEffect>()?.ApplyBurn();
            }
        }

        Destroy(gameObject);
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

    private void ClearWarning()
    {
        if (warningObject == null)
            return;

        Destroy(warningObject);
        warningObject = null;
    }

    private void OnDestroy()
    {
        ClearWarning();
    }
}
