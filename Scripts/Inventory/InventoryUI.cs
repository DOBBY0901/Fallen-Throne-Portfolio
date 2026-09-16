using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ItemDatabaseSO database;
    [SerializeField] private Inventory inventory;

    [Header("Slots")]
    [SerializeField] private Transform slotGridRoot;

    [Header("Detail Panel")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text effectText;
    [SerializeField] private TMP_Text descriptionText;

    private InventorySlotUI[] slotUIs;
    private int selectedIndex = -1;

    private void Awake()
    {
        if (slotGridRoot != null)
        {
            slotUIs =
                slotGridRoot.GetComponentsInChildren<InventorySlotUI>(true);
        }
        else
        {
            slotUIs = new InventorySlotUI[0];
        }

        for (int i = 0; i < slotUIs.Length; i++)
            slotUIs[i]?.Bind(i, OnClickSlot);

        HideDetailContents();
    }

    private void OnEnable()
    {
        selectedIndex = -1;
        HideDetailContents();

        if (inventory != null)
            inventory.OnChanged += Refresh;

        Refresh();
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnChanged -= Refresh;

        HideDetailContents();
    }

    public void Refresh()
    {
        if (inventory == null || database == null)
            return;

        var slots = inventory.Slots;

        for (int i = 0; i < slotUIs.Length; i++)
        {
            InventorySlotUI slotUI = slotUIs[i];

            if (slotUI == null)
                continue;

            if (i >= slots.Count || slots[i].IsEmpty)
            {
                slotUI.SetEmpty();
                continue;
            }

            ItemDataSO data = database.Get(slots[i].id);

            if (data == null)
            {
                slotUI.SetEmpty();
                continue;
            }

            slotUI.Set(data, slots[i].count);
        }

        // 선택 중이던 슬롯이 변경된 경우 상세 정보도 함께 갱신한다.
        if (selectedIndex >= 0)
            ShowDetail(selectedIndex);
    }

    public string GetSelectedItemId()
    {
        if (inventory == null ||
            selectedIndex < 0 ||
            selectedIndex >= inventory.Slots.Count)
        {
            return null;
        }

        Inventory.Slot slot = inventory.Slots[selectedIndex];

        return slot.IsEmpty ? null : slot.id;
    }

    private void OnClickSlot(int index)
    {
        selectedIndex = index;
        ShowDetail(index);
    }

    private void ShowDetail(int index)
    {
        if (inventory == null ||
            database == null ||
            index < 0 ||
            index >= inventory.Slots.Count)
        {
            HideDetailContents();
            return;
        }

        Inventory.Slot slot = inventory.Slots[index];

        if (slot.IsEmpty)
        {
            HideDetailContents();
            return;
        }

        ItemDataSO data = database.Get(slot.id);

        if (data == null)
        {
            HideDetailContents();
            return;
        }

        if (detailIcon != null)
        {
            detailIcon.sprite = data.Icon;
            detailIcon.enabled = data.Icon != null;
        }

        if (nameText != null)
        {
            nameText.text = data.DisplayName;
            nameText.gameObject.SetActive(true);
        }

        if (effectText != null)
        {
            effectText.text = data.EffectDescription;
            effectText.gameObject.SetActive(true);
        }

        if (descriptionText != null)
        {
            descriptionText.text = data.Description;
            descriptionText.gameObject.SetActive(true);
        }
    }

    private void HideDetailContents()
    {
        if (detailIcon != null)
        {
            detailIcon.sprite = null;
            detailIcon.enabled = false;
        }

        SetTextVisible(nameText, false);
        SetTextVisible(effectText, false);
        SetTextVisible(descriptionText, false);
    }

    private static void SetTextVisible(
        TMP_Text text,
        bool visible)
    {
        if (text == null)
            return;

        if (!visible)
            text.text = string.Empty;

        text.gameObject.SetActive(visible);
    }
}
