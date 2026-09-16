using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlotGameInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private QuickSlotManager quickSlot;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Inventory inventory;
    [SerializeField] private ItemDatabaseSO database;

    [Header("Feedback")]
    [SerializeField] private Transform vfxSpawnPoint;

    [Header("Options")]
    [SerializeField] private bool clearSlotWhenEmpty;

    private readonly Dictionary<string, float> nextUseTimeById =
        new Dictionary<string, float>();

    private readonly Dictionary<string, float> cooldownDurationById =
        new Dictionary<string, float>();

    private void Update()
    {
        if (quickSlot == null)
            return;

        if (menuManager != null && menuManager.IsOpen)
            return;

        HandleScrollInput();
        HandleUseInput();
    }

    public bool TryGetCooldown(
        string id,
        out float remaining,
        out float duration)
    {
        remaining = 0f;
        duration = 0f;

        if (string.IsNullOrEmpty(id))
            return false;

        if (!nextUseTimeById.TryGetValue(id, out float nextUseTime))
            return false;

        if (!cooldownDurationById.TryGetValue(id, out duration))
            return false;

        remaining = Mathf.Max(0f, nextUseTime - Time.time);

        return remaining > 0f && duration > 0f;
    }

    private void HandleScrollInput()
    {
        if (Mouse.current == null)
            return;

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll > 0.01f)
            quickSlot.ScrollSelect(-1);
        else if (scroll < -0.01f)
            quickSlot.ScrollSelect(1);
    }

    private void HandleUseInput()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.eKey.wasPressedThisFrame)
        {
            return;
        }

        TryUseCurrent();
    }

    private void TryUseCurrent()
    {
        if (inventory == null ||
            database == null ||
            playerHealth == null)
        {
            return;
        }

        string id = quickSlot.GetCurrentItemId();

        if (string.IsNullOrEmpty(id))
            return;

        ItemDataSO data = database.Get(id);

        if (data == null || data.Category != ItemCategory.Consumable)
            return;

        if (inventory.GetTotalCount(id) <= 0)
        {
            ClearCurrentSlotIfNeeded(id);
            return;
        }

        if (IsOnCooldown(id))
            return;

        bool applied = ItemEffectRunner.TryApplyUseEffects(
            data,
            playerHealth.gameObject
        );

        if (!applied)
            return;

        if (!inventory.UseItem(id, 1))
            return;

        PlayUseFeedback(data);
        StartCooldown(id, data.UseCooldown);

        ClearCurrentSlotIfNeeded(id);
    }

    private bool IsOnCooldown(string id)
    {
        return nextUseTimeById.TryGetValue(id, out float nextUseTime)
            && Time.time < nextUseTime;
    }

    private void StartCooldown(string id, float cooldown)
    {
        if (cooldown <= 0f)
            return;

        nextUseTimeById[id] = Time.time + cooldown;
        cooldownDurationById[id] = cooldown;
    }

    private void ClearCurrentSlotIfNeeded(string id)
    {
        if (!clearSlotWhenEmpty)
            return;

        if (inventory.GetTotalCount(id) <= 0)
            quickSlot.Clear(quickSlot.CurrentIndex);
    }

    private void PlayUseFeedback(ItemDataSO data)
    {
        if (data.UseSfx != null)
        {
            AudioManager.Instance?.Play2DSfx(
                data.UseSfx,
                data.UseSfxVolume
            );
        }

        if (data.UseVfxPrefab == null)
            return;

        Transform parent =
            vfxSpawnPoint != null
                ? vfxSpawnPoint
                : playerHealth.transform;

        GameObject vfx = Instantiate(
            data.UseVfxPrefab,
            parent.position,
            parent.rotation,
            parent
        );

        vfx.transform.localPosition = Vector3.zero;
        vfx.transform.localRotation = Quaternion.identity;
    }
}
