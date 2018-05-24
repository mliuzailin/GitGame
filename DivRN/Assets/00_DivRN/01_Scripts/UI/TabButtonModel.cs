using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TabButtonModel : ButtonModel
{
    private bool m_isSelected = false;

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

    new public event EventHandler OnUpdated;

    public TabButtonModel()
    {
        base.OnUpdated += () =>
        {
            if (OnUpdated != null)
                OnUpdated();
        };
    }
}
