using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;

public class TabButtonView : ButtonView
{
    [SerializeField]
    protected Image m_baseButtonImage;
    [SerializeField]
    protected Image m_selectedFrameImage;

    protected TabButtonModel m_model = null;


    public static TabButtonView Attach(string prefabPath, GameObject parent)
    {
        return ButtonView.Attach<TabButtonView>(prefabPath, parent);
    }


    public void SetModel(TabButtonModel model)
    {
        m_model = model;

        m_model.OnUpdated += () =>
        {
            m_selectedFrameImage.gameObject.SetActive(m_model.isSelected);
        };

        base.SetModel<TabButtonModel>(model);

        m_selectedFrameImage.gameObject.SetActive(m_model.isSelected);
    }
}
