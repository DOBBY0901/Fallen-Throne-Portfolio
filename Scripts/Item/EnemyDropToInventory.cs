using UnityEngine;

public class EnemyDropToInventory : MonoBehaviour
{
    [SerializeField] private DropTableSO dropTable;
    [SerializeField] private ItemDatabaseSO database;
    [SerializeField] private Inventory inventory;

    private bool hasDropped;

    private void Awake()
    {
        ResolveInventory();
    }

    public void DropOnce()
    {
        if (hasDropped)
            return;

        if (dropTable == null || database == null)
            return;

        ResolveInventory();

        if (inventory == null)
            return;

        hasDropped = true;

        var rolledItems = dropTable.Roll();

        foreach (var (data, amount) in rolledItems)
        {
            if (data == null || amount <= 0)
                continue;

            inventory.AddItem(
                database,
                data.Id,
                amount
            );
        }
    }

    private void ResolveInventory()
    {
        if (inventory != null)
            return;

        inventory = FindFirstObjectByType<Inventory>(
            FindObjectsInactive.Include
        );
    }
}
