using System;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Base Stats")]
    [SerializeField] private int baseAttack;
    [SerializeField] private int baseDefense;
    [SerializeField] private int baseMoveSpeed;

    [Header("References")]
    [SerializeField] private EquipmentManager equipment;

    public event Action OnStatsChanged;

    public int FinalAttack =>
        baseAttack + GetEquipmentStat(StatType.Attack);

    public int FinalDefense =>
        baseDefense + GetEquipmentStat(StatType.Defense);

    public int FinalMoveSpeed =>
        baseMoveSpeed + GetEquipmentStat(StatType.MoveSpeed);

    // 공격력 5당 추가 피해 +1
    public int DamageBonusFromAttack => FinalAttack / 5;

    // 방어력 5당 받는 피해 -1
    public int DamageReductionFromDefense => FinalDefense / 5;

    // 이동속도 스탯 1당 이동속도 1% 증가
    public float MoveSpeedMultiplier =>
        Mathf.Max(0.1f, 1f + (FinalMoveSpeed * 0.01f));

    private void OnEnable()
    {
        if (equipment != null)
            equipment.OnChanged += HandleEquipmentChanged;
    }

    private void OnDisable()
    {
        if (equipment != null)
            equipment.OnChanged -= HandleEquipmentChanged;
    }

    private int GetEquipmentStat(StatType statType)
    {
        return equipment != null
            ? equipment.GetTotalStat(statType)
            : 0;
    }

    private void HandleEquipmentChanged()
    {
        OnStatsChanged?.Invoke();
    }
}
