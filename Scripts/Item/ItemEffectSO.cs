using UnityEngine;

public abstract class ItemEffectSO : ScriptableObject
{
    public abstract bool CanApply(GameObject user);
    public abstract void Apply(GameObject user);
}
