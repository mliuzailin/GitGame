using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PartySelectGroupUnitListItemModel : ListItemModel
{
    public event EventHandler OnViewStarted = null;
    public event EventHandler OnShowedIcon = null;
    public event EventHandler OnShowedName = null;
    public event EventHandler OnShowedNextIcon = null;
    public event EventHandler OnShowedNextName = null;


    new public event EventHandler OnUpdated = null;

    private bool m_isSelected = false;
    public bool isSelected
    {
        get { return m_isSelected; }
        set
        {
            bool isUpdated = m_isSelected != value;

            m_isSelected = value;

            if (isUpdated
                && OnUpdated != null)
                OnUpdated();
        }
    }

    public PartySelectGroupUnitListItemModel(uint index) : base(index)
    {
        m_index = index;

        base.OnUpdated += () =>
        {
            if (OnUpdated != null)
                OnUpdated();
        };
    }

    public void ViewStarted()
    {
        if (OnViewStarted != null)
            OnViewStarted();
    }

    public void ShowIcon()
    {
        if (OnShowedIcon != null)
            OnShowedIcon();
    }

    public void ShowName()
    {
        if (OnShowedName != null)
            OnShowedName();
    }

    public void ShowNextIcon()
    {
        if (OnShowedNextIcon != null)
            OnShowedNextIcon();
    }

    public void ShowNextName()
    {
        if (OnShowedNextName != null)
            OnShowedNextName();
    }
}
