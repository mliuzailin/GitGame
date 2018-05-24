using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconSkillUpEffect : View
{
    private static readonly string PrefabPath = "Prefab/Effect/Common/LevelUp/IconSkillUpEffect";
    private static readonly string AnimationName = "icon_skill_up_effect";

    public static IconSkillUpEffect Attach(GameObject parent = null)
    {
        var effect = View.Attach<IconSkillUpEffect>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<IconSkillUpEffect>() failed.");
        return effect;
    }

    public IconSkillUpEffect Show()
    {
        SoundUtil.PlaySE(SEID.VOICE_INGAME_MM_SKILLUP);
        PlayAnimation(AnimationName);
        return this;
    }
}
