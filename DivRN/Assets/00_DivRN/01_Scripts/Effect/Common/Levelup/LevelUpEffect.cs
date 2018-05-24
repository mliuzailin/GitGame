using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpEffect : View
{
    private static readonly string PrefabPath = "Prefab/Effect/Common/LevelUp/LevelUpEffect";
    private static readonly string AnimationName = "level_up_effect";

    public static LevelUpEffect Attach(GameObject parent = null)
    {
        var effect = View.Attach<LevelUpEffect>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<LevelUpEffect>() failed.");
        return effect;
    }

    public void Show()
    {
        SoundUtil.PlaySE(SEID.SE_MM_B02_RANKUP);
        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_RANKUP);
        PlayAnimation(AnimationName);
    }
}
