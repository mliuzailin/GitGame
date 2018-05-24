using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingEffectSimple : LoadingEffect
{
    [SerializeField]
    private GameObject m_commonEffectRoot;

    [SerializeField]
    private TextMeshProUGUI m_descriptionText;

    private static readonly string PrefabPath = "Prefab/Effect/Common/Loading/LoadingEffectSimple";

    public static LoadingEffectSimple Attach(GameObject parent = null)
    {
        var effect = View.Attach<LoadingEffectSimple>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<LoadingEffectSimple>() failed.");
        return effect;
    }

    void Awake()
    {
        LoadingEffectCommon.Attach(m_commonEffectRoot);

        string description = "";
        if (UnityUtil.GetTextTry("common_loading2", ref description))
            m_descriptionText.text = description;
    }
}
