using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using M4u;

public class LoadingEffectCommon : LoadingEffect
{
    private static readonly string PrefabPath = "Prefab/Effect/Common/Loading/LoadingEffectCommon";

    public static LoadingEffectCommon Attach(GameObject parent = null)
    {
        var effect = View.Attach<LoadingEffectCommon>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<LoadingEffectCommon>() failed.");
        return effect;
    }
}
