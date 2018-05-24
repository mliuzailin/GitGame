using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using M4u;

public class LoadingEffectGuard : LoadingEffect
{
    [SerializeField]
    private GameObject m_commonEffectRoot;

    private static readonly string PrefabPath = "Prefab/Effect/Common/Loading/LoadingEffectGuard";

    public static LoadingEffectGuard Attach(GameObject parent = null)
    {
        var effect = View.Attach<LoadingEffectGuard>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<LoadingEffectGuard>() failed.");
        return effect;
    }

    void Awake()
    {
        LoadingEffectCommon.Attach(m_commonEffectRoot);
    }
}
