using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class DropIconListItemModel : ListItemModel
{
    public event EventHandler OnShowedNext = null;
    public event EventHandler OnLoopStarted = null;

    private bool m_isUnit = true;
    public bool isUnit
    {
        get { return m_isUnit; }
        set { m_isUnit = value; }
    }

    public DropIconListItemModel(uint index) : base(index)
    {
        m_index = index;

    }

    public void ShowNext()
    {
        if (OnShowedNext != null)
            OnShowedNext();
    }

    public void LoopStart()
    {
        if (OnLoopStarted != null)
            OnLoopStarted();
    }
}
