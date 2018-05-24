using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using M4u;

public class LoadingEffectHTTPConnect : LoadingEffect
{
    [SerializeField]
    private GameObject m_commonEffectRoot;

    private static readonly string PrefabPath = "Prefab/Effect/Common/Loading/LoadingEffectHTTPConnect";

    public static LoadingEffectHTTPConnect Attach(GameObject parent = null)
    {
        var effect = View.Attach<LoadingEffectHTTPConnect>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<LoadingEffectHTTPConnect>() failed.");
        return effect;
    }

    void Awake()
    {
        LoadingEffectCommon.Attach(m_commonEffectRoot);
    }
}
