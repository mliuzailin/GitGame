using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using M4u;
using ServerDataDefine;


public class MainMenuFooterButtonModel : ButtonModel
{
    new public event ButtonModel.EventHandler OnUpdated;

    private bool m_isSelected = false;

    private List<MAINMENU_CATEGORY> m_categories = new List<MAINMENU_CATEGORY>();

    public MainMenuFooterButtonModel AddCategory(MAINMENU_CATEGORY category)
    {
        if (!m_categories.Contains(category))
            m_categories.Add(category);
        return this;
    }

    public bool IsCategoryOf(MAINMENU_CATEGORY category)
    {
        return m_categories.Contains(category);
    }

    public MainMenuFooterButtonModel()
    {
        base.OnUpdated += () =>
        {
            OnUpdated();
        };
    }


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
}
