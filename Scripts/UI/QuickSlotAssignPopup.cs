using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlotAssignPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject root;
    [SerializeField] private QuickSlotManager quickSlot;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private ItemDatabaseSO database;
    [SerializeField] private Inventory inventory;

    [Header("Slot UI")]
    [SerializeField] private QuickSlotUI[] slotUIs =
        new QuickSlotUI[QuickSlotManager.SlotCount];

    public bool IsOpen =>
        root != null && root.activeSelf;

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void Update()
    {
        if (!IsOpen || Keyboard.current == null)
            return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) Assign(0);
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) Assign(1);
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) Assign(2);
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) Assign(3);
        else if (Keyboard.current.digit5Key.wasPressedThisFrame) Assign(4);
    }

    public void OpenPopup()
    {
        if (root == null)
            return;

        root.SetActive(true);
        RefreshPopupSlots();
    }

    public void Close()
    {
        if (root != null)
            root.SetActive(false);
    }

    private void Assign(int slotIndex)
    {
        if (inventoryUI == null ||
            database == null ||
            quickSlot == null)
        {
            return;
        }

        string id = inventoryUI.GetSelectedItemId();

        if (string.IsNullOrEmpty(id))
        {
            Close();
            return;
        }

        ItemDataSO data = database.Get(id);

        if (data == null ||
            data.Category != ItemCategory.Consumable)
        {
            return;
        }

        quickSlot.SetItemId(slotIndex, id);
        RefreshPopupSlots();
    }

    private void RefreshPopupSlots()
    {
        for (int i = 0; i < QuickSlotManager.SlotCount; i++)
        {
            if (i >= slotUIs.Length || slotUIs[i] == null)
                continue;

            string id =
                quickSlot != null
                    ? quickSlot.GetItemId(i)
                    : null;

            if (string.IsNullOrEmpty(id))
            {
                slotUIs[i].SetEmpty();
                continue;
            }

            ItemDataSO data =
                database != null
                    ? database.Get(id)
                    : null;

            if (data == null)
            {
                slotUIs[i].SetEmpty();
                continue;
            }

            int count =
                inventory != null
                    ? inventory.GetTotalCount(id)
                    : 0;

            slotUIs[i].Set(data.Icon, count);
        }
    }
}
