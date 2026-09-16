using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHpUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text bossNameText;
    [SerializeField] private Image hpFillImage;
    [SerializeField] private GameObject lockIcon;

    [Header("Color")]
    [SerializeField] private Color normalHpColor;
    [SerializeField] private Color invincibleHpColor = Color.gray;

    public bool IsBossFightActive { get; private set; }

    private void Awake()
    {
        SetInvincibleUI(false);
    }

    public void Show()
    {
        IsBossFightActive = true;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void EndBossFight()
    {
        IsBossFightActive = false;
        gameObject.SetActive(false);
    }

    public void SetBossName(string bossName)
    {
        if (bossNameText != null)
            bossNameText.text = bossName;
    }

    public void UpdateHp(int currentHp, int maxHp)
    {
        if (hpFillImage == null)
            return;

        hpFillImage.fillAmount =
            maxHp <= 0
                ? 0f
                : Mathf.Clamp01((float)currentHp / maxHp);
    }

    public void SetInvincibleUI(bool isInvincible)
    {
        if (hpFillImage != null)
        {
            hpFillImage.color =
                isInvincible
                    ? invincibleHpColor
                    : normalHpColor;
        }

        if (lockIcon != null)
            lockIcon.SetActive(isInvincible);
    }
}
