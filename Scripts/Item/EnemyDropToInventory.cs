using UnityEngine;

public class EnemyDropToInventory : MonoBehaviour
{
    [SerializeField] private DropTableSO dropTable;
    [SerializeField] private Inventory inventory;

    private bool hasDropped;

    private void Awake()
    {
        ResolveInventory();
    }

    public void DropOnce()
    {
        if (hasDropped || dropTable == null)
            return;

        ResolveInventory();

        if (inventory == null)
            return;

        hasDropped = true;

        var rolledItems = dropTable.Roll();

        // Auto-loot 방식의 포트폴리오 샘플이다.
        // 전체 보상을 담을 수 없으면 부분 지급하지 않는다.
        inventory.AddItems(rolledItems);
    }

    private void ResolveInventory()
    {
        if (inventory != null)
            return;

        inventory =
            FindFirstObjectByType<Inventory>(
                FindObjectsInactive.Include
            );
    }
}
