using UnityEngine;
using UnityEngine.UI;

public class EnemyHpBarUI : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Image fillImage;

    private void Awake()
    {
        if (enemyHealth == null)
            enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    private void OnEnable()
    {
        if (enemyHealth == null)
            return;

        enemyHealth.OnHpChanged += HandleHpChanged;
        HandleHpChanged(enemyHealth.CurrentHp, enemyHealth.MaxHp);
    }

    private void OnDisable()
    {
        if (enemyHealth != null)
            enemyHealth.OnHpChanged -= HandleHpChanged;
    }

    private void HandleHpChanged(int currentHp, int maxHp)
    {
        if (fillImage == null)
            return;

        fillImage.fillAmount =
            maxHp <= 0 ? 0f : (float)currentHp / maxHp;
    }
}
