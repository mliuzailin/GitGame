using UnityEngine;
using System.Collections;

public class MainMenuHelp : MainMenuSeq
{
    MenuButtonList m_buttonList;
    public MenuButtonList m_ButtonList
    {
        get
        {
            if (m_buttonList == null)
            {
                m_buttonList = GetComponentInChildren<MenuButtonList>();
            }
            return m_buttonList;
        }
    }

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    public new void Update()
    {
        if (PageSwitchUpdate() == false)
        {
            return;
        }
    }
    protected override void PageSwitchSetting(bool initalize)
    {
        base.PageSwitchSetting(initalize);

        m_ButtonList.setupMenuList(MAINMENU_CATEGORY.HELP);

        MainMenuManager.Instance.currentCategory = MAINMENU_CATEGORY.HELP;
    }
}