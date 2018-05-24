using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class AreaSelectListItemModel : ListItemModel
{
    new public event EventHandler OnUpdated;
    public event EventHandler OnBeganShowingTitle;
    public event EventHandler OnShowedNext;

    private bool m_isActive = false;
    private bool m_isChallenge = false;

    public AreaSelectListItemModel(uint index) : base(index)
    {
        m_index = index;

        base.OnUpdated += () =>
        {
            if (OnUpdated != null)
                OnUpdated();
        };
    }

    public void ShowTitle()
    {
        if (OnBeganShowingTitle != null)
            OnBeganShowingTitle();
    }

    // 順番に出現させるときに次のアイテムを出現させる
    public void ShowNext()
    {
        if (OnShowedNext != null)
            OnShowedNext();
    }

    public bool isActive
    {
        get { return m_isActive; }
        set
        {
            m_isActive = value;

            if (OnUpdated != null)
                OnUpdated();
        }
    }

    public bool isChallenge
    {
        get { return m_isChallenge; }
        set
        {
            m_isChallenge = value;

            if (OnUpdated != null)
                OnUpdated();
        }
    }
}
