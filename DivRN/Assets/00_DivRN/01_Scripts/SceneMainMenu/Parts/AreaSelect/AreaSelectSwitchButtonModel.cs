using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AreaSelectSwitchButtonModel : ButtonModel
{
    new public event ButtonModel.EventHandler OnUpdated;
    public event ButtonModel.EventHandler OnShowedNext;

    protected bool m_isSelected = false;
    protected int m_index = 0;

    public bool isSelected
    {
        get { return m_isSelected; }
        set
        {
            m_isSelected = value;

            if (OnUpdated != null)
                OnUpdated();
        }
    }

    public AreaSelectSwitchButtonModel(int index)
    {
        m_index = index;

        base.OnUpdated += () =>
        {
            if (OnUpdated != null)
                OnUpdated();
        };
    }

    // 順番に出現させるときに次のアイテムを出現させる
    public void ShowNext()
    {
        if (OnShowedNext != null)
            OnShowedNext();
    }

    private string m_labelText = "";
    public string labelText
    {
        get { return m_labelText; }
        set
        {
            m_labelText = value;
            if (OnUpdated != null)
                OnUpdated();
        }
    }

    public int index { get { return m_index; } }
}
