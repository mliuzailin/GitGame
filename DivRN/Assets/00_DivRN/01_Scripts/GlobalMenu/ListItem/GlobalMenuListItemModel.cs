using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GlobalMenuListItemModel : ListItemModel
{
    public event EventHandler OnClosedWithSelected;

    public GlobalMenuListItemModel(uint index) : base(index) { }

    private bool m_isSelected = false;
    public bool isSelected
    {
        get { return m_isSelected; }
        set
        {
            m_isSelected = value;

        }
    }

    public override void Close()
    {
        if (m_isSelected
            && OnClosedWithSelected != null)
        {
            m_isReady = false;

            OnClosedWithSelected();
            return;
        }

        base.Close();
    }
}
