using UnityEngine;

public static class ItemEffectRunner
{
    public static bool TryApplyUseEffects(
        ItemDataSO item,
        GameObject user)
    {
        if (item == null || user == null)
            return false;

        var effects = item.OnUseEffects;

        if (effects == null || effects.Count == 0)
            return false;

        int validEffectCount = 0;

        // 먼저 모든 효과가 적용 가능한지 확인해
        // 일부 효과만 적용된 뒤 사용이 실패하는 상황을 방지한다.
        for (int i = 0; i < effects.Count; i++)
        {
            ItemEffectSO effect = effects[i];

            if (effect == null)
                continue;

            validEffectCount++;

            if (!effect.CanApply(user))
                return false;
        }

        if (validEffectCount == 0)
            return false;

        for (int i = 0; i < effects.Count; i++)
        {
            ItemEffectSO effect = effects[i];

            if (effect != null)
                effect.Apply(user);
        }

        return true;
    }
}
