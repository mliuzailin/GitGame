using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FriendReloadButton : ButtonView
{
    [SerializeField]
    Button m_Button = null;

    public void SetEnable(bool isEnable)
    {
        m_modelBase.isEnabled = isEnable;
    }

    public void SetReloadButtonModel(ButtonModel model)
    {
        model.OnUpdated += () =>
        {
            m_Button.interactable = m_modelBase.isEnabled;
        };

        SetModel(model);
    }
}
