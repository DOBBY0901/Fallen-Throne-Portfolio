using System.Collections.Generic;
using UnityEngine;

public enum ItemCategory
{
    Consumable,
    Equipment,
    Material,
    Junk
}

public enum ItemGrade
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

[CreateAssetMenu(menuName = "Items/Item Data")]
public class ItemDataSO : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;
    [SerializeField] private string displayName;

    [TextArea]
    [SerializeField] private string description;

    [SerializeField] private string effectDescription;
    [SerializeField] private Sprite icon;

    [Header("Category")]
    [SerializeField] private ItemCategory category;
    [SerializeField] private ItemGrade grade = ItemGrade.Common;

    [Header("Stack")]
    [SerializeField] private bool stackable = true;

    [Min(1)]
    [SerializeField] private int maxStack = 99;

    [Header("Use Feedback")]
    [SerializeField] private AudioClip useSfx;

    [Range(0f, 1f)]
    [SerializeField] private float useSfxVolume = 1f;

    [SerializeField] private GameObject useVfxPrefab;
    [SerializeField] private float useCooldown;

    [Header("Equipment")]
    [SerializeField] private EquipmentSlot equipSlot;
    [SerializeField] private EquipmentStat[] equipStats;

    [Header("Use Effects")]
    [SerializeField] private List<ItemEffectSO> onUseEffects = new List<ItemEffectSO>();

    public string Id => id;
    public string DisplayName => displayName;
    public string Description => description;
    public string EffectDescription => effectDescription;
    public Sprite Icon => icon;

    public ItemCategory Category => category;
    public ItemGrade Grade => grade;

    public bool Stackable => stackable;
    public int MaxStack => maxStack;

    public AudioClip UseSfx => useSfx;
    public float UseSfxVolume => useSfxVolume;
    public GameObject UseVfxPrefab => useVfxPrefab;
    public float UseCooldown => useCooldown;

    public EquipmentSlot EquipSlot => equipSlot;
    public EquipmentStat[] EquipStats => equipStats;

    public IReadOnlyList<ItemEffectSO> OnUseEffects => onUseEffects;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (!string.IsNullOrEmpty(id))
            id = id.Trim();

        if (!stackable)
            maxStack = 1;
    }
#endif
}
