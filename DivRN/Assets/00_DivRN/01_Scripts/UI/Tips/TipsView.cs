using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TipsView : View
{
    [SerializeField]
    private Text m_titleText;
    [SerializeField]
    private Text m_detailText;

    private static readonly string PrefabPath = "Prefab/Common/Tips/TipsView";

    private TipsModel m_model;


    public static TipsView Attach(GameObject parent)
    {
        var view = View.Attach<TipsView>(PrefabPath, parent);
        UnityEngine.Debug.Assert(view != null, "View.Attach<TipsView>() failed.");
        return view;
    }


    public TipsView SetModel(TipsModel model)
    {
        m_model = model;

        m_model.OnUpdated += () =>
        {
            SetTip();
        };

        SetTip();

        return this;
    }

    private void SetTip()
    {
        if (m_model == null)
            return;

        var tipSet = m_model.tipSet;
        m_titleText.text = tipSet.title;
        m_detailText.text = tipSet.detail;
    }
}
