using System;
using UnityEngine;

public class QuickSlotManager : MonoBehaviour
{
    public const int SlotCount = 5;

    [SerializeField] private string[] slotItemIds = new string[SlotCount];

    public int CurrentIndex { get; private set; }

    public event Action OnChanged;

    private void Awake()
    {
        EnsureSlotArray();
    }

    public string GetItemId(int slotIndex)
    {
        if (!IsValidIndex(slotIndex))
            return null;

        return slotItemIds[slotIndex];
    }

    public string GetCurrentItemId()
    {
        return GetItemId(CurrentIndex);
    }

    public void SetItemId(int slotIndex, string itemId)
    {
        if (!IsValidIndex(slotIndex))
            return;

        slotItemIds[slotIndex] = itemId;
        OnChanged?.Invoke();
    }

    public void Clear(int slotIndex)
    {
        if (!IsValidIndex(slotIndex))
            return;

        slotItemIds[slotIndex] = null;
        OnChanged?.Invoke();
    }

    public void ScrollSelect(int direction)
    {
        if (direction == 0)
            return;

        CurrentIndex =
            (CurrentIndex + direction + SlotCount) % SlotCount;

        OnChanged?.Invoke();
    }

    private static bool IsValidIndex(int slotIndex)
    {
        return slotIndex >= 0 && slotIndex < SlotCount;
    }

    private void EnsureSlotArray()
    {
        if (slotItemIds != null && slotItemIds.Length == SlotCount)
            return;

        string[] resized = new string[SlotCount];

        if (slotItemIds != null)
        {
            int copyCount = Mathf.Min(
                slotItemIds.Length,
                SlotCount
            );

            Array.Copy(slotItemIds, resized, copyCount);
        }

        slotItemIds = resized;
    }
}
