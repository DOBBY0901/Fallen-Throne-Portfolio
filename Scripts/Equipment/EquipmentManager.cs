using System;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [SerializeField] private ItemDatabaseSO database;
    [SerializeField] private Inventory inventory;

    private readonly Dictionary<EquipmentSlot, string> equipped =
        new Dictionary<EquipmentSlot, string>();

    public event Action OnChanged;

    public string GetEquippedId(EquipmentSlot slot)
    {
        return equipped.TryGetValue(slot, out string id)
            ? id
            : null;
    }

    public bool TryEquip(string itemId)
    {
        if (database == null ||
            inventory == null ||
            string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        ItemDataSO data = database.Get(itemId);

        if (data == null ||
            data.Category != ItemCategory.Equipment)
        {
            return false;
        }

        if (inventory.GetTotalCount(itemId) <= 0)
            return false;

        EquipmentSlot slot = data.EquipSlot;
        string oldItemId = GetEquippedId(slot);

        if (oldItemId == itemId)
            return true;

        // 새 장비를 먼저 확보한다.
        if (!inventory.RemoveItem(itemId, 1))
            return false;

        // 기존 장비를 인벤토리로 되돌릴 수 없는 경우 새 장비 제거를 롤백한다.
        if (!string.IsNullOrEmpty(oldItemId))
        {
            bool returnedOldItem =
                inventory.AddItem(database, oldItemId, 1);

            if (!returnedOldItem)
            {
                inventory.AddItem(database, itemId, 1);
                return false;
            }
        }

        equipped[slot] = itemId;
        OnChanged?.Invoke();

        return true;
    }

    public bool Unequip(EquipmentSlot slot)
    {
        string itemId = GetEquippedId(slot);

        if (string.IsNullOrEmpty(itemId) ||
            database == null ||
            inventory == null)
        {
            return false;
        }

        if (!inventory.AddItem(database, itemId, 1))
            return false;

        equipped.Remove(slot);
        OnChanged?.Invoke();

        return true;
    }

    public int GetTotalStat(StatType statType)
    {
        if (database == null)
            return 0;

        int total = 0;

        foreach (string itemId in equipped.Values)
        {
            if (string.IsNullOrEmpty(itemId))
                continue;

            ItemDataSO data = database.Get(itemId);

            if (data == null || data.EquipStats == null)
                continue;

            EquipmentStat[] stats = data.EquipStats;

            for (int i = 0; i < stats.Length; i++)
            {
                if (stats[i].statType == statType)
                    total += stats[i].value;
            }
        }

        return total;
    }
}
