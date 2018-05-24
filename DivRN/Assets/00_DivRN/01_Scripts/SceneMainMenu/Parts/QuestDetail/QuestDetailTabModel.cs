using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class QuestDetailTabModel : ListItemModel
{
    public event EventHandler OnShowedNext;
    public event EventHandler OnSelected;

    private bool m_isSelected = false;
    public bool isSelected
    {
        get { return m_isSelected; }
        set
        {
            if (m_isSelected == value)
                return;


            m_isSelected = value;

            if (m_isSelected
                && OnSelected != null)
                OnSelected();
        }
    }

    public QuestDetailTabModel(uint index) : base(index)
    {
    }

    // 次のボタンを順番に表示する
    public void ShowNext()
    {
        if (OnShowedNext != null)
            OnShowedNext();
    }
}
