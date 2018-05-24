using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingEffectDownload : LoadingEffect
{
    [SerializeField]
    private GameObject m_progressBarRoot;

    [SerializeField]
    private TextMeshProUGUI m_descriptionText;

    private static readonly string PrefabPath = "Prefab/Effect/Common/Loading/LoadingEffectDownload";


    private ProgressBar m_progressBar = null;

    public static LoadingEffectDownload Attach(GameObject parent = null)
    {
        var effect = View.Attach<LoadingEffectDownload>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<LoadingEffectDownload>() failed.");
        return effect;
    }

    void Awake()
    {
        m_progressBar = ProgressBar.Attach(m_progressBarRoot);

        string description = "";
        if (UnityUtil.GetTextTry("common_loading2", ref description))
            m_descriptionText.text = description;
    }

    override public void Progress(float percent)
    {
        if (m_progressBar == null)
            return;

        m_progressBar.Progress(percent);
    }

    override public void ProgressFiles(float current, float max)
    {
        if (m_progressBar == null)
            return;

        m_progressBar.ProgressFiles(current, max);
    }
}
