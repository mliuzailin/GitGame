using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingEffectToBattle : LoadingEffect
{
    [SerializeField]
    private GameObject m_commonEffectRoot = null;
    [SerializeField]
    private TextMeshProUGUI m_questNameText = null;
    [SerializeField]
    private TextMeshProUGUI m_descriptionText;

    private static readonly string PrefabPath = "Prefab/Effect/Common/Loading/LoadingEffectToBattle";

    public static LoadingEffectToBattle Attach(GameObject parent = null)
    {
        var effect = View.Attach<LoadingEffectToBattle>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<LoadingEffectToBattle>() failed.");
        return effect;
    }

    void Awake()
    {
        if (m_commonEffectRoot != null)
            LoadingEffectCommon.Attach(m_commonEffectRoot);

        string description = "";
        if (UnityUtil.GetTextTry("common_loading2", ref description))
            m_descriptionText.text = description;
    }

    override public void SetText(string text)
    {
        if (m_questNameText == null)
            return;

        m_questNameText.text = text;
    }
}
