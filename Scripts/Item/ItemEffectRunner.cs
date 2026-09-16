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

        for (int i = 0; i < effects.Count; i++)
        {
            ItemEffectSO effect = effects[i];

            if (effect == null)
                continue;

            // 하나라도 적용에 실패하면 아이템 사용을 실패로 처리한다.
            if (!effect.Apply(user))
                return false;
        }

        return true;
    }
}
