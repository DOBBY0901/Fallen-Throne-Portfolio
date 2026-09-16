using System.Collections.Generic;
using UnityEngine;

public class EquipmentSelectPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemDatabaseSO database;
    [SerializeField] private EquipmentManager equipment;

    [Header("UI")]
    [SerializeField] private EquipmentPopupSlotUI[] slotUIs;

    private EquipmentSlot currentSlot;

    private void Awake()
    {
        if (slotUIs != null)
        {
            for (int i = 0; i < slotUIs.Length; i++)
                slotUIs[i]?.Bind(OnClickEquip);
        }

        gameObject.SetActive(false);
    }

    public void Open(EquipmentSlot slot)
    {
        currentSlot = slot;

        gameObject.SetActive(true);
        Rebuild();
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void OnClickUnequip()
    {
        if (equipment != null &&
            equipment.Unequip(currentSlot))
        {
            Close();
        }
    }

    private void Rebuild()
    {
        if (inventory == null ||
            database == null ||
            slotUIs == null)
        {
            return;
        }

        List<ItemDataSO> candidates =
            new List<ItemDataSO>();

        HashSet<string> addedIds =
            new HashSet<string>();

        var inventorySlots = inventory.Slots;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            Inventory.Slot slot = inventorySlots[i];

            if (slot.IsEmpty)
                continue;

            ItemDataSO data = database.Get(slot.id);

            if (data == null ||
                data.Category != ItemCategory.Equipment ||
                data.EquipSlot != currentSlot)
            {
                continue;
            }

            // 같은 장비가 여러 슬롯에 있어도 선택 목록에는 한 번만 표시한다.
            if (addedIds.Add(data.Id))
                candidates.Add(data);
        }

        int visibleCount =
            Mathf.Min(candidates.Count, slotUIs.Length);

        for (int i = 0; i < visibleCount; i++)
            slotUIs[i]?.Set(candidates[i]);

        for (int i = visibleCount; i < slotUIs.Length; i++)
            slotUIs[i]?.SetEmpty();
    }

    private void OnClickEquip(string itemId)
    {
        if (equipment != null &&
            equipment.TryEquip(itemId))
        {
            Close();
        }
    }
}
