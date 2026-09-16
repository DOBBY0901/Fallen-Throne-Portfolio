using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Database")]
public class ItemDatabaseSO : ScriptableObject
{
    [SerializeField] private List<ItemDataSO> items = new List<ItemDataSO>();

    private Dictionary<string, ItemDataSO> itemCache;

    public ItemDataSO Get(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return null;

        EnsureCache();

        itemCache.TryGetValue(id, out ItemDataSO data);
        return data;
    }

    public bool Contains(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return false;

        EnsureCache();
        return itemCache.ContainsKey(id);
    }

    private void EnsureCache()
    {
        if (itemCache != null)
            return;

        itemCache = new Dictionary<string, ItemDataSO>(items.Count);

        foreach (ItemDataSO data in items)
        {
            if (data == null || string.IsNullOrWhiteSpace(data.Id))
                continue;

            itemCache[data.Id] = data;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        itemCache = null;

        HashSet<string> idSet = new HashSet<string>();

        foreach (ItemDataSO data in items)
        {
            if (data == null)
                continue;

            if (string.IsNullOrWhiteSpace(data.Id))
            {
                Debug.LogWarning(
                    $"[ItemDatabase] Empty ID: {data.name}",
                    this
                );

                continue;
            }

            if (!idSet.Add(data.Id))
            {
                Debug.LogError(
                    $"[ItemDatabase] Duplicate ID: '{data.Id}'",
                    this
                );
            }
        }
    }
#endif
}
