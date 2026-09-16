using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public const int SlotCount = 25;

    [Serializable]
    public struct Slot
    {
        public string id;
        public int count;

        public bool IsEmpty =>
            string.IsNullOrEmpty(id) || count <= 0;
    }

    [SerializeField] private Slot[] slots =
        new Slot[SlotCount];

    public IReadOnlyList<Slot> Slots => slots;

    public event Action OnChanged;

    private void Awake()
    {
        EnsureSlotArray();
    }

    private void Reset()
    {
        slots = new Slot[SlotCount];
    }

    public bool AddItem(
        ItemDatabaseSO database,
        string id,
        int amount)
    {
        if (database == null ||
            string.IsNullOrWhiteSpace(id) ||
            amount <= 0)
        {
            return false;
        }

        ItemDataSO data = database.Get(id);

        if (data == null)
            return false;

        if (!CanAddToSlots(slots, data, amount))
            return false;

        AddToSlots(slots, data, amount);
        OnChanged?.Invoke();

        return true;
    }

    public bool CanAddItems(
        IReadOnlyList<(ItemDataSO data, int amount)> items)
    {
        if (items == null)
            return false;

        Slot[] simulatedSlots =
            (Slot[])slots.Clone();

        for (int i = 0; i < items.Count; i++)
        {
            ItemDataSO data = items[i].data;
            int amount = items[i].amount;

            if (data == null || amount <= 0)
                continue;

            if (!CanAddToSlots(
                    simulatedSlots,
                    data,
                    amount))
            {
                return false;
            }

            AddToSlots(
                simulatedSlots,
                data,
                amount
            );
        }

        return true;
    }

    public bool AddItems(
        IReadOnlyList<(ItemDataSO data, int amount)> items)
    {
        if (!CanAddItems(items))
            return false;

        bool changed = false;

        for (int i = 0; i < items.Count; i++)
        {
            ItemDataSO data = items[i].data;
            int amount = items[i].amount;

            if (data == null || amount <= 0)
                continue;

            AddToSlots(slots, data, amount);
            changed = true;
        }

        if (changed)
            OnChanged?.Invoke();

        return true;
    }

    public bool RemoveItem(string id, int amount)
    {
        if (string.IsNullOrWhiteSpace(id) ||
            amount <= 0)
        {
            return false;
        }

        if (GetTotalCount(id) < amount)
            return false;

        int remaining = amount;
        bool needsCompaction = false;

        for (int i = 0;
             i < slots.Length && remaining > 0;
             i++)
        {
            if (slots[i].IsEmpty ||
                slots[i].id != id)
            {
                continue;
            }

            int take =
                Mathf.Min(slots[i].count, remaining);

            slots[i].count -= take;
            remaining -= take;

            if (slots[i].count <= 0)
            {
                ClearSlot(i);
                needsCompaction = true;
            }
        }

        if (needsCompaction)
            CompactSlots();

        OnChanged?.Invoke();
        return true;
    }

    public bool UseItem(string id, int amount = 1)
    {
        return RemoveItem(id, amount);
    }

    public int GetTotalCount(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return 0;

        int total = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty &&
                slots[i].id == id)
            {
                total += slots[i].count;
            }
        }

        return total;
    }

    private static bool CanAddToSlots(
        Slot[] targetSlots,
        ItemDataSO data,
        int amount)
    {
        int capacity = 0;

        for (int i = 0; i < targetSlots.Length; i++)
        {
            if (targetSlots[i].IsEmpty)
            {
                capacity += data.Stackable
                    ? data.MaxStack
                    : 1;

                if (capacity >= amount)
                    return true;

                continue;
            }

            if (data.Stackable &&
                targetSlots[i].id == data.Id)
            {
                capacity += Mathf.Max(
                    0,
                    data.MaxStack -
                    targetSlots[i].count
                );

                if (capacity >= amount)
                    return true;
            }
        }

        return capacity >= amount;
    }

    private static void AddToSlots(
        Slot[] targetSlots,
        ItemDataSO data,
        int amount)
    {
        int remaining = amount;

        if (data.Stackable)
        {
            for (int i = 0;
                 i < targetSlots.Length &&
                 remaining > 0;
                 i++)
            {
                if (targetSlots[i].IsEmpty ||
                    targetSlots[i].id != data.Id)
                {
                    continue;
                }

                int space =
                    data.MaxStack -
                    targetSlots[i].count;

                if (space <= 0)
                    continue;

                int add =
                    Mathf.Min(space, remaining);

                targetSlots[i].count += add;
                remaining -= add;
            }
        }

        for (int i = 0;
             i < targetSlots.Length &&
             remaining > 0;
             i++)
        {
            if (!targetSlots[i].IsEmpty)
                continue;

            int add = data.Stackable
                ? Mathf.Min(
                    data.MaxStack,
                    remaining
                )
                : 1;

            targetSlots[i].id = data.Id;
            targetSlots[i].count = add;

            remaining -= add;
        }
    }

    private void CompactSlots()
    {
        int writeIndex = 0;

        for (int readIndex = 0;
             readIndex < slots.Length;
             readIndex++)
        {
            if (slots[readIndex].IsEmpty)
                continue;

            if (readIndex != writeIndex)
            {
                slots[writeIndex] =
                    slots[readIndex];

                ClearSlot(readIndex);
            }

            writeIndex++;
        }
    }

    private void ClearSlot(int index)
    {
        slots[index].id = null;
        slots[index].count = 0;
    }

    private void EnsureSlotArray()
    {
        if (slots != null &&
            slots.Length == SlotCount)
        {
            return;
        }

        Slot[] resized =
            new Slot[SlotCount];

        if (slots != null)
        {
            int copyCount =
                Mathf.Min(
                    slots.Length,
                    SlotCount
                );

            Array.Copy(
                slots,
                resized,
                copyCount
            );
        }

        slots = resized;
    }
}
