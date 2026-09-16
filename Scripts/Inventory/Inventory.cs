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

    [SerializeField] private Slot[] slots = new Slot[SlotCount];

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

        // 전체 수량을 담을 공간이 없으면 인벤토리를 변경하지 않는다.
        if (GetAvailableCapacity(data) < amount)
            return false;

        int remaining = amount;

        if (data.Stackable)
            remaining = FillExistingStacks(data, remaining);

        if (remaining > 0)
            FillEmptySlots(data, remaining);

        OnChanged?.Invoke();
        return true;
    }

    public bool RemoveItem(string id, int amount)
    {
        if (string.IsNullOrWhiteSpace(id) || amount <= 0)
            return false;

        // 일부만 제거되는 상황을 방지한다.
        if (GetTotalCount(id) < amount)
            return false;

        int remaining = amount;
        bool needsCompaction = false;

        for (int i = 0; i < slots.Length && remaining > 0; i++)
        {
            if (slots[i].IsEmpty || slots[i].id != id)
                continue;

            int take = Mathf.Min(slots[i].count, remaining);

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
            if (!slots[i].IsEmpty && slots[i].id == id)
                total += slots[i].count;
        }

        return total;
    }

    private int FillExistingStacks(
        ItemDataSO data,
        int remaining)
    {
        for (int i = 0; i < slots.Length && remaining > 0; i++)
        {
            if (slots[i].IsEmpty || slots[i].id != data.Id)
                continue;

            int space = data.MaxStack - slots[i].count;

            if (space <= 0)
                continue;

            int add = Mathf.Min(space, remaining);

            slots[i].count += add;
            remaining -= add;
        }

        return remaining;
    }

    private void FillEmptySlots(
        ItemDataSO data,
        int amount)
    {
        int remaining = amount;

        for (int i = 0; i < slots.Length && remaining > 0; i++)
        {
            if (!slots[i].IsEmpty)
                continue;

            int add = data.Stackable
                ? Mathf.Min(data.MaxStack, remaining)
                : 1;

            slots[i].id = data.Id;
            slots[i].count = add;

            remaining -= add;
        }
    }

    private int GetAvailableCapacity(ItemDataSO data)
    {
        int capacity = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
            {
                capacity += data.Stackable
                    ? data.MaxStack
                    : 1;

                continue;
            }

            if (data.Stackable && slots[i].id == data.Id)
            {
                capacity += Mathf.Max(
                    0,
                    data.MaxStack - slots[i].count
                );
            }
        }

        return capacity;
    }

    private void CompactSlots()
    {
        int writeIndex = 0;

        for (int readIndex = 0; readIndex < slots.Length; readIndex++)
        {
            if (slots[readIndex].IsEmpty)
                continue;

            if (readIndex != writeIndex)
            {
                slots[writeIndex] = slots[readIndex];
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
        if (slots != null && slots.Length == SlotCount)
            return;

        Slot[] resized = new Slot[SlotCount];

        if (slots != null)
        {
            int copyCount = Mathf.Min(slots.Length, SlotCount);
            Array.Copy(slots, resized, copyCount);
        }

        slots = resized;
    }
}
