using System;

public enum EquipmentSlot
{
    Helmet,
    Armor,
    Gloves,
    Shoes,
    Weapon
}

public enum StatType
{
    Attack,
    Defense,
    MoveSpeed
}

[Serializable]
public struct EquipmentStat
{
    public StatType statType;
    public int value;
}
