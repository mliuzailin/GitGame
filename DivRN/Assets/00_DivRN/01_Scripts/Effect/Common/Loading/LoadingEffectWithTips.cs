using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingEffectWithTips : LoadingEffect
{
    [SerializeField]
    private GameObject m_commonEffectRoot;

    [SerializeField]
    private GameObject m_TipsRoot;

    [SerializeField]
    private TextMeshProUGUI m_descriptionText;


    private static readonly string PrefabPath = "Prefab/Effect/Common/Loading/LoadingEffectWithTips";


    private TipsModel m_tipsModel = null;

    private static readonly float PagingInerval = 5.0f;
    private WaitTimer m_waitTimer = null;


    public static LoadingEffectWithTips Attach(GameObject parent = null)
    {
        var effect = View.Attach<LoadingEffectWithTips>(PrefabPath, parent);
        UnityEngine.Debug.Assert(effect != null, "View.Attach<LoadingEffectWithTips>() failed.");
        return effect;
    }

    void Awake()
    {
        LoadingEffectCommon.Attach(m_commonEffectRoot);

        string description = "";
        if (UnityUtil.GetTextTry("common_loading2", ref description))
            m_descriptionText.text = description;
    }

    private void Update()
    {
        if (m_waitTimer == null)
            m_waitTimer = new WaitTimer(
                PagingInerval,
                () =>
                {
                    m_tipsModel.Next();
                    m_waitTimer = null;
                });
        else
            m_waitTimer.Tick(Time.deltaTime);
    }

    override public void SetTips(TipsModel model)
    {
        m_tipsModel = model;
        TipsView.Attach(m_TipsRoot).SetModel(m_tipsModel);
    }
}
