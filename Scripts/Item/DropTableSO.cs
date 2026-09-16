using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DropEntry
{
    public ItemDataSO item;

    [Range(0f, 1f)]
    public float chance = 1f;

    [Min(1)]
    public int minAmount = 1;

    [Min(1)]
    public int maxAmount = 1;
}

[CreateAssetMenu(menuName = "Items/Drop Table")]
public class DropTableSO : ScriptableObject
{
    [SerializeField] private List<DropEntry> drops = new List<DropEntry>();

    public List<(ItemDataSO data, int amount)> Roll()
    {
        var result = new List<(ItemDataSO data, int amount)>();

        foreach (DropEntry entry in drops)
        {
            if (entry == null || entry.item == null)
                continue;

            if (Random.value > entry.chance)
                continue;

            int min = Mathf.Min(entry.minAmount, entry.maxAmount);
            int max = Mathf.Max(entry.minAmount, entry.maxAmount);

            int amount = Random.Range(min, max + 1);

            if (amount > 0)
                result.Add((entry.item, amount));
        }

        return result;
    }
}
