using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuickSlotHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private QuickSlotManager quickSlot;
    [SerializeField] private ItemDatabaseSO database;
    [SerializeField] private Inventory inventory;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private QuickSlotGameInput quickSlotInput;

    [Header("HUD")]
    [SerializeField] private GameObject hudRoot;
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text countText;
    [SerializeField] private TMP_Text indexText;

    [Header("Cooldown")]
    [SerializeField] private Image cooldownMaskImage;
    [SerializeField] private TMP_Text cooldownText;

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnChanged += RefreshStatic;

        if (quickSlot != null)
            quickSlot.OnChanged += RefreshStatic;

        RefreshStatic();
        SetCooldownVisible(false);
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnChanged -= RefreshStatic;

        if (quickSlot != null)
            quickSlot.OnChanged -= RefreshStatic;

        SetCooldownVisible(false);
    }

    private void LateUpdate()
    {
        bool menuOpen =
            menuManager != null && menuManager.IsOpen;

        if (hudRoot != null)
            hudRoot.SetActive(!menuOpen);

        if (menuOpen)
        {
            SetCooldownVisible(false);
            return;
        }

        if (quickSlot == null)
            return;

        UpdateCooldownUI(quickSlot.GetCurrentItemId());
    }

    private void RefreshStatic()
    {
        if (quickSlot == null ||
            database == null ||
            inventory == null)
        {
            return;
        }

        if (indexText != null)
            indexText.text = (quickSlot.CurrentIndex + 1).ToString();

        string id = quickSlot.GetCurrentItemId();

        if (string.IsNullOrEmpty(id))
        {
            ClearItemDisplay();
            return;
        }

        ItemDataSO data = database.Get(id);

        if (data == null)
        {
            ClearItemDisplay();
            return;
        }

        if (iconImage != null)
        {
            iconImage.sprite = data.Icon;
            iconImage.enabled = data.Icon != null;
        }

        if (countText != null)
        {
            int count = inventory.GetTotalCount(id);
            countText.text = count.ToString();
        }
    }

    private void UpdateCooldownUI(string id)
    {
        if (string.IsNullOrEmpty(id) || quickSlotInput == null)
        {
            SetCooldownVisible(false);
            return;
        }

        if (!quickSlotInput.TryGetCooldown(
                id,
                out float remaining,
                out float duration))
        {
            SetCooldownVisible(false);
            return;
        }

        SetCooldownVisible(true);

        if (cooldownMaskImage != null)
        {
            cooldownMaskImage.fillAmount =
                duration <= 0f
                    ? 0f
                    : Mathf.Clamp01(remaining / duration);
        }

        if (cooldownText != null)
        {
            cooldownText.text =
                remaining >= 1f
                    ? Mathf.CeilToInt(remaining).ToString()
                    : remaining.ToString("0.0");
        }
    }

    private void ClearItemDisplay()
    {
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }

        if (countText != null)
            countText.text = string.Empty;

        SetCooldownVisible(false);
    }

    private void SetCooldownVisible(bool visible)
    {
        if (cooldownMaskImage != null)
            cooldownMaskImage.gameObject.SetActive(visible);

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(visible);
    }
}
