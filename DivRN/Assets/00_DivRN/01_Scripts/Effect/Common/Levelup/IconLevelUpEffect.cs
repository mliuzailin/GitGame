using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconLevelUpEffect : View
{
    private static readonly string PrefabPath = "Prefab/Effect/Common/LevelUp/IconLevelUpEffect";
    private static readonly string AnimationName = "icon_level_up_effect";

    public static IconLevelUpEffect Attach(GameObject parent = null)
    {
        var effect = View.Attach<IconLevelUpEffect>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<IconLevelUpEffect>() failed.");
        return effect;
    }

    public IconLevelUpEffect Show()
    {
        SoundUtil.PlaySE(SEID.SE_MM_D04_LEVEL_UP);
        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_LEVELUP);
        PlayAnimation(AnimationName);
        return this;
    }
}
